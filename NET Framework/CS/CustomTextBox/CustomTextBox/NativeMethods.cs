using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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
            WM_SETTEXT = 0x000C,
            WM_PAINT = 0x000F,
            WM_ERASEBKGND = 0x0014,
            WM_CHAR = 0x0102,
            WM_IME_CHAR = 0x0286;

        public const int
            WM_CUT = 0x0300,
            WM_COPY = 0x0301,
            WM_PASTE = 0x0302,
            WM_CLEAR = 0x0303,
            WM_UNDO = 0x0304;

        public const int
            EM_REPLACESEL = 0x00C2,
            EM_GETRECT = 0x00B2,
            EM_SETRECT = 0x00B3,
            EM_UNDO = 0x00C7;

        public const int
            ES_MULTILINE = 0x0004;

        public const char BACKSPACE = '\b';      // Backspace キー
        public const char CTRL_A = '\x01';       // CTRL+A（全選択）
        public const char CTRL_C = '\x03';       // CTRL+C（コピー）
        public const char CTRL_V = '\x16';       // CTRL+V（貼り付け）
        public const char CTRL_X = '\x18';       // CTRL+X（切り取り）
        public const char CTRL_Z = '\x1A';       // CTRL+Z（元に戻す）

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

        [DllImport(ExternDll.User32)]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom) {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rectangle r) {
                Left = r.Left;
                Top = r.Top;
                Right = r.Right;
                Bottom = r.Bottom;
            }

            public override string ToString() {
                return string.Format("RECT({0},{1},{2},{3})", Left, Top, Right, Bottom);
            }

            public int Width {
                get {
                    return Right - Left;
                }
            }

            public int Height {
                get {
                    return Bottom - Top;
                }
            }

            public Rectangle Rectangle {
                get {
                    return Rectangle.FromLTRB(Left, Top, Right, Bottom);
                }
            }
        }
    }
}
