using System.ComponentModel;

namespace Calculator
{
    public partial class CalcForm : Form
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
#if DEBUG
                Text = string.Format("電卓({0})", value);
#endif
            }
        }

        private char Operator = char.MinValue;
        private string Number1 = string.Empty;

        public CalcForm() {
            InitializeComponent();
            foreach (var button in Controls.OfType<CalcButton>()) {
                button.Click += Button_Click;
            }
            label1.Text = "0";
            label1.Paint += Label1_Paint;
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
            } catch (Exception) {
                IsError = true;
                MyStatus = Status.S5;
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
                    break;

                case Status.S5:
                    break;
            }
        }

        private void PercentCalcuration() {
            decimal tmp;

            switch (Operator) {
                case '/':
                case '*':
                    // 除算/乗算のときは数値２を1/100倍して計算
                    tmp = Convert.ToDecimal(label1.Text) / 100;
                    label1.Text = tmp.ToString();
                    Calcuration();
                    break;
                case '+':
                case '-':
                    // 加算/減算のときは数値１の数値２％を求めて数値２として計算
                    tmp = Convert.ToDecimal(Number1) / 100 * Convert.ToDecimal(label1.Text);
                    label1.Text = tmp.ToString();
                    Calcuration();
                    break;
            }
        }


        private void InvertButtonClick() {
            switch (MyStatus) {
                case Status.S5:
                    break;
                default:
                    decimal d = Convert.ToDecimal(label1.Text);
                    label1.Text = (0 - d).ToString();
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
            IsError = false;
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
            label1.Value = result;
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

        private bool _IsError = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsError {
            get {
                return _IsError;
            }
            set {
                if (IsError != value) {
                    _IsError = value;
                    label1.ForeColor = value ? Color.Red : SystemColors.ControlText;
                    label1.Invalidate();
                }
            }
        }

        private void Label1_Paint(object? sender, PaintEventArgs e) {
            if (IsError) {
                using (var brush = new SolidBrush(label1.ForeColor))
                using (var fnt = new Font("ＭＳ ゴシック", 9F, FontStyle.Bold)) {
                    e.Graphics.DrawString("E", fnt, brush, 10, 25);
                }
            }
        }
    }
}