namespace CustomDrawApp
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class DateTimePickerEx : DateTimePicker
    {
        readonly CustomDrawWindow cdw;

        public DateTimePickerEx() : base()
        {
            cdw = new CustomDrawWindow(this);
            cdw.CustomDraw += CustomDraw;
        }

        private Color _ForeColor = Color.Empty;

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override Color ForeColor {
            get {
                if (_ForeColor.IsEmpty)
                {
                    return SystemColors.WindowText;
                }
                return _ForeColor;
            }
            set {
                if (_ForeColor != value)
                {
                    _ForeColor = value;
                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                    OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeForeColor()
        {
            return !_ForeColor.IsEmpty;
        }

        public override void ResetForeColor()
        {
            _ForeColor = Color.Empty;
        }

        private Color _BackColor = Color.Empty;

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override Color BackColor {
            get {
                if (_BackColor.IsEmpty)
                {
                    return SystemColors.Window;
                }
                return _BackColor;
            }
            set {
                if (_BackColor != value)
                {
                    _BackColor = value;
                    if (IsHandleCreated)
                    {
                        Invalidate();
                    }
                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeBackColor()
        {
            return !_BackColor.IsEmpty;
        }

        public override void ResetBackColor()
        {
            _BackColor = Color.Empty;
        }

        // ややこしくなるのでプロパティを隠すｗ
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete()]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool RightToLeftLayout { get ; set; }

        private bool IsDisable {
            get {
                return !Enabled || (ShowCheckBox && (!Checked));
            }
        }

        private void CustomDraw(object sender, PaintEventArgs e)
        {
            // ネイティブイメージを描画
            var bmpOrg = cdw.NativeClientBitmap;
            var g = e.Graphics;
            g.DrawImage(bmpOrg, Point.Empty);

            // 枠を除いて描画領域とする
            var bsz = SystemInformation.Border3DSize;
            var clip = Rectangle.Inflate(this.ClientRectangle, -bsz.Width, -bsz.Height);

            IntPtr hdc = g.GetHdc();
            try
            {
                // BackColor を反映
                using (var bmpBack = (Bitmap)bmpOrg.Clone())
                {
                    BitBlt(bmpBack, BackColor, SRCAND);
                    BitBlt(hdc, bmpBack, clip, SRCCOPY);
                }

                // ForeColor を反映
                using (var bmpFore = CreateNegativeBitmap(bmpOrg))
                {
                    Color foreColor = IsDisable ? Color.Gray : ForeColor;
                    BitBlt(bmpFore, foreColor, SRCAND);
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
                    // チェックボックスの領域をネイティブイメージより描画
                    BitBlt(hdc, bmpOrg, checkBox, SRCCOPY);
                }

                // アップダウンコントロールがある場合はその領域をボタン領域とする
                if (dti.hwndUD != IntPtr.Zero)
                {
                    GetWindowRect(dti.hwndUD, out RECT rc);
                    button = RectangleToClient(rc.Rectangle);
                }

                // ボタンを描画
                if (!button.IsEmpty)
                {
                    // ボタンの領域をネイティブイメージより描画
                    BitBlt(hdc, bmpOrg, button, SRCCOPY);
                }

                // フォーカスがあるとき、キャレットの部分のみ転送
                if (this.ContainsFocus)
                {
                    // キャレットの色を検索する領域を設定
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

        // ビットマップの色を反転
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

        // ネイティブイメージの色を検索してキャレットの位置を取得
        // takiru氏に感謝！
        private unsafe static Rectangle GetCaretRectangle(Bitmap bmp, Rectangle canvas)
        {
            Rectangle bmpRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(bmpRect, ImageLockMode.ReadOnly, bmp.PixelFormat);
            try
            {
                int pixelSize = Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int rangeStartX = -1;
                int rangeEndX = -1;
                int rangeStartY = canvas.Top + 2;   // Yの開始座標を、とりあえず4ピクセル目とする
                int rangeEndY = bmpData.Height - 4; // Yの終了座標は、全体レイアウトの高さ-4

                byte* pixels = (byte*)bmpData.Scan0;
                Color baseColor = SystemColors.Highlight;
                byte[] hlBytes = new byte[] { baseColor.B, baseColor.G, baseColor.R };
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

        private class ExternDll
        {
            public const string Gdi32 = "gdi32.dll";
            public const string User32 = "user32.dll";
        }

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

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, DATETIMEPICKERINFO lParam);

        [DllImport(ExternDll.User32)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private const int SRCPAINT      = 0x00EE0086;
        private const int SRCAND        = 0x008800C6;
        private const int NOTSRCCOPY    = 0x00330008;
        private const int SRCCOPY       = 0x00CC0020;

        [DllImport(ExternDll.Gdi32)]
        private static extern int BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, int rop);

        [DllImport(ExternDll.Gdi32)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport(ExternDll.Gdi32)]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport(ExternDll.Gdi32)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport(ExternDll.Gdi32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hgdiobj);

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
