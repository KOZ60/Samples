using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class WrapModeController<T> : NativeWindowBase<T> where T : Control, IWrapModeControl
    {
        private readonly NativeMethods.EditWordBreakProc editWordBreakDelegate;
        protected WrapMode wrapMode;

        public WrapModeController(T owner) : base(owner) {
            editWordBreakDelegate = new NativeMethods.EditWordBreakProc(EditWordBreak);
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            NativeMethods.SendMessage(new HandleRef(this, Handle),
                    NativeMethods.EM_SETWORDBREAKPROC, IntPtr.Zero, editWordBreakDelegate);
        }

        int mouseEventCount = 0;

        protected bool inMouseEvent {
            get {
                return mouseEventCount > 0;
            }
        }

        protected override void WndProc(ref Message m) {

            // マウス操作があったらフラグをON
            if (m.Msg >= NativeMethods.WM_MOUSEFIRST &&
                m.Msg <= NativeMethods.WM_MOUSELAST) {

                mouseEventCount++;
                try {
                    base.WndProc(ref m);
                } finally {
                    mouseEventCount--;
                }

            } else {
                base.WndProc(ref m);
            }
        }

        private int EditWordBreak(IntPtr lpch, int ichCurrent, int cch, WordBreakCode code) {

            if (Owner.WrapMode == WrapMode.CharWrap && !inMouseEvent) {
                switch (code) {
                    case WordBreakCode.IsDelimiter:
                        return 0;
                    case WordBreakCode.Left:
                        return 0;
                    case WordBreakCode.Right:
                        return cch;
                }
            } else {
                WordBreakEventArgs e = new WordBreakEventArgs(Owner.WrapMode, lpch, ichCurrent, cch, code);
                Owner.WordBreakCallback(e);
                return e.Result;
            }
            return 0;
        }

        public int DefaultWordBreakProc(WordBreakEventArgs e) {
            unsafe {
                char* text = (char*)e.textPtr.ToPointer();
                int pos = e.Position;
                int length = e.TextLength;

                switch (e.Code) {
                    case WordBreakCode.IsDelimiter: {
                            switch (GetWordBreakCharType(text[pos])) {
                                case WordBreakCharType.HalfDelimiter:
                                case WordBreakCharType.WideDelimiter:
                                    return 1;
                            }
                            return 0;
                        }

                    case WordBreakCode.Left: {
                            if (pos >= e.TextLength) {
                                pos = e.TextLength - 1;
                            }
                            char currentChar = text[pos];
                            WordBreakCharType current = GetWordBreakCharType(currentChar);
                            pos--;
                            while (pos >= 0) {
                                char c = text[pos];
                                WordBreakCharType charType = GetWordBreakCharType(c);
                                if (charType != current) {
                                    return pos + 1;
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
                            WordBreakCharType current = GetWordBreakCharType(currentChar);
                            while (pos < length) {
                                char c = text[pos];
                                WordBreakCharType charType = GetWordBreakCharType(c);
                                if (charType != current) {
                                    return pos;
                                }
                                pos++;
                            }
                        }
                        return length;
                }
                return 0;
            }
        }

        private static WordBreakCharType GetWordBreakCharType(char c) {
            if (c >= '0' && c <= '9') {
                return WordBreakCharType.HalfNumber;
            }
            if (c >= 'A' && c <= 'Z') {
                return WordBreakCharType.HalfAlphabet;
            }
            if (c >= 'a' && c <= 'z') {
                return WordBreakCharType.HalfAlphabet;
            }
            if (c >= 0xA6 && c <= 0xDD) {
                return WordBreakCharType.HalfKana;
            }
            if (c <= 0xFF) {
                return WordBreakCharType.HalfDelimiter;
            }

            if (c >= '０' && c <= '９') {
                return WordBreakCharType.WideNumber;
            }
            if (c >= 'Ａ' && c <= 'Ｚ') {
                return WordBreakCharType.WideAlphabet;
            }
            if (c >= 'ａ' && c <= 'ｚ') {
                return WordBreakCharType.WideAlphabet;
            }
            if (c >= 'ぁ' && c <= 'ん') {
                return WordBreakCharType.WideHiragana;
            }
            if (c == 'ー') {
                return WordBreakCharType.WideKana;
            }
            if (c >= 'ァ' && c <= 'ヶ') {
                return WordBreakCharType.WideKana;
            }
            if (c >= '\u3000' && c < '０') {
                return WordBreakCharType.WideDelimiter;
            }

            if (char.IsWhiteSpace(c)) {
                return WordBreakCharType.WideDelimiter;
            }

            UnicodeCategory category = char.GetUnicodeCategory(c);

            return WordBreakCharType.Others | (WordBreakCharType)(int)category;
        }
    }
}
