using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CustomDrawApp
{
    internal class CustomCaret : SafeHandleMinusOneIsInvalid
    {
        private readonly IntPtr BLACK_HANDLE = IntPtr.Zero;

        public CustomCaret(Color color, Size size) : base(true) {
            Color = color;
            Size = size;

            if (color == Color.Black) {
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

        [DllImport("User32")]
        private static extern bool CreateCaret(
                IntPtr hwnd, IntPtr hbitmap, int width, int height);

        [DllImport("User32")]
        private static extern bool ShowCaret(IntPtr hwnd);

        [DllImport("User32")]
        private static extern bool DestroyCaret();

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public void Show(Control control) {
            DestroyCaret();
            CreateCaret(control.Handle, handle, Width, Height);
            ShowCaret(control.Handle);
        }

        protected override bool ReleaseHandle() {
            bool ret = true;
            if (handle != BLACK_HANDLE) {
                ret = DeleteObject(handle);
            }
            SetHandle(new IntPtr(-1));
            return ret;
        }
    }
}
