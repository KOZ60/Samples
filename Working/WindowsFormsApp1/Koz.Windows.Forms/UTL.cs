using System;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public static class UTL {

        // フォントサイズの平均値を取得
        public static Size GetFontAverageSize(Font font) {
            using (var wrapper = new GraphicsWrapper(font)) {
                return wrapper.GetFontAverageSize();
            }
        }

        public static Rectangle DeflateRect(Rectangle rect, Padding padding) {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
        }
 
        public static bool IsDesignMode {
            get {
                return Assembly.GetEntryAssembly() == null;
            }
        }

        [ThreadStatic]
        private static int lockCount = 0;

        public static int LockWindow(IntPtr handle) {
            if (lockCount == 0) {
                NativeMethods.LockWindowUpdate(new HandleRef(null, handle));
            }
            lockCount += 1;
            return lockCount;
        }

        public static int UnlockWindow() {
            lockCount -= 1;
            if (lockCount <= 0) {
                NativeMethods.LockWindowUpdate(new HandleRef(null, IntPtr.Zero));
                lockCount = 0;
            }
            return lockCount;
        }

        public static int MAKELONG(int low, int high) {
            return (high << 16) | (low & 0xffff);
        }

        public static IntPtr MAKELPARAM(int low, int high) {
            return (IntPtr)((high << 16) | (low & 0xffff));
        }

        public static int HIWORD(int n) {
            return (n >> 16) & 0xffff;
        }

        public static int HIWORD(IntPtr n) {
            return HIWORD(unchecked((int)(long)n));
        }

        public static int LOWORD(int n) {
            return n & 0xffff;
        }

        public static int LOWORD(IntPtr n) {
            return LOWORD(unchecked((int)(long)n));
        }

        public static int SignedHIWORD(IntPtr n) {
            return SignedHIWORD(unchecked((int)(long)n));
        }
        public static int SignedLOWORD(IntPtr n) {
            return SignedLOWORD(unchecked((int)(long)n));
        }

        public static int SignedHIWORD(int n) {
            int i = (int)(short)((n >> 16) & 0xffff);

            return i;
        }

        public static int SignedLOWORD(int n) {
            int i = (int)(short)(n & 0xFFFF);

            return i;
        }

        public static Point ToPoint(IntPtr lparam) {
            int x = SignedLOWORD(lparam);
            int y = SignedHIWORD(lparam);
            return new Point(x, y);
        }

        public static Point InvalidPoint = new Point(int.MinValue, int.MinValue);

        public static bool IsInvalid(this Point pt) {
            return pt == InvalidPoint;
        }

    }
}
