using System.Drawing;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public static class LayoutUtils {

        public static Rectangle DeflateRect(Rectangle rect, Padding padding) {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
        }

    }
}
