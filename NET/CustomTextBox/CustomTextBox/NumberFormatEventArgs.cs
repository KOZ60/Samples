namespace CustomTextBox
{
    using System;

    public class NumberFormatEventArgs : EventArgs
    {
        public NumberFormatEventArgs(decimal value, string formatString, string formatedText)
        {
            this.Value = value;
            this.FormatString = formatString;
            this.FormatedText = formatedText;
        }

        public decimal Value { get; }
        public string FormatString { get; }
        public string FormatedText { get; set; }
    }
}
