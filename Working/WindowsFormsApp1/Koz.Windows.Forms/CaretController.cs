using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Koz.Windows.Forms
{
    public class CaretController : NativeWindowBase<Control>
    {

        public static Color DefaultCaretColor = Color.Black;
        public const int DefaultCaretWidth = 1;

        public CaretController(Control owner) : base(owner) { }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_SETFOCUS:
                    WmSetFocus(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        public Color CaretColor { get; set; } = Color.Empty;
        public int CaretWidth { get; set; } = 1;

        private CaretBitmap caretBitmap;

        private void WmSetFocus(ref Message m) {
            base.WndProc(ref m);
            ShowCaret();
        }

        protected override void OnDisposed(EventArgs e) {
            base.OnDisposed(e);
            if (caretBitmap != null) {
                caretBitmap.Dispose();
                caretBitmap = null;
            }
        }

        public void ShowCaret() {
            NativeMethods.DestroyCaret();
            Size avg = UTL.GetFontSizeAverage(Owner.Font);
            Size caretSize = new Size(CaretWidth, avg.Height);
            if (caretBitmap == null) {
                caretBitmap = new CaretBitmap(CaretColor, caretSize);
            } else if (caretBitmap.Color != CaretColor || caretBitmap.Size != caretSize) {
                caretBitmap.Dispose();
                caretBitmap = new CaretBitmap(CaretColor, caretSize);
            }
            HandleRef hwnd = new HandleRef(this, Owner.Handle);
            NativeMethods.CreateCaret(hwnd, caretBitmap, caretBitmap.Width, caretBitmap.Height);
            NativeMethods.ShowCaret(hwnd);
        }

    }
}
