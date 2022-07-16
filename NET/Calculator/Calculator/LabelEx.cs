namespace Calculator
{
    internal class LabelEx : Label
    {
        public override string Text {
            get {
                return base.Text;
            }
            set {
                if (value == null) {
                    value = string.Empty;
                }
                if (decimal.TryParse(value, out decimal decValue)) {
                    int pos = value.IndexOf('.');
                    if (pos == -1) {
                        base.Text = decValue.ToString("#,##0");
                    } else if (pos < value.Length - 1) {
                        string formatString;
                        formatString = "#,##0." + new string('0', value.Length - pos - 1);
                        base.Text = decValue.ToString(formatString);
                    } else {
                        base.Text = decValue.ToString("#,##0") + ".";
                    }
                } else {
                    base.Text = value;
                }
            }
        }
    }
}
