using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Koz.Windows.Forms
{
    public class GraphicsWrapper : IDisposable
    {
        private HandleRef hdc;
        private HandleRef fontHandle;
        private bool needDeleteHdc;
        private bool needDeleteFont;
        private HandleRef oldFont;

        public GraphicsWrapper(Font font) : this(font.ToHfont(), true) { }

        public GraphicsWrapper(IntPtr fontHandle, bool needDeleteFont) {
            IntPtr hdc = NativeMethods.CreateCompatibleDC(new HandleRef(this, IntPtr.Zero));
            Initialize(hdc, true, fontHandle, needDeleteFont);
        }

        public GraphicsWrapper(IntPtr hdc, Font font)  {
            Initialize(hdc, false, font.ToHfont(), true);
        }

        public GraphicsWrapper(IntPtr hdc, bool needDeleteHdc, IntPtr fontHandle, bool needDeleteFont) {
            Initialize(hdc, needDeleteHdc, fontHandle, needDeleteFont);
        }

        private void Initialize(IntPtr hdc, bool needDeleteHdc, IntPtr fontHandle, bool needDeleteFont) {
            this.hdc = new HandleRef(this, hdc);
            this.needDeleteHdc = needDeleteHdc;
            this.fontHandle = new HandleRef(this, fontHandle);
            this.needDeleteFont = needDeleteFont;
            this.oldFont = new HandleRef(this, NativeMethods.SelectObject(this.hdc, this.fontHandle));
        }

        public void ChangeFont(Font font) {
            fontHandle = new HandleRef(this, font.ToHfont());
            HandleRef prevFont = new HandleRef(this, NativeMethods.SelectObject(hdc, fontHandle));
            if (needDeleteFont) {
                NativeMethods.DeleteObject(prevFont);
            }
            needDeleteFont = true;
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

        private bool disposedValue;

        protected virtual void Dispose(bool disposing) {
            if (disposedValue) return;
            disposedValue = true;
            NativeMethods.SelectObject(hdc, oldFont);
            if (needDeleteFont) {
                NativeMethods.DeleteObject(fontHandle);
            }
            if (needDeleteHdc) {
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
