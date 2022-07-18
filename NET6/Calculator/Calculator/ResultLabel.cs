namespace Calculator
{
    internal class ResultLabel : Label
    {
        public override string Text {
            get {
                return base.Text;
            }
            set {
                if (value == null) {
                    value = string.Empty;
                }
                if (decimal.TryParse(value, out decimal decimalValue)) {
                    int decimalPosition = value.IndexOf('.');
                    if (decimalPosition == -1) {
                        base.Text = decimalValue.ToString("#,##0");
                    } else {
                        int decimalDigits = value.Length - decimalPosition - 1;
                        if (decimalDigits > 0) {
                            string formatString = "#,##0." + new string('0', decimalDigits);
                            base.Text = decimalValue.ToString(formatString);
                        } else {
                            base.Text = decimalValue.ToString("#,##0") + ".";
                        }
                    }
                } else {
                    base.Text = value;
                }
            }
        }
    }
}
