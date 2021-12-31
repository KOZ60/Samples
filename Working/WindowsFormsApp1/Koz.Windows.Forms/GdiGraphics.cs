using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Koz.Windows.Forms
{
    public class GdiGraphics : IDisposable
    {
        private HandleRef hdc;
        private HandleRef fontHandle;
        private readonly bool deleteHdc;
        private bool deleteFont;
        private HandleRef oldFont;

        public GdiGraphics(Font font) : this(font.ToHfont(), true) { }

        public GdiGraphics(IntPtr fontHandle, bool deleteFont) 
            : this(IntPtr.Zero, true, fontHandle, deleteFont) { }

        public GdiGraphics(IntPtr hdc, Font font)
            : this(hdc, false, font.ToHfont(), true) { }

        public GdiGraphics(IntPtr hdc, bool deleteHdc, IntPtr fontHandle, bool deleteFont) {
            if (hdc == IntPtr.Zero) {
                hdc = NativeMethods.CreateCompatibleDC(new HandleRef(this, IntPtr.Zero));
                deleteHdc = true;
            }
            this.hdc = new HandleRef(this, hdc);
            this.deleteHdc = deleteHdc;
            this.fontHandle = new HandleRef(this, fontHandle);
            this.deleteFont = deleteFont;
            this.oldFont = new HandleRef(this, NativeMethods.SelectObject(this.hdc, this.fontHandle));
        }

        public void ChangeFont(Font font) {
            fontHandle = new HandleRef(this, font.ToHfont());
            HandleRef prevFont = new HandleRef(this, NativeMethods.SelectObject(hdc, fontHandle));
            if (deleteFont) {
                NativeMethods.DeleteObject(prevFont);
            }
            deleteFont = true;
        }

        public IntPtr GetHdc() {
            return hdc.Handle;
        }

        public Size GetFontAverageSize() {
            NativeMethods.GetTextMetricsW(hdc, out NativeMethods.TEXTMETRICW tmNative);
            return new Size(tmNative.tmAveCharWidth, tmNative.tmHeight);
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

        public void SetBkMode(BkModeConstans bkMode) {
            NativeMethods.SetBkMode(hdc, bkMode);
        }

        private bool disposedValue;

        private void DisposeCaller(bool disposing) {
            if (disposedValue) return;
            disposedValue = true;
            Dispose(disposing);
        }

        protected virtual void Dispose(bool disposing) {
            NativeMethods.SelectObject(hdc, oldFont);
            if (deleteFont) {
                NativeMethods.DeleteObject(fontHandle);
            }
            if (deleteHdc) {
                NativeMethods.DeleteDC(hdc);
            }
        }

        ~GdiGraphics() {
            DisposeCaller(false);
        }

        public void Dispose() {
            DisposeCaller(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
