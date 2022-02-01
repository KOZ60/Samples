using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CustomTextBox
{
    public class PaddingTextBox : TextBox
    {
        private static Padding defaultPadding = new Padding(0, 2, 0, 2);
        private bool initLayouted = false;
        private bool autoSize = true;

        public PaddingTextBox() {
            base.Padding = DefaultPadding;
            base.AutoSize = false;
            SetStyle(ControlStyles.FixedHeight, true);
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool AutoSize {
            get {
                return autoSize;
            }
            set {
                if (AutoSize != value) {
                    autoSize = value;
                    OnAutoSizeChanged(EventArgs.Empty);
                }
            }
        }

        protected override Padding DefaultPadding {
            get {
                return defaultPadding;
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Padding Padding {
            get {
                return base.Padding;
            }
            set {
                base.Padding = value;
            }
        }

        protected override CreateParams CreateParams {
            get {
                var cp = base.CreateParams;
                cp.Style |= NativeMethods.ES_MULTILINE;
                return cp;
            }
        }

        protected override void OnAutoSizeChanged(EventArgs e) {
            DoLayout();
            base.OnAutoSizeChanged(e);
        }

        protected override void OnMultilineChanged(EventArgs e) {
            DoLayout();
            base.OnMultilineChanged(e);
        }

        protected override void OnHandleCreated(EventArgs e) {
            DoLayout();
            base.OnHandleCreated(e);
        }

        protected override void OnPaddingChanged(EventArgs e) {
            DoLayout();
            base.OnPaddingChanged(e);
        }

        protected override void OnFontChanged(EventArgs e) {
            DoLayout();
            base.OnFontChanged(e);
        }

        protected override void OnResize(EventArgs e) {
            DoLayout();
            base.OnResize(e);
        }

        protected override void InitLayout() {
            base.InitLayout();
            initLayouted = true;
            DoLayout();
        }

        protected virtual void DoLayout() {
            if (!initLayouted) return;

            bool fixedHeight = AutoSize && !Multiline;
            SetStyle(ControlStyles.FixedHeight, fixedHeight);

            // フォントの大きさ + Padding から ClientSize の最小値を計算
            Size fontAvgSize = NativeMethods.GetFontAverageSize(Font);
            int minHeight = fontAvgSize.Height + Padding.Top + Padding.Bottom;
            int minWidth = fontAvgSize.Width + Padding.Left + Padding.Right;

            bool needChange = false;
            Size cs = ClientSize;

            // 高さ調整
            if (fixedHeight) {
                // 固定長の時は最小値に
                if (cs.Height != minHeight) {
                    cs.Height = minHeight;
                    needChange = true;
                }
            } else {
                if (cs.Height < minHeight) {
                    cs.Height = minHeight;
                    needChange = true;
                }
            }

            // 幅調整
            if (cs.Width < minWidth) {
                cs.Width = minWidth;
                needChange = true;
            }

            // ClientSize を設定
            if (needChange) {
                initLayouted = false;
                try {
                    ClientSize = cs;
                } finally {
                    initLayouted = true;
                }
            }

            // Padding を有効に
            Rectangle rectangle = LayoutUtils.DeflateRect(ClientRectangle, Padding);
            NativeMethods.RECT rect = new NativeMethods.RECT(rectangle);
            NativeMethods.SendMessage(Handle, NativeMethods.EM_SETRECT, IntPtr.Zero, ref rect);
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_SETTEXT:
                case NativeMethods.EM_REPLACESEL:
                    if (!Multiline) {
                        using (var buffer = RemoveControlChar(m.LParam)) {
                            var msg = Message.Create(m.HWnd, m.Msg, m.WParam, buffer.Handle);
                            base.WndProc(ref msg);
                            m.Result = msg.Result;
                        }
                    } else {
                        base.WndProc(ref m);
                    }
                    break;

                case NativeMethods.WM_CHAR:
                case NativeMethods.WM_IME_CHAR:
                    if (!Multiline) {
                        switch ((char)(int)m.WParam) {
                            case (char)0x0D:
                            case (char)0x0A:
                            case (char)0x09:
                                return;
                        }
                    }
                    base.WndProc(ref m);
                    break;

                case NativeMethods.WM_PASTE:
                    if (Clipboard.ContainsText()) {
                        string pasteText = Clipboard.GetText();
                        if (!Multiline) {
                            pasteText = RemoveControlChar(pasteText);
                        }
                        OnPaste(pasteText);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected string RemoveControlChar(string value) {
            var sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
                if (!char.IsControl(c)) {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static SafeStringBuffer RemoveControlChar(IntPtr ptr) {
            int length = NativeMethods.lstrlenW(ptr);
            var buffer = new SafeStringBuffer((length + 1) * 2);
            unsafe {
                char* src = (char*)ptr;
                char* dst = (char*)buffer.Handle;
                for (int i = 0; i < length; i++) {
                    if (!char.IsControl(src[i])) {
                        *dst = src[i];
                        dst++;
                    }
                }
                *dst = '\0';
            }
            return buffer;
        }

        protected virtual void OnPaste(string text) {
            Paste(text);
        }
    }
}
