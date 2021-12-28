using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Koz.Windows.Forms
{
    public class FontHandleWrapper : SafeHandle
    {
        FontFamily FontFamily;
        bool GdiVerticalFont;
        byte GdiCharSet;
        FontStyle Style;
        float Size;
        GraphicsUnit Unit;
        Size averageSize;

        public FontHandleWrapper(Font font) : base(IntPtr.Zero, true) {
            handle = font.ToHfont();
            FontFamily = font.FontFamily;
            GdiVerticalFont = font.GdiVerticalFont;
            GdiCharSet = font.GdiCharSet;
            Style = font.Style;
            Size = font.Size;
            Unit = font.Unit;
            using (var wrapper = new GraphicsWrapper(handle, false)) {
                averageSize = wrapper.GetFontAverageSize();
            }
        }

        public Size AverageSize {
            get {
                return averageSize;
            }
        }

        public IntPtr Handle {
            get {
                return handle;
            }
        }

        public bool IsEquals(Font font) {
            return font.FontFamily.Equals(this.FontFamily) &&
               font.GdiVerticalFont == this.GdiVerticalFont &&
               font.GdiCharSet == this.GdiCharSet &&
               font.Style == this.Style &&
               font.Size == this.Size &&
               font.Unit == this.Unit;
        }

        protected override bool ReleaseHandle() {
            NativeMethods.DeleteObject(new HandleRef(this, handle));
            handle = IntPtr.Zero;
            return true;
        }

        public override bool IsInvalid {
            get {
                return handle == IntPtr.Zero;
            }
        }
    }
}
