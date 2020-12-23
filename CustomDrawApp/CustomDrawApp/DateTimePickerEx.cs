using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CustomDrawApp
{
    class DateTimePickerEx : DateTimePicker
    {
        readonly CustomDrawWindow cdw;

        public DateTimePickerEx() : base()
        {
            cdw = new CustomDrawWindow(this);
            cdw.CustomDraw += CustomDraw;
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override Color ForeColor { get => base.ForeColor; set => base.ForeColor = value; }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override Color BackColor { get => base.BackColor; set => base.BackColor = value; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool RightToLeftLayout { get ; set; }

        private void CustomDraw(object sender, PaintEventArgs e)
        {
            var bmpOrg = cdw.NativeClientBitmap;
            var g = e.Graphics;
            g.DrawImage(bmpOrg, Point.Empty);

            var bsz = SystemInformation.Border3DSize;
            var clip = Rectangle.Inflate(this.ClientRectangle, -bsz.Width, -bsz.Height);

            IntPtr hdc = g.GetHdc();
            try
            {
                using (var bmpBack = (Bitmap)bmpOrg.Clone())
                {
                    BitBlt(bmpBack, BackColor, SRCAND);
                    BitBlt(hdc, bmpBack, clip, SRCCOPY);
                }
                using (var bmpFore = CreateNegativeBitmap(bmpOrg))
                {
                    BitBlt(bmpFore, ForeColor, SRCAND);
                    BitBlt(hdc, bmpFore, clip, SRCPAINT);
                }

                // レイアウト（チェックボックスとボタンの位置）を取得
                var dti = new DATETIMEPICKERINFO();
                SendMessage(this.Handle, DTM_GETDATETIMEPICKERINFO, IntPtr.Zero, dti);
                Rectangle checkBox = dti.rcCheck.Rectangle;
                Rectangle button = dti.rcButton.Rectangle;

                // チェックボックスを描画
                if (!checkBox.IsEmpty)
                {
                    BitBlt(hdc, bmpOrg, checkBox, SRCCOPY);
                }

                // アップダウンコントロールがある場合はその領域をボタンとする
                if (dti.hwndUD != IntPtr.Zero)
                {
                    GetWindowRect(dti.hwndUD, out RECT rc);
                    button = RectangleToClient(rc.Rectangle);
                }

                // ボタンを描画
                if (!button.IsEmpty)
                {
                    BitBlt(hdc, bmpOrg, button, SRCCOPY);
                }

                // フォーカスがあるとき、キャレットの部分のみ転送
                if (this.ContainsFocus)
                {
                    var left = clip.Left;
                    var right = clip.Right;
                    if (!checkBox.IsEmpty)
                    {
                        left = checkBox.Right;
                    }
                    if (!button.IsEmpty)
                    {
                        right = button.Left;
                    }
                    var canvas = Rectangle.FromLTRB(left, clip.Top, right, clip.Bottom);
                    Rectangle caret = GetCaretRectangle(bmpOrg, canvas);
                    if (!caret.IsEmpty)
                    {
                        BitBlt(hdc, bmpOrg, caret, SRCCOPY);
                    }
                }
            }
            finally
            {
                g.ReleaseHdc();
            }
        }

        private static Bitmap CreateNegativeBitmap(Bitmap bmp)
        {
            var bmpDest = new Bitmap(bmp.Width, bmp.Height);
            using (var g = Graphics.FromImage(bmpDest))
            {
                var hdc = g.GetHdc();
                BitBlt(hdc, bmp, NOTSRCCOPY);
                g.ReleaseHdc();
            }
            return bmpDest;
        }

        private unsafe static Rectangle GetCaretRectangle(Bitmap bmp, Rectangle canvas)
        {
            int pixelSize = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, bmp.PixelFormat);
            try
            {
                var rangeStartX = -1;
                var rangeEndX = -1;
                var rangeStartY = canvas.Top + 2;   // Yの開始座標を、とりあえず4ピクセル目とする
                var rangeEndY = bmpData.Height - 4; // Yの終了座標は、どの環境でも全体レイアウトの高さ-4

                byte* pixels = (byte*)bmpData.Scan0;
                var baseColor = SystemColors.Highlight;
                var hlBytes = new byte[] { baseColor.B, baseColor.G, baseColor.R };
                fixed (byte* highlightColor = &hlBytes[0])
                {
                    // X方向へ選択領域座標を取得
                    for (int x = canvas.Left; x < canvas.Right; x++)
                    {
                        int pos = rangeStartY * bmpData.Stride + x * pixelSize;
                        if (ColorCompare(&pixels[pos], highlightColor))
                        {
                            if (rangeStartX == -1)
                            {
                                rangeStartX = x;
                            }
                            else
                            {
                                rangeEndX = x;
                            }
                        }
                    }
                    // 見つからなかったら空の Rectangle を返す
                    if (rangeStartX == -1)
                    {
                        return Rectangle.Empty;
                    }
                    // Y開始座標が3ピクセル目の場合があるので、開始座標を求め直す
                    for (int y = rangeStartY; y >= canvas.Top; y--)
                    {
                        int pos = y * bmpData.Stride + rangeStartX * pixelSize;
                        if (ColorCompare(&pixels[pos], highlightColor))
                        {
                            rangeStartY = y;
                        }
                    }
                    return Rectangle.FromLTRB(rangeStartX, rangeStartY, rangeEndX + 1, rangeEndY);
                }
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
        }

        private unsafe static bool ColorCompare(byte* A, byte* B)
        {
            if (A[0] != B[0]) return false;
            if (A[1] != B[1]) return false;
            if (A[2] != B[2]) return false;
            return true;
        }

        #region WINAPI

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rectangle Rectangle {
                get {
                    return Rectangle.FromLTRB(Left, Top, Right, Bottom);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private class DATETIMEPICKERINFO
        {
            public int cbSize;
            public RECT rcCheck;
            public int stateCheck;
            public RECT rcButton;
            public int stateButton;
            public IntPtr hwndEdit;
            public IntPtr hwndUD;
            public IntPtr hwndDropDown;

            public DATETIMEPICKERINFO()
            {
                cbSize = Marshal.SizeOf(this);
            }
        }

        private const int DTM_FIRST = 0x1000;
        private const int DTM_GETDATETIMEPICKERINFO = DTM_FIRST + 14;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, DATETIMEPICKERINFO lParam);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private const int SRCPAINT      = 0x00EE0086;
        private const int SRCAND        = 0x8800C6;
        private const int NOTSRCCOPY    = 0x330008;
        private const int SRCCOPY       = 0xCC0020;

        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, int rop);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr hgdiobj);

        private static Bitmap CreateColorBitmap(Color color, Size size)
        {
            var bmp = new Bitmap(size.Width, size.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(color);
            }
            return bmp;
        }

        private static void BitBlt(Bitmap bmp, Color color, int dwRop)
        {
            var rectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                IntPtr hdc = g.GetHdc();
                BitBlt(hdc, bmp, rectangle, SRCCOPY);
                using (Bitmap colorbmp = CreateColorBitmap(color, bmp.Size))
                {
                    BitBlt(hdc, colorbmp, rectangle, dwRop);
                }
                g.ReleaseHdc();
            }
        }

        private static void BitBlt(IntPtr hdc, Bitmap bmp, int dwRop)
        {
            BitBlt(hdc, bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), dwRop);
        }

        private static void BitBlt(IntPtr hdc, Bitmap bmp, Rectangle rectangle, int dwRop)
        {
            BitBlt(hdc, bmp, rectangle, rectangle.Location, dwRop);
        }

        private static void BitBlt(IntPtr hdc, Bitmap bmp, Rectangle rectangle, Point point, int dwRop)
        {
            IntPtr hdcSrc = CreateCompatibleDC(hdc);
            IntPtr hBitmap = bmp.GetHbitmap();
            IntPtr hbmpOld = SelectObject(hdcSrc, hBitmap);

            BitBlt(hdc,
                rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height,
                hdcSrc, point.X, point.Y, dwRop);

            SelectObject(hdcSrc, hbmpOld);
            DeleteObject(hBitmap);
            DeleteDC(hdcSrc);
        }

        #endregion

    }
}
