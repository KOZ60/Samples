namespace Calculator
{
    internal class NoFocusButton : Button
    {
        public NoFocusButton() {
            SetStyle(ControlStyles.Selectable, false);
        }

        public char CharValue { get; set; } = '\0';
    }
}
