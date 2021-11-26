using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CustomDrawApp
{
    public class TextBoxEx : TextBox {

        const string CaretCategory = "キャレットの形状";

        readonly CustomDrawWindow cdw;

        public TextBoxEx() {
            cdw = new CustomDrawWindow(this);
            cdw.CustomDraw += CustomDraw;
        }

        private void CustomDraw(object sender, PaintEventArgs e) {
            e.Graphics.DrawImage(cdw.NativeClientBitmap, 0, 0);
        }

        #region CaretColor プロパティ

        // -------------------------------------------------------------------------------
        // CaretColor プロパティ
        // -------------------------------------------------------------------------------
        private Color _CaretColor = Color.Empty;

        /// <summary>
        /// キャレットの色を取得または設定します。
        /// </summary>
        [Category(CaretCategory)]
        [Description("キャレットの色を取得または設定します。")]
        public Color CaretColor {
            get {
                if (!ShouldSerializeCaretColor()) {
                    return Color.Black;
                }
                return _CaretColor;
            }
            set {
                if (CaretColor != value) {
                    _CaretColor = value;
                    SetupCaret();
                    OnCaretColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// CaretColorChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnCaretColorChanged(EventArgs e) {
            if (CaretColorChanged != null) CaretColorChanged(this, e);
        }

        /// <summary>
        /// CaretColor プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler CaretColorChanged;

        /// <summary>
        /// CaretColor の値を既定値に設定します。
        /// </summary>
        internal void ResetCaretColor() {
            CaretColor = Color.Empty;
        }

        /// <summary>
        /// CaretColor の値が変更されているかを取得します。
        /// </summary>
        internal bool ShouldSerializeCaretColor() {
            return _CaretColor != Color.Empty;
        }

        #endregion

        #region CaretWidth プロパティ

        // -------------------------------------------------------------------------------
        // CaretWidth プロパティ
        // -------------------------------------------------------------------------------
        private const int DefaultCaretWidth = 1;
        private int _CaretWidth = DefaultCaretWidth;

        /// <summary>
        /// キャレットの幅を取得または設定します。
        /// </summary>
        [Category(CaretCategory)]
        [Description("キャレットの幅を取得または設定します。")]
        [DefaultValue(DefaultCaretWidth)]
        public int CaretWidth {
            get {
                return _CaretWidth;
            }
            set {
                if (CaretWidth != value) {
                    _CaretWidth = value;
                    SetupCaret();
                    OnCaretWidthChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// CaretWidthChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnCaretWidthChanged(EventArgs e) {
            if (CaretWidthChanged != null) CaretWidthChanged(this, e);
        }

        /// <summary>
        /// CaretWidth プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler CaretWidthChanged;

        #endregion

        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            SetupCaret();
        }

        CustomCaret _CustomCaret;

        private CustomCaret CustomCaret {
            get {
                Size caretSize = new Size(CaretWidth, Font.Height);
                if (_CustomCaret == null) {
                    _CustomCaret = new CustomCaret(CaretColor, caretSize);
                } else if (_CustomCaret.Color != CaretColor || _CustomCaret.Size != caretSize) {
                    _CustomCaret.Dispose();
                    _CustomCaret = new CustomCaret(CaretColor, caretSize);
                }
                return _CustomCaret;
            }
        }

        private void SetupCaret() {
            if (IsHandleCreated && Focused) {
                CustomCaret.Show(this);
            }
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            CustomCaret.Show(this);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (_CustomCaret != null) {
                _CustomCaret.Dispose();
                _CustomCaret = null;
            }
        }
    }
}
