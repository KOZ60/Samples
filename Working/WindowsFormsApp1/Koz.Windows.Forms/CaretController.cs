using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class CaretController : NativeWindowBase<Control>
    {
        public static Color DefaultCaretColor = Color.Black;
        public const int DefaultCaretWidth = 1;

        public CaretController(TextEditor owner) : base(owner) { }

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

        protected bool IsDefault {
            get {
                return CaretColor.IsEmpty && CaretWidth == DefaultCaretWidth;
            }
        }

        public void ShowCaret() {

            if (UTL.IsDesignMode) return;
            if (IsDefault) return;

            Size avg = UTL.GetFontAverageSize(Owner.Font);
            Size caretSize = new Size(CaretWidth, avg.Height);
            if (caretBitmap == null) {
                caretBitmap = new CaretBitmap(CaretColor, caretSize);
            } else if (caretBitmap.Color != CaretColor || caretBitmap.Size != caretSize) {
                caretBitmap.Dispose();
                caretBitmap = new CaretBitmap(CaretColor, caretSize);
            }
            HandleRef hwnd = new HandleRef(this, Owner.Handle);
            NativeMethods.DestroyCaret();
            NativeMethods.CreateCaret(hwnd, caretBitmap, caretBitmap.Width, caretBitmap.Height);
            NativeMethods.ShowCaret(hwnd);
        }

        public class CaretBitmap : SafeHandleMinusOneIsInvalid
        {
            private readonly IntPtr BLACK_HANDLE = IntPtr.Zero;

            public CaretBitmap(Color color, Size size) : base(true) {
                Color = color;
                Size = size;

                if (color.IsEmpty) {
                    SetHandle(BLACK_HANDLE);
                } else {
                    using (var bmp = new Bitmap(Width, Height)) {
                        using (var g = Graphics.FromImage(bmp)) {
                            g.Clear(CreateCaretColor(color));
                        }
                        SetHandle(bmp.GetHbitmap());
                    }
                }
            }

            private static Color CreateCaretColor(Color color) {
                int A = color.A;
                int R = color.R ^ 0xff;
                int G = color.G ^ 0xff;
                int B = color.B ^ 0xff;
                return Color.FromArgb(A, R, G, B);
            }

            public Color Color { get; }

            public Size Size { get; }

            public int Width {
                get {
                    return Size.Width;
                }
            }

            public int Height {
                get {
                    return Size.Height;
                }
            }

            protected override bool ReleaseHandle() {
                bool ret = true;
                if (handle != BLACK_HANDLE) {
                    ret = NativeMethods.DeleteObject(new HandleRef(this, handle));
                }
                SetHandle(NativeMethods.INVALID_HANDLE_VALUE);
                return ret;
            }
        }
    }
}
