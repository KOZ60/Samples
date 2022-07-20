namespace Calculator
{
    public partial class Form1 : Form
    {
        private enum Status
        {
            /// <summary>数値１入力中</summary>
            S1,
            /// <summary>演算子入力後、入力待ち</summary>
            S2,
            /// <summary>数値２入力中</summary>
            S3,
            /// <summary>演算実行後、入力待ち</summary>
            S4,
            /// <summary>エラー発生</summary>
            S5
        }

        private Status _MyStatus = Status.S1;
        private Status MyStatus { 
            get {
                return _MyStatus;
            }
            set {
                _MyStatus = value;
                Text = String.Format("電卓({0})", value); // デバッグ用
            }
        }
        
        private char Operator = char.MinValue;
        private string Number = string.Empty;

        public Form1() {
            InitializeComponent();
            foreach (var button in Controls.OfType<NoFocusButton>()) {
                button.Click += Button_Click;
            }
            label1.Text = "0";
            MyStatus = Status.S1;
        }

        private void Button_Click(object? sender, EventArgs e) {
            var button = sender as NoFocusButton;
            if (button == null) return;
            ButtonClick(button.CharValue);
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
            Number = string.Empty;
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
            Number = label1.Text;
        }

        private void Calcuration() {
            decimal num1 = Convert.ToDecimal(Number);
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