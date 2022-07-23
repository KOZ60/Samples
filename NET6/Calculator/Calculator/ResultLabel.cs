namespace Calculator
{
    internal class ResultLabel : Label
    {
        public override string Text {
            get {
                return base.Text;
            }
            set {
                if (DesignMode) {
                    base.Text = value;
                } else {
                    base.Text = CustomFormat(value);
                }
            }
        }

        private static string CustomFormat(string value) {
            if (string.IsNullOrEmpty(value)) {
                return "0";
            } else if (decimal.TryParse(value, out decimal decimalValue)) {
                int decimalPosition = value.IndexOf('.');
                if (decimalPosition == -1) {
                    return decimalValue.ToString("#,##0");
                } else {
                    int decimalDigits = value.Length - decimalPosition - 1;
                    if (decimalDigits > 0) {
                        string formatString = "#,##0." + new string('0', decimalDigits);
                        return decimalValue.ToString(formatString);
                    } else {
                        return decimalValue.ToString("#,##0") + ".";
                    }
                }
            } else {
                return value;
            }
        }
    }
}
