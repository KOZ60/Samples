namespace Calculator
{
    using System.ComponentModel;

    internal class ResultLabel : Label
    {
        private const int DefaultMaxNumberOfDigits = 15;
        private int _MaxNumberOfDigits;
        private decimal MaxValue;
        private decimal MinValue;

        public ResultLabel() {
            MaxNumberOfDigits = DefaultMaxNumberOfDigits;
        }

        public override string Text {
            get {
                return base.Text;
            }
            set {
                if (DesignMode) {
                    base.Text = value;
                } else {
                    if (ValidText(value)) {
                        base.Text = CustomFormat(value);
                    }
                }
            }
        }

        [DefaultValue(DefaultMaxNumberOfDigits)]
        public int MaxNumberOfDigits { 
            get {
                 return _MaxNumberOfDigits;
            }
            set {
                if (value < 1 || value > 15) {
                    throw new ArgumentOutOfRangeException();
                }
                _MaxNumberOfDigits = value;
                MaxValue = (decimal)Math.Pow(10, value) - 1;
                MinValue = 0 - MaxValue;
            }
        }

        private bool ValidText(string value) {
            if (value != null) {
                int pos = value.IndexOf('.');
                if (pos >= 0) {
                    value = value.Substring(0, pos);
                }
                value = value.Replace("-", "");
                value = value.Replace(",", "");
                return value.Length <= MaxNumberOfDigits;
            }
            return true;
        }

        private string CustomFormat(string value) {
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

        public decimal Value {
            set {
                if (value < MinValue || value > MaxValue) {
                    throw new OverflowException();
                }
                Text = value.ToString(String.Format("G{0}", MaxNumberOfDigits));
            }
        }
    }
}
