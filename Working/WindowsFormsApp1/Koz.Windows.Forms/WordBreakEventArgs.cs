using System;
using System.Runtime.InteropServices;

namespace Koz.Windows.Forms
{
    public class WordBreakEventArgs : EventArgs
    {
        internal WordBreakEventArgs(IntPtr lpch, int ichCurrent, int cch, WordBreakCode code) {
            Text = Marshal.PtrToStringAuto(lpch, cch);
            Position = ichCurrent;
            TextLength = cch;
            Code = code;
        }

        public string Text { get; }
        public int Position { get; }
        public int TextLength { get; }
        public WordBreakCode Code;
        public int Result { get; set; }
    }
}
