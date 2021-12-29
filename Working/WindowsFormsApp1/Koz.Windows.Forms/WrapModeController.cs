using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class WrapModeController<T> : NativeWindowBase<T> where T : Control, IWrapModeControl
    {
        private readonly NativeMethods.EditWordBreakProc EditWordBreakDelegate;
        protected bool inMouseEvent = false;
        protected WrapMode wrapMode;

        public WrapModeController(T textBox) : base(textBox) {
            EditWordBreakDelegate = new NativeMethods.EditWordBreakProc(EditWordBreak);
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            NativeMethods.SendMessage(new HandleRef(this, Handle),
                    NativeMethods.EM_SETWORDBREAKPROC, IntPtr.Zero, EditWordBreakDelegate);
        }

        protected override void WndProc(ref Message m) {

            // マウス操作があったらフラグをON
            if (m.Msg >= NativeMethods.WM_MOUSEFIRST &&
                m.Msg <= NativeMethods.WM_MOUSELAST) {

                inMouseEvent = true;
                base.WndProc(ref m);
                inMouseEvent = false;

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
                if (!e.Handled) {
                    e.Result = EditWordBreak(e);
                }
                return e.Result;
            }
            return 0;
        }

        private unsafe static int EditWordBreak(WordBreakEventArgs e) {
            char* text = (char*)e.textPtr.ToPointer();
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
                        if (pos >= e.TextLength) {
                            pos = e.TextLength - 1;
                        }
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
