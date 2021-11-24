using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CustomDrawApp
{
    class TextBoxEx : TextBox
    {
        readonly CustomDrawWindow cdw;

        public TextBoxEx() {
            cdw = new CustomDrawWindow(this);
            cdw.CustomDraw += CustomDraw;
        }

        private void CustomDraw(object sender, PaintEventArgs e) {
            e.Graphics.DrawImage(cdw.NativeClientBitmap, 0, 0);
        }

        Color _CaretColor = Color.Empty;

        public Color CaretColor {
            get {
                if (_CaretColor.IsEmpty) {
                    return Color.Black;
                }
                return _CaretColor;
            }
            set {
                _CaretColor = value;
                ShowCaretIfChanged();
            }
        }

        public void ResetCaretColor() {
            _CaretColor = Color.Empty;
        }

        internal bool ShouldSerializeCaretColor() {
            return !_CaretColor.IsEmpty;
        }

        public override Font Font {
            get { return base.Font; }
            set {
                base.Font = value;
                ShowCaretIfChanged();
            }
        }

        int _CaretWidth = 1;

        [DefaultValue(1)]
        public int CaretWidth {
            get {
                return _CaretWidth;
            }
            set {
                if (_CaretWidth != value) {
                    _CaretWidth = value;
                    ShowCaretIfChanged();
                }
            }
        }

        CustomCaret customCaret;
        private Size CaretSize {
            get {
                return new Size(CaretWidth, Font.Height);
            }
        }

        private void ShowCaretIfChanged() {
            bool changed = false;
            if (customCaret == null) {
                customCaret = new CustomCaret(CaretColor, CaretWidth, Font.Height);
                changed = true;
            } else if (customCaret.Color != CaretColor || customCaret.Size != CaretSize) {
                customCaret.Dispose();
                customCaret = new CustomCaret(CaretColor, CaretWidth, Font.Height);
                changed = true;
            }
            if (Focused && changed) {
                customCaret.Show(this);
            }
        }

        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            customCaret.Show(this);
        }
    }
}
