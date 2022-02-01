using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CustomTextBox
{
    static class NativeMethods
    {
        private static class ExternDll
        {
            public const string Kernel32 = "kernel32.dll";
            public const string User32 = "user32.dll";
            public const string Gdi32 = "gdi32.dll";
        }

        public const int
            WM_CHAR         = 0x0102,
            WM_SETTEXT      = 0x000C,
            WM_IME_CHAR     = 0x0286,
            WM_PASTE        = 0x0302,
            EM_REPLACESEL   = 0x00C2,
            EM_SETRECT      = 0x00B3;

        public const int
            ES_MULTILINE = 0x0004;

        [StructLayout(LayoutKind.Sequential)]
        public struct TEXTMETRICW
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public ushort tmFirstChar;
            public ushort tmLastChar;
            public ushort tmDefaultChar;
            public ushort tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }

        [DllImport(ExternDll.Gdi32)]
        public static extern bool GetTextMetricsW(IntPtr hdc, out TEXTMETRICW lptm);

        [DllImport(ExternDll.Gdi32)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport(ExternDll.Gdi32)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport(ExternDll.Gdi32)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport(ExternDll.Gdi32)]
        public static extern bool DeleteObject(IntPtr hObject);

        public static Size GetFontAverageSize(Font font) {
            IntPtr hFont = font.ToHfont();
            IntPtr hdc = CreateCompatibleDC(IntPtr.Zero);
            IntPtr oldFont = SelectObject(hdc, hFont);
            try {
                GetTextMetricsW(hdc, out TEXTMETRICW tm);
                return new Size(tm.tmAveCharWidth, tm.tmHeight);
            } finally {
                SelectObject(hdc, oldFont);
                DeleteObject(hFont);
                DeleteDC(hdc);
            }
        }

        [DllImport(ExternDll.Kernel32)]
        public static extern int lstrlenW(IntPtr lpString);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref RECT lParam);

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(Rectangle r) {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }

            public override string ToString() {
                return string.Format("RECT({0},{1},{2},{3})", left, top, right, bottom);
            }

            public int Width {
                get {
                    return this.right - this.left;
                }
            }

            public int Height {
                get {
                    return this.bottom - this.top;
                }
            }

            public Rectangle Rectangle {
                get {
                    return Rectangle.FromLTRB(left, top, right, bottom);
                }
            }
        }
    }
}
