using System;
using System.Runtime.InteropServices;

namespace Koz.Windows.Forms
{
    public class WordBreakEventArgs : EventArgs
    {
        internal readonly IntPtr textPtr;
        private string text;

        internal WordBreakEventArgs(WrapMode mode, IntPtr lpch, int ichCurrent, int cch, WordBreakCode code) {
            WrapMode = mode;
            textPtr = lpch;
            Position = ichCurrent;
            TextLength = cch;
            Code = code;
        }

        public string Text {
            get {
                // テキストが大きくなるとコストが高くなるため、ポインタを保持しておき、
                // Text プロパティが呼ばれた時だけ string に変換する
                if (text == null) {
                    text = Marshal.PtrToStringAuto(textPtr, TextLength);
                }
                return text;
            }
        }
        public WrapMode WrapMode { get; }
        public int Position { get; }
        public int TextLength { get; }
        public WordBreakCode Code;
        public int Result { get; set; }
        public bool Handled { get; set; }

    }
}
