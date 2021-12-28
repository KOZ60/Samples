using System;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class GraphicsWrapper : IDisposable
    {
        private Graphics graphics;
        private HandleRef hwnd;
        private HandleRef hdc;
        private HandleRef hFont;
        private HandleRef oldFont;

        public GraphicsWrapper(Graphics graphics, Font font) {
            this.graphics = graphics;
            Initialize(graphics.GetHdc(), font);
        }

        public GraphicsWrapper(IntPtr hwnd, Font font) {
            this.graphics = null;
            this.hwnd = new HandleRef(this, hwnd);
            Initialize(NativeMethods.GetDC(this.hwnd), font);
        }

        private void Initialize(IntPtr dc, Font font) {
            this.hdc = new HandleRef(this, dc);
            this.hFont = new HandleRef(this, font.ToHfont());
            this.oldFont = new HandleRef(this, NativeMethods.SelectObject(hdc, hFont));
        }

        public void SetFont(Font font) {
            hFont = new HandleRef(this, font.ToHfont());
            HandleRef prevFont = new HandleRef(this, NativeMethods.SelectObject(hdc, hFont));
            NativeMethods.DeleteObject(prevFont);
        }

        public void SetColor(Color foreColor, Color backColor) {
            int Win32ForeColor = ColorTranslator.ToWin32(foreColor);
            int Win32BackColor = ColorTranslator.ToWin32(backColor);
            SetColor(Win32ForeColor, Win32BackColor);
        }

        public void SetColor(int foreColor, int backColor) {
            NativeMethods.SetTextColor(hdc, foreColor);
            NativeMethods.SetBkColor(hdc, backColor);
        }

        public void TextOut(int x, int y, ref char c) {
            NativeMethods.TextOut(hdc, x, y, ref c, 1);
        }

        public void TextOut(int x, int y, string str) {
            NativeMethods.TextOut(hdc, x, y, str, str.Length);
        }

        public void TextOut(int x, int y, StringBuilder str) {
            NativeMethods.TextOut(hdc, x, y, str, str.Length);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing) {
            if (disposedValue) return;
            disposedValue = true;
            NativeMethods.SelectObject(hdc, oldFont);
            NativeMethods.DeleteObject(hFont);
            if (graphics != null) {
                graphics.ReleaseHdc();
            } else {
                NativeMethods.ReleaseDC(hwnd, hdc);
                NativeMethods.DeleteDC(hdc);
            }
        }

        ~GraphicsWrapper() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
