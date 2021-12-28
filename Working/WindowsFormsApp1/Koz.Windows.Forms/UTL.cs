using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public static class UTL {

        // フォントサイズの平均値を取得
        public static Size GetFontSizeAverage(Font font) {
            HandleRef hdc = new HandleRef(null, NativeMethods.GetDC(new HandleRef(null, IntPtr.Zero)));
            HandleRef hFont = new HandleRef(null, font.ToHfont());
            HandleRef oldFont = new HandleRef(null, NativeMethods.SelectObject(hdc, hFont));
            Size result = GetFontSizeAverageFromHdc(hdc);
            NativeMethods.SelectObject(hdc, oldFont);
            NativeMethods.DeleteObject(hFont);
            NativeMethods.DeleteDC(hFont);
            return result;
        }

        // フォントが選択済みのデバイスコンテキストハンドルよりフォントサイズの平均値を取得
        public static Size GetFontSizeAverageFromHdc(HandleRef hdc) {
            NativeMethods.GetTextMetricsW(hdc, out NativeMethods.TEXTMETRICW tmNative);
            return new Size(tmNative.tmAveCharWidth, tmNative.tmHeight);
        }

        public static Rectangle DeflateRect(Rectangle rect, Padding padding) {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
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
