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
        private readonly IntPtr GRAY_HANDLE = new IntPtr(1);

        public CustomCaret(Color color, int width, int fontHeight) : base(true) {
            Color = color;
            Size = new Size(width, fontHeight);

            if (color == Color.Black) {
                SetHandle(BLACK_HANDLE);
            } else if (color == Color.Gray) {
                SetHandle(GRAY_HANDLE);
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
            if (handle != BLACK_HANDLE && handle != GRAY_HANDLE) {
                return DeleteObject(handle);
            }
            return true;
        }

        //public static bool operator ==(CustomCaret left, CustomCaret right) {
        //    return (left.Color == right.Color) &&
        //           (left.Size == right.Size);
        //}

        //public static bool operator !=(CustomCaret left, CustomCaret right) {
        //    return !(left == right);
        //}

        //public override bool Equals(object obj) {
        //    CustomCaret other = obj as CustomCaret;
        //    if (other != null) {
        //        return this == other;
        //    }
        //    return false;
        //}

        //public override int GetHashCode() {
        //    return Color.GetHashCode() << 8 | Size.GetHashCode();
        //}
    }
}
