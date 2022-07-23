namespace Calculator
{
    public partial class CalcForm : Form
    {
        private enum Status
        {
            /// <summary>êîílÇPì¸óÕíÜ</summary>
            S1,
            /// <summary>ââéZéqì¸óÕå„ÅAì¸óÕë“Çø</summary>
            S2,
            /// <summary>êîílÇQì¸óÕíÜ</summary>
            S3,
            /// <summary>ââéZé¿çså„ÅAì¸óÕë“Çø</summary>
            S4,
            /// <summary>ÉGÉâÅ[î≠ê∂</summary>
            S5
        }

        private Status _MyStatus = Status.S1;
        private Status MyStatus { 
            get {
                return _MyStatus;
            }
            set {
                _MyStatus = value;
#if DEBUG
                Text = string.Format("ìdëÏ({0})", value);
#endif
            }
        }
        
        private char Operator = char.MinValue;
        private string Number1 = string.Empty;

        private string PrevNumber1 = string.Empty;
        private string PrevNumber2 = string.Empty;

        public CalcForm() {
            InitializeComponent();
            foreach (var button in Controls.OfType<CalcButton>()) {
                button.Click += Button_Click;
            }
            label1.Text = "0";
            MyStatus = Status.S1;
        }

        private void Button_Click(object? sender, EventArgs e) {
            var button = sender as CalcButton;
            if (button == null) return;
            ButtonClick(button.Caption);
        }

        private readonly HashSet<char> NumChars = new HashSet<char>(".0123456789");
        private readonly HashSet<char> CalcChars = new HashSet<char>("+-*/");

        private void ButtonClick(char caption) {
            try {
                if (NumChars.Contains(caption)) {
                    NumButtonClick(caption);
                } else if (CalcChars.Contains(caption)) {
                    CalcButtonClick(caption);
                } else if (caption == '=') {
                    EqualButtonClick();
                } else if (caption == 'R') {
                    RootButtonClick();
                } else if (caption == 'P') {
                    PercentButtonClick();
                } else if (caption == 'I') {
                    InvertButtonClick();
                } else if (caption == 'C') {
                    ClearButtonClick();
                }
            } catch (Exception ex) {
                label1.Text = "Error!";
                label1.ForeColor = Color.Red;
                MyStatus = Status.S5;
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearButtonClick() {
            Clear();
        }

        private void RootButtonClick() {
            switch (MyStatus) {
                case Status.S5:
                    break;
                default:
                    double d = double.Parse(label1.Text);
                    label1.Text = Math.Sqrt(d).ToString();
                    break;
            }
        }

        private void PercentButtonClick() {
            switch (MyStatus) {
                case Status.S1:
                case Status.S2:
                    break;
                case Status.S3:
                    PercentCalcuration();
                    MyStatus = Status.S4;
                    break;

                case Status.S4:
                    // êîílÇÉoÉbÉNÉAÉbÉvÇ©ÇÁñﬂÇµÇƒåvéZ
                    Number1 = PrevNumber1;
                    label1.Text = PrevNumber2;
                    PercentCalcuration();
                    break;

                case Status.S5:
                    break;
            }
        }

        private void PercentCalcuration() {
            string prevText;
            decimal tmp;

            switch (Operator) {
                case '/':
                case '*':
                    // èúéZ/èÊéZÇÃÇ∆Ç´ÇÕêîílÇQÇ1/100î{ÇµÇƒåvéZ
                    prevText = label1.Text;
                    tmp = Convert.ToDecimal(prevText) / 100;
                    label1.Text = tmp.ToString();
                    Calcuration();
                    PrevNumber2 = prevText;
                    break;
                case '+':
                case '-':
                    // â¡éZ/å∏éZÇÃÇ∆Ç´ÇÕêîílÇPÇÃêîílÇQÅìÇãÅÇﬂÇƒêîílÇQÇ∆ÇµÇƒåvéZ
                    prevText = label1.Text;
                    tmp = Convert.ToDecimal(Number1) / 100 * Convert.ToDecimal(label1.Text);
                    label1.Text = tmp.ToString();
                    Calcuration();
                    PrevNumber2 = prevText;
                    break;
            }
        }


        private void InvertButtonClick() {
            switch (MyStatus) {
                case Status.S5:
                    break;
                default:
                    decimal d = decimal.Parse(label1.Text);
                    label1.Text = d.ToString();
                    break;
            }
        }

        private void NumButtonClick(char caption) {
            switch (MyStatus) {
                case Status.S1:
                    EditLabel(label1.Text, caption);
                    break;

                case Status.S2:
                    EditLabel("0", caption);
                    MyStatus = Status.S3;
                    break;

                case Status.S3:
                    EditLabel(label1.Text, caption);
                    break;

                case Status.S4:
                    EditLabel("0", caption);
                    MyStatus = Status.S1;
                    break;
            }
        }

        private void CalcButtonClick(char caption) {
            switch (MyStatus) {
                case Status.S1:
                    SaveCaption(caption);
                    MyStatus = Status.S2;
                    break;

                case Status.S2:
                    SaveCaption(caption);
                    break;

                case Status.S3:
                    EqualButtonClick();
                    SaveCaption(caption);
                    MyStatus = Status.S2;
                    break;

                case Status.S4:
                    SaveCaption(caption);
                    MyStatus = Status.S2;
                    break;
            }
        }

        private void EqualButtonClick() {
            switch (MyStatus) {
                case Status.S1:
                    break;

                case Status.S2:
                    Calcuration();
                    MyStatus = Status.S4;
                    break;

                case Status.S3:
                    Calcuration();
                    MyStatus = Status.S4;
                    break;

                case Status.S4:
                    break;
            }
        }

        private void Clear() {
            label1.Text = "0";
            label1.ForeColor = SystemColors.ControlText;
            MyStatus = Status.S1;
            Operator = char.MinValue;
            Number1 = string.Empty;
        }

        private void EditLabel(string labelText, char caption) {
            switch (caption) {
                case '.':
                    if (!labelText.Contains('.')) {
                        labelText += ".";
                    }
                    break;
                case '0':
                    if (labelText != "0") {
                        labelText += "0";
                    }
                    break;
                default:
                    if (labelText != "0") {
                        labelText += caption;
                    } else {
                        labelText = caption.ToString();
                    }
                    break;
            }
            label1.Text = labelText;
        }

        private void SaveCaption(char caption) {
            Operator = caption;
            Number1 = label1.Text;
        }

        private void Calcuration() {
            PrevNumber1 = Number1;
            PrevNumber2 = label1.Text;
            decimal num1 = Convert.ToDecimal(Number1);
            decimal num2 = Convert.ToDecimal(label1.Text);
            decimal result = 0;
            switch (Operator) {
                case '/':
                    result = num1 / num2;
                    break;
                case '*':
                    result = num1 * num2;
                    break;
                case '-':
                    result = num1 - num2;
                    break;
                case '+':
                    result = num1 + num2;
                    break;
            }
            label1.Text = string.Format("{0:G28}", result);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e) {
            char caption;
            if (e.KeyChar == 0x0d) {
                caption = '=';
            } else {
                caption = e.KeyChar;
            }
            ButtonClick(caption);
        }
    }
}