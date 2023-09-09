namespace Calculator
{
    internal class CalcButton : Button
    {
        public CalcButton() {
            SetStyle(ControlStyles.Selectable, false);
        }

        public char Caption { get; set; } = '\0';
    }
}
