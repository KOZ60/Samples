using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class WordBreakController : NativeWindow
    {
        public const WrapMode DefaultWrapMode = WrapMode.NoWrap;

        private readonly TextBoxBase textBox;
        private readonly NativeMethods.EditWordBreakProc EditWordBreakProc;
        private bool inDoubleClick = false;
        private WrapMode wrapMode;

        public WordBreakController(TextBoxBase textBox) {
            this.textBox = textBox;
            EditWordBreakProc = new NativeMethods.EditWordBreakProc(EditWordBreak);
            this.WrapMode = WrapMode.NoWrap;
            textBox.HandleCreated += TextBox_HandleCreated;
            if (textBox.IsHandleCreated) {
                TextBox_HandleCreated(textBox, EventArgs.Empty);
            }
        }

        private void TextBox_HandleCreated(object sender, EventArgs e) {
            AssignHandle(((Control)sender).Handle);
            NativeMethods.SendMessage(new HandleRef(this, Handle),
                    NativeMethods.EM_SETWORDBREAKPROC, IntPtr.Zero, EditWordBreakProc);
        }

        public WrapMode WrapMode { 
            get {
                return wrapMode;
            }
            set {
                wrapMode = value;
                switch (value) {
                    case WrapMode.NoWrap:
                        textBox.WordWrap = false;
                        break;
                    default:
                        textBox.WordWrap = true;
                        break;
                }
                if (textBox.IsHandleCreated) {
                    textBox.Invalidate();
                }
            }
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_NCDESTROY:
                    base.WndProc(ref m);
                    ReleaseHandle();
                    break;
                case NativeMethods.WM_RBUTTONDBLCLK:
                    inDoubleClick = true;
                    base.WndProc(ref m);
                    inDoubleClick = false;
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private int EditWordBreak(IntPtr lpch, int ichCurrent, int cch, NativeMethods.EditWordBreakCode code) {
            if (WrapMode == WrapMode.CharWrap && !inDoubleClick) {
                switch (code) {
                    case NativeMethods.EditWordBreakCode.IsDelimiter:
                        return 0;
                    case NativeMethods.EditWordBreakCode.Left:
                        return 0;
                    case NativeMethods.EditWordBreakCode.Right:
                        return cch;
                }

            } else {

                unsafe {

                    char* buffer = (char*)lpch.ToPointer();
                    switch (code) {
                        case NativeMethods.EditWordBreakCode.IsDelimiter: {
                                char currentChar = buffer[ichCurrent];
                                CharType current = GetCharType(currentChar);
                                return (current == CharType.Delimiter) ? 1 : 0;
                            }

                        case NativeMethods.EditWordBreakCode.Left: {
                                char currentChar = buffer[ichCurrent];
                                CharType current = GetCharType(currentChar);
                                UnicodeCategory category = char.GetUnicodeCategory(currentChar);
                                ichCurrent--;
                                while (ichCurrent >= 0) {
                                    char c = buffer[ichCurrent];
                                    CharType charType = GetCharType(c);
                                    if (charType != current) {
                                        return ichCurrent + 1;
                                    }
                                    if (current == CharType.Others) {
                                        if (char.GetUnicodeCategory(c) != category) {
                                            return ichCurrent + 1;
                                        }
                                    }
                                    ichCurrent--;
                                }
                            }
                            return 0;

                        case NativeMethods.EditWordBreakCode.Right: {
                                if (ichCurrent > 0) {
                                    ichCurrent--;
                                }
                                char currentChar = buffer[ichCurrent];
                                CharType current = GetCharType(currentChar);
                                UnicodeCategory category = char.GetUnicodeCategory(currentChar);
                                while (ichCurrent < cch) {
                                    char c = buffer[ichCurrent];
                                    CharType charType = GetCharType(c);
                                    if (charType != current) {
                                        return ichCurrent;
                                    }
                                    if (current == CharType.Others) {
                                        if (char.GetUnicodeCategory(c) != category) {
                                            return ichCurrent;
                                        }
                                    }
                                    ichCurrent++;
                                }
                            }
                            return cch;
                    }

                }
            }
            return 0;
        }

        private enum CharType
        {
            Delimiter,
            HalfNumber,
            HalfAlphabet,
            HalfKana,
            WideNumber,
            WideAlphabet,
            WideKana,
            WideHiragana,
            Others,
        }

        private CharType GetCharType(char c) {
            if (c >= '0' && c <= '9') {
                return CharType.HalfNumber;
            }
            if (c >= 'A' && c <= 'Z') {
                return CharType.HalfAlphabet;
            }
            if (c >= 'a' && c <= 'z') {
                return CharType.HalfAlphabet;
            }
            if (c >= 166 && c <= 221) {
                return CharType.HalfKana;
            }
            if (c < 0xFF) {
                return CharType.Delimiter;
            }
            if (c >= '０' && c <= '９') {
                return CharType.WideNumber;
            }
            if (c >= 'Ａ' && c <= 'Ｚ') {
                return CharType.WideAlphabet;
            }
            if (c >= 'ａ' && c <= 'ｚ') {
                return CharType.WideAlphabet;
            }
            if (c >= 'ぁ' && c <= 'ん') {
                return CharType.WideHiragana;
            }
            if (c == 'ー') {
                return CharType.WideKana;
            }
            if (c >= 'ァ' && c <= 'ヶ') {
                return CharType.WideKana;
            }
            if (c >= '\u3000' && c < '０') {
                return CharType.Delimiter;
            }

            if (char.IsWhiteSpace(c)) {
                return CharType.Delimiter;
            }
            return CharType.Others;
        }
    }
}
