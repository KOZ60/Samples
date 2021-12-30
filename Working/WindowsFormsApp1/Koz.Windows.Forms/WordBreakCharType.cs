namespace Koz.Windows.Forms
{
    [System.Flags]
    internal enum WordBreakCharType
    {
        HalfDelimiter   = 0x001000,
        HalfNumber      = 0x002000,
        HalfAlphabet    = 0x004000,
        HalfKana        = 0x008000,
        WideNumber      = 0x010000,
        WideAlphabet    = 0x020000,
        WideKana        = 0x040000,
        WideHiragana    = 0x080000,
        WideDelimiter   = 0x100000,
        Others          = 0x200000,
    }
}
