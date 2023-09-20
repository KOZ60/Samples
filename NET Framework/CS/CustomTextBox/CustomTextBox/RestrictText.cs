namespace CustomTextBox
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    public class RestrictText : TextBox
    {
        public virtual string DenyChars { get; set; } = string.Empty;

        public virtual void ResetDenyChars() {
            DenyChars = string.Empty; 
        }

        protected virtual bool ShouldSerializeDenyChars() {
            return !string.IsNullOrEmpty(DenyChars);
        }

        private bool canRaiseEvents = true;

        protected override void OnKeyDown(KeyEventArgs e) {
            bool valid = true;
            switch (e.KeyCode) {
                case Keys.Delete:
                    // Delete キーが押された時
                    GetUnSelectedText(out string left, out string right);
                    if (SelectionLength > 0) {
                        // 選択文字列が存在する場合は消去される
                        valid = IsValidText(left + right);
                    } else {
                        // 選択文字列が存在しない場合は右の文字が消去される
                        if (right.Length > 0) {
                            valid = IsValidText(left + right.Substring(1));
                        }
                    }
                    break;
            }
            if (!valid) {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            base.OnKeyDown(e);
        }

        // 非選択文字を取得
        protected void GetUnSelectedText(out string left, out string right) {
            string prevText = base.Text;
            left = prevText.Substring(0, SelectionStart);
            right = prevText.Substring(SelectionStart + SelectionLength);
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
            bool valid = true;
            string left, right;
            switch (e.KeyChar) {
                case NativeMethods.BACKSPACE:
                    // Backspace キーが押された時
                    GetUnSelectedText(out left, out right);
                    if (SelectionLength > 0) {
                        // 選択文字列が存在する場合は消去される
                        valid = IsValidText(left + right);
                    } else {
                        // 選択文字列が存在しない場合は左の文字が消去される
                        if (left.Length > 0) {
                            valid = IsValidText(left.Substring(0, left.Length - 1) + right);
                        }
                    }
                    break;

                case NativeMethods.CTRL_A:
                    // 全選択
                    break;

                // クリップボード操作は通す
                case NativeMethods.CTRL_C:
                case NativeMethods.CTRL_V:
                case NativeMethods.CTRL_X:
                case NativeMethods.CTRL_Z:
                    break;

                default:
                    if (IsValidChar(e.KeyChar)) {
                        GetUnSelectedText(out left, out right);
                        var checkText = StringBuilderCache.Acquire();
                        checkText.Append(left);
                        checkText.Append(e.KeyChar);
                        checkText.Append(right);
                        valid = IsValidText(StringBuilderCache.GetStringAndRelease(checkText));
                    } else {
                        valid = false;
                    }
                    break;
            }
            // エラーがあれば抑止
            if (!valid) {
                e.Handled = true;
                return;
            }
            base.OnKeyPress(e);
        }

        [DebuggerStepThrough]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_CUT:
                case NativeMethods.WM_CLEAR:
                    // 切り取りもしくはクリア
                    WmCutOrWmClear(ref m);
                    break;

                case NativeMethods.WM_COPY:
                    // COPY は OK
                    base.WndProc(ref m);
                    break;

                case NativeMethods.WM_PASTE:
                    // 貼り付け時
                    WmPaste(ref m);
                    break;

                case NativeMethods.WM_UNDO:
                case NativeMethods.EM_UNDO:
                    // 元に戻す
                    WmUndo(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmCutOrWmClear(ref Message m) {
            // 選択文字列が消去される
            GetUnSelectedText(out string left, out string right);
            if (IsValidText(left + right)) {
                base.WndProc(ref m);
            }
        }

        private void WmPaste(ref Message m) {
            if (!Clipboard.ContainsText()) {
                return;
            }
            // クリップボードの内容を1文字ずつチェック
            GetUnSelectedText(out string left, out string right);
            var pasteText = StringBuilderCache.Acquire();
            foreach (var c in Clipboard.GetText()) {
                if (IsValidChar(c)) {
                    var checkText = StringBuilderCache.Acquire();
                    checkText.Append(left);
                    checkText.Append(pasteText.ToString());
                    checkText.Append(c);
                    checkText.Append(right);
                    if (IsValidText(StringBuilderCache.GetStringAndRelease(checkText))) {
                        pasteText.Append(c);
                    }
                }
            }
            // 貼り付け文字があれば貼り付ける
            if (pasteText.Length > 0) {
                Paste(StringBuilderCache.GetStringAndRelease(pasteText));
            } else {
                StringBuilderCache.Release(pasteText);
            }
            m.Result = IntPtr.Zero;
        }

        private void WmUndo(ref Message m) {
            // 元に戻してまずかったら前の値に戻す
            NativeMethods.LockWindowUpdate(m.HWnd);
            try {
                string prevText = base.Text;
                base.WndProc(ref m);
                if (!IsValidText(base.Text)) {
                    SetTextWithoutEvents(prevText);
                }
            } finally {
                NativeMethods.LockWindowUpdate(IntPtr.Zero);
                Invalidate();
            }
        }

        protected override bool CanRaiseEvents {
            get {
                return canRaiseEvents;
            }
        }

        protected void SetTextWithoutEvents(string value) {
            if (base.Text != value) {
                canRaiseEvents = false;
                try {
                    base.Text = value;
                } finally {
                    canRaiseEvents = true;
                }
            }
        }

        protected virtual bool IsValidChar(char keyChar) {
            return DenyChars.IndexOf(keyChar) < 0;
        }

        protected virtual bool IsValidText(string text) {
            return (MaxLength == 0 || text.Length <= MaxLength);
        }

        protected override void OnEnter(EventArgs e) {
            SetTextWithoutEvents(UnFormatText(base.Text));
            SelectAll();
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e) {
            SetTextWithoutEvents(FormatText(base.Text));
            base.OnLeave(e);
        }

        protected virtual string UnFormatText(string text) {
            var sb = StringBuilderCache.Acquire();
            foreach (char c in text) {
                if (IsValidText(sb.ToString() + c)) {
                    sb.Append(c);
                }
            }
            return StringBuilderCache.GetStringAndRelease(sb);
        }

        protected virtual string FormatText(string text) {
            return text;
        }

    }
}
