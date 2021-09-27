namespace CustomTextBox
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Text;
    using System.Windows.Forms;

    public class RestrictText : TextBox
    {
        private const int WM_CUT = 0x0300;
        private const int WM_COPY = 0x0301;
        private const int WM_PASTE = 0x0302;
        private const int WM_CLEAR = 0x0303;

        private const char BACKSPACE = '\b';    // Backspace キー
        private const char CTRL_A = '\x01';     // CTRL+A（全選択）
        private const char CTRL_C = '\x03';     // CTRL+C（コピー）
        private const char CTRL_V = '\x16';     // CTRL+V（貼り付け）
        private const char CTRL_X = '\x18';     // CTRL+X（切り取り）
        private const char CTRL_Z = '\x1A';     // CTRL+Z（元に戻す）

        private StringBuilder pasteText = new StringBuilder();
        private StringBuilder checkText = new StringBuilder();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool valid = true;
            string left, right;
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    // Delete キーが押された時
                    GetUnSelectedText(out left, out right);
                    if (SelectionLength > 0)
                    {
                        // 選択文字列が存在する場合は消去される
                        valid = IsValidText(left + right);
                    }
                    else 
                    {
                        // 選択文字列が存在しない場合は右の文字が消去される
                        if (right.Length > 0)
                        {
                            valid = IsValidText(left + right.Substring(1));
                        }
                    }
                    break;
            }
            if (!valid)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            base.OnKeyDown(e);
        }

        // 非選択文字を取得
        protected void GetUnSelectedText(out string left, out string right)
        {
            string prevText = base.Text;
            left = prevText.Substring(0, SelectionStart);
            right = prevText.Substring(SelectionStart + SelectionLength);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            bool valid = true;
            string left, right;
            switch (e.KeyChar)
            {
                case BACKSPACE:
                    // Backspace キーが押された時
                    GetUnSelectedText(out left, out right);
                    if (SelectionLength > 0)
                    {
                        // 選択文字列が存在する場合は消去される
                        valid = IsValidText(left + right);
                    }
                    else 
                    {
                        // 選択文字列が存在しない場合は左の文字が消去される
                        if (left.Length > 0)
                        {
                            valid = IsValidText(left.Substring(0, left.Length - 1) + right);
                        }
                    }
                    break;

                case CTRL_A:
                    // 全選択
                    break;

                // クリップボード操作は通す
                case CTRL_C:
                case CTRL_V:
                case CTRL_X:
                case CTRL_Z:
                    break;

                default:
                    if (IsValidChar(e.KeyChar))
                    {
                        GetUnSelectedText(out left, out right);
                        checkText.Clear();
                        checkText.Append(left);
                        checkText.Append(e.KeyChar);
                        checkText.Append(right);
                        valid = IsValidText(checkText.ToString());
                    }
                    else
                    {
                        valid = false;
                    }
                    break;
            }
            // エラーがあれば抑止
            if (!valid)
            {
                e.Handled = true;
                return;
            }
            base.OnKeyPress(e);
        }

        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            string left, right;
            switch (m.Msg)
            {
                case WM_CUT:
                case WM_CLEAR:
                    // 選択文字列が消去されるのでチェック
                    GetUnSelectedText(out left, out right);
                    if (IsValidText(left + right))
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case WM_PASTE:
                    // 貼り付け時
                    if (Clipboard.ContainsText())
                    {
                        // クリップボードの内容を1文字ずつチェック
                        GetUnSelectedText(out left, out right);
                        pasteText.Clear();
                        foreach (var c in Clipboard.GetText())
                        {
                            if (IsValidChar(c))
                            {
                                checkText.Clear();
                                checkText.Append(left);
                                checkText.Append(pasteText.ToString());
                                checkText.Append(c);
                                checkText.Append(right);
                                if (IsValidText(checkText.ToString()))
                                {
                                    pasteText.Append(c);
                                }
                            }
                        }
                        // 貼り付け文字があれば貼り付ける
                        if (pasteText.Length > 0)
                        {
                            base.Paste(pasteText.ToString());
                        }
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected virtual bool IsValidChar(char keyChar)
        {
            return true;
        }

        protected virtual bool IsValidText(string text)
        {
            return (MaxLength == 0 || text.Length <= MaxLength);
        }

        protected override void OnEnter(EventArgs e)
        {
            this.SelectAll();
            base.OnEnter(e);
        }
    }
}
