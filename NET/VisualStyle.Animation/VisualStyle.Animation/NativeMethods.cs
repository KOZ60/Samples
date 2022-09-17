namespace VisualStyle.Animation
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    static class NativeMethods
    {
        class ExternDll
        {
            public const string Kernel32 = "kernel32.dll";
            public const string User32 = "user32.dll";
            public const string Gdi32 = "gdi32.dll";
            public const string Uxtheme = "uxtheme.dll";
        }

        public const int WM_PAINT = 0x000f;
        public const int WM_PRINTCLIENT = 0x0318;
        public const int WM_NCDESTROY = 0x0082;

        [DllImport(ExternDll.Uxtheme)]
        public static extern IntPtr BufferedPaintInit();
        [DllImport(ExternDll.Uxtheme)]
        public static extern IntPtr BufferedPaintUnInit();

        [DllImport(ExternDll.Uxtheme)]
        public static extern IntPtr BeginBufferedAnimation(
            IntPtr hwnd,
            IntPtr hdcTarget,
            ref RECT rcTarget,
            BP_BUFFERFORMAT dwFormat,
            IntPtr pPaintParams,
            ref BP_ANIMATIONPARAMS pAnimationParams,
            out IntPtr phdcFrom,
            out IntPtr phdcTo
        );

        [DllImport(ExternDll.Uxtheme)]
        public static extern int EndBufferedAnimation(IntPtr hbpAnimation, bool fUpdateTarget);

        [DllImport(ExternDll.Uxtheme)]
        public static extern bool BufferedPaintRenderAnimation(IntPtr hwnd, IntPtr hdcTarget);

        [DllImport(ExternDll.Uxtheme)]
        public static extern IntPtr BufferedPaintStopAllAnimations(IntPtr hwnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;

            public RECT(Rectangle r) {
                left = r.Left;
                top = r.Top;
                right = r.Right;
                bottom = r.Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BP_ANIMATIONPARAMS
        {
            public int cbSize;
            public int dwFlags; // Reserved
            public BP_ANIMATIONSTYLE style;
            public int dwDuration;
        }

        [Flags]
        public enum BP_ANIMATIONSTYLE
        {
            None = 0,
            Linear = 1,
            Cubic = 2,
            Sine = 3
        }

        public enum BP_BUFFERFORMAT
        {
            CompatibleBitmap,
            DIB,
            TopDownDIB,
            TopDownMonoDIB
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public int rcPaint_left;
            public int rcPaint_top;
            public int rcPaint_right;
            public int rcPaint_bottom;
            public bool fRestore;
            public bool fIncUpdate;
            public int reserved1;
            public int reserved2;
            public int reserved3;
            public int reserved4;
            public int reserved5;
            public int reserved6;
            public int reserved7;
            public int reserved8;
        }

        [DllImport(ExternDll.User32)]
        public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32)]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SelectPalette(IntPtr hdc, IntPtr hpal, int bForceBackground);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int RealizePalette(IntPtr hDC);

        public static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette) {
            IntPtr halftonePalette = Graphics.GetHalftonePalette();
            IntPtr result = SelectPalette(dc, halftonePalette, (force ? 0 : 1));
            if (result != IntPtr.Zero && realizePalette) {
                RealizePalette(dc);
            }
            return result;
        }

        [DllImport(ExternDll.User32)]
        public static extern IntPtr WindowFromPoint(Point p);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenThemeData(IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPWStr)] string pszClassList);

        [DllImport(ExternDll.Uxtheme)]
        public static extern int CloseThemeData(IntPtr hTheme);

        [DllImport(ExternDll.Uxtheme)]
        public static extern int GetThemeTransitionDuration(
            IntPtr hTheme, int iPartId, int iStateIdFrom,
            int iStateIdTo, int iPropId, out int pdwDuration);

        public const int TMT_TRANSITIONDURATIONS = 6000;
    }
}
