namespace Calculator
{
    internal class NoFocusButton : Button
    {
        public NoFocusButton() {
            SetStyle(ControlStyles.Selectable, false);
        }
    }
}
