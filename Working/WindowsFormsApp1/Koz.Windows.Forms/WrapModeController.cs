using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class WrapModeController : NativeWindowBase<TextBoxBase>
    {
        public const WrapMode DefaultWrapMode = WrapMode.NoWrap;

        private delegate int EditWordBreakProc(IntPtr lpch, int ichCurrent, int cch, WordBreakCode code);
        private readonly EditWordBreakProc EditWordBreakProcDelegate;
        protected bool inDoubleClick = false;
        protected WrapMode wrapMode;

        public WrapModeController(TextBoxBase textBox) : base(textBox) {
            EditWordBreakProcDelegate = new EditWordBreakProc(EditWordBreak);
            this.WrapMode = WrapMode.NoWrap;
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            NativeMethods.SendMessage(new HandleRef(this, Handle),
                    NativeMethods.EM_SETWORDBREAKPROC, IntPtr.Zero, EditWordBreakProcDelegate);
        }

        public WrapMode WrapMode { 
            get {
                return wrapMode;
            }
            set {
                wrapMode = value;
                switch (value) {
                    case WrapMode.NoWrap:
                        Owner.WordWrap = false;
                        break;
                    default:
                        Owner.WordWrap = true;
                        break;
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

        protected virtual void OnWordBreak(WordBreakEventArgs e) {
            WordBreak?.Invoke(this, e);
        }

        public event EventHandler<WordBreakEventArgs> WordBreak;

        private int EditWordBreak(IntPtr lpch, int ichCurrent, int cch, WordBreakCode code) {

            if (WrapMode == WrapMode.CharWrap && !inDoubleClick) {
                switch (code) {
                    case WordBreakCode.IsDelimiter:
                        return 0;
                    case WordBreakCode.Left:
                        return 0;
                    case WordBreakCode.Right:
                        return cch;
                }
            } else {
                WordBreakEventArgs e = new WordBreakEventArgs(lpch, ichCurrent, cch, code);
                e.Result = EditWordBreak(e);
                OnWordBreak(e);
                return e.Result;
            }
            return 0;
        }

        private static int EditWordBreak(WordBreakEventArgs e) {
            string text = e.Text;
            int pos = e.Position;
            int length = e.TextLength;

            switch (e.Code) {
                case WordBreakCode.IsDelimiter: {
                        switch (GetCharType(text[pos])) {
                            case CharType.HalfDelimiter:
                            case CharType.WideDelimiter:
                                return 1;
                        }
                        return 0;
                    }

                case WordBreakCode.Left: {
                        char currentChar = text[pos];
                        CharType current = GetCharType(currentChar);
                        UnicodeCategory category = char.GetUnicodeCategory(currentChar);
                        pos--;
                        while (pos >= 0) {
                            char c = text[pos];
                            CharType charType = GetCharType(c);
                            if (charType != current) {
                                return pos + 1;
                            }
                            if (current == CharType.Others) {
                                if (char.GetUnicodeCategory(c) != category) {
                                    return pos + 1;
                                }
                            }
                            pos--;
                        }
                    }
                    return 0;

                case WordBreakCode.Right: {
                        if (pos > 0) {
                            pos--;
                        }
                        char currentChar = text[pos];
                        CharType current = GetCharType(currentChar);
                        UnicodeCategory category = char.GetUnicodeCategory(currentChar);
                        while (pos < length) {
                            char c = text[pos];
                            CharType charType = GetCharType(c);
                            if (charType != current) {
                                return pos;
                            }
                            if (current == CharType.Others) {
                                if (char.GetUnicodeCategory(c) != category) {
                                    return pos;
                                }
                            }
                            pos++;
                        }
                    }
                    return length;
            }
            return 0;
        }

        private enum CharType
        {
            HalfDelimiter,
            HalfNumber,
            HalfAlphabet,
            HalfKana,
            WideNumber,
            WideAlphabet,
            WideKana,
            WideHiragana,
            WideDelimiter,
            Others,
        }

        private static CharType GetCharType(char c) {
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
                return CharType.HalfDelimiter;
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
                return CharType.WideDelimiter;
            }

            if (char.IsWhiteSpace(c)) {
                return CharType.WideDelimiter;
            }
            return CharType.Others;
        }
    }
}
