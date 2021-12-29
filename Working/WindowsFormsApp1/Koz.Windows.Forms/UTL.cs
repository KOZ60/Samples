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

        public static Rectangle DeflateRect(Rectangle rect, Padding padding) {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
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
    }
}
