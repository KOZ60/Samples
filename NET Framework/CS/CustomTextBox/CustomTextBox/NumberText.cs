namespace CustomTextBox
{
    using System;
    using System.ComponentModel;
    using System.Text;

    public class NumberText : RestrictText
    {
        // 符号を許可するかどうか
        [DefaultValue(true)]
        public bool Sign { get; set; } = true;

        // 整数部の長さ
        [DefaultValue(5)]
        public int IntegerLength { get; set; } = 5;

        // 小数部の長さ
        [DefaultValue(3)]
        public int DecimalLength { get; set; } = 3;

        // フォーカスを失ったとき表示する際の書式
        [DefaultValue("#,0.000")]
        public string FormatString { get; set; } = "#,0.000";

        // 値を編集する際に発生するイベント。編集後の文字列をカスタマイズすることができます。
        public event EventHandler<NumberFormatEventArgs> Format;

        protected override bool IsValidChar(char keyChar) {
            switch (keyChar) {
                case '-':
                case '+':
                    // 符号チェック
                    return Sign;
                case '.':
                    // 小数部の長さがゼロ以上なら小数点を許可
                    return DecimalLength > 0;
                default:
                    // 数字のみ入力可
                    return ((keyChar >= '0') && (keyChar <= '9'));
            }
        }

        protected override bool IsValidText(string text) {
            return TryParse(text, out _);
        }

        protected virtual bool TryParse(string text, out decimal result) {
            result = decimal.Zero;
            var signChar = '+';

            // 空ならOK
            if (string.IsNullOrEmpty(text)) {
                return true;
            }

            // 左の符号を除く
            var leftChar = text[0];
            if (leftChar == '+' || leftChar == '-') {
                signChar = leftChar;
                text = text.Substring(1);
            } else {
                // 右の符号を除く
                var rightChar = text[text.Length - 1];
                if (rightChar == '+' || rightChar == '-') {
                    signChar = rightChar;
                    text = text.Substring(0, text.Length - 1);
                }
            }

            // 符号が残っていたらエラー
            if (text.IndexOfAny(new char[] { '+', '-' }) != -1) {
                return false;
            }

            // 小数点で分解
            var args = text.Split('.');

            // 小数点が2つ以上あればエラー
            if (args.Length > 2) {
                return false;
            }

            // 整数部の長さを確認
            if (args[0].Length > IntegerLength) {
                return false;
            }

            // 小数部の長さを確認
            if (args.Length > 1) {
                if (args[1].Length > DecimalLength) {
                    return false;
                }
            }

            // Decimal に変換
            decimal.TryParse(text, out result);

            // 符号を反映
            if (signChar == '-') {
                result *= -1;
            }
            return true;
        }

        protected virtual string GetValidText() {
            var sb = new StringBuilder();
            foreach (var c in base.Text) {
                if (IsValidChar(c)) {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        protected virtual string GetFormatText() {
            return GetFormatText(Value);
        }

        protected virtual string GetFormatText(decimal value) {
            string format;
            string formatedText;
            if (string.IsNullOrEmpty(FormatString)) {
                format = "{0}";
            } else {
                format = "{0:" + FormatString + "}";
            }
            try {
                formatedText = string.Format(format, value);
            } catch {
                formatedText = base.Text;
            }

            var e = new NumberFormatEventArgs(this.Value, this.FormatString, formatedText);
            OnFormat(e);
            return e.FormatedText;
        }

        protected virtual void OnFormat(NumberFormatEventArgs e) {
            Format?.Invoke(this, e);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual decimal Value {
            get {
                TryParse(GetValidText(), out decimal result);
                return result;
            }
            set {
                base.Text = GetFormatText(value);
            }
        }

        protected override void OnEnter(EventArgs e) {
            SetTextWithoutEvents(GetValidText());
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e) {
            SetTextWithoutEvents(GetFormatText());
            base.OnLeave(e);
        }
    }
}
