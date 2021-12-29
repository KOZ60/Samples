namespace Koz.Windows.Forms
{
    public interface IWrapModeControl
    {
        WrapMode WrapMode { get; set; }
        void WordBreakCallback(WordBreakEventArgs e);
    }
}
