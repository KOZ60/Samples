﻿namespace CustomDrawApp
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class CustomDrawWindow : NativeWindow
    {
        public Control Owner { get; }


        public CustomDrawWindow(Control owner)
        {
            Owner = owner;
            Owner.HandleCreated += Owner_HandleCreated;
            if (Owner.IsHandleCreated)
            {
                Owner_HandleCreated(owner, EventArgs.Empty);
            }
        }

        private void Owner_HandleCreated(object sender, EventArgs e)
        {
            // Window が作成されたら AssignHandle
            AssignHandle(((Control)sender).Handle);
        }

        /// <summary>
        /// カスタムドロー用イベントです。カスタムドローを行うタイミングで呼び出されます。
        /// </summary>
        public event PaintEventHandler CustomDraw;

        protected virtual void OnCustomDraw(PaintEventArgs e)
        {
            if (CustomDraw != null) CustomDraw(this, e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCDESTROY:
                    WmNcDestroy(ref m);
                    break;

                case WM_PRINTCLIENT:
                    WmPrintClient(ref m);
                    break;

                case WM_PAINT:
                    WmPaint(ref m);
                    break;

                case WM_ERASEBKGND:
                    // メッセージを無視
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmNcDestroy(ref Message m)
        {
            base.WndProc(ref m);

            // WM_NCDESTROY が来たら ReleaseHandle
            ReleaseHandle();

            // bitmap 作成済みなら解放
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }

        private void WmPrintClient(ref Message m)
        {
            // WPARAM にセットされた HDC に描画を行う
            OnCustomDraw(m.WParam);
        }

        private void OnCustomDraw(IntPtr hdc)
        {
            using (var g = Graphics.FromHdc(hdc))
            {
                using (PaintEventArgs e = new PaintEventArgs(g, Owner.ClientRectangle))
                {
                    OnCustomDraw(e);
                }
            }
        }

        private void WmPaint(ref Message m)
        {
            if (m.WParam == IntPtr.Zero)
            {
                // コントロールに描画する
                var ps = new PAINTSTRUCT();
                var hdc = BeginPaint(m.HWnd, ref ps);
                if (hdc != IntPtr.Zero)
                {
                    var clip = ps.rcPaint.Rectangle;
                    if (clip.Width > 0 && clip.Height > 0)
                    {
                        IntPtr oldPal = SetUpPalette(hdc, false, false);
                        // ダブルバッファの処理
                        var bufferContext = BufferedGraphicsManager.Current;
                        using (BufferedGraphics bufferedGraphics = bufferContext.Allocate(hdc, Owner.ClientRectangle))
                        {
                            bufferedGraphics.Graphics.SetClip(clip);
                            using (var pevent = new PaintEventArgs(bufferedGraphics.Graphics, clip))
                            {
                                OnCustomDraw(pevent);
                                bufferedGraphics.Render();
                            }
                        }
                        if (oldPal != IntPtr.Zero)
                        {
                            SelectPalette(hdc, oldPal, 0);
                        }
                    }
                    EndPaint(m.HWnd, ref ps);
                }
            }
            else
            {
                // WPARAM にセットされた HDC に描画を行う
                OnCustomDraw(m.WParam);
            }
        }

        // 減色モードでの描画を高品質にする(らしい) .NET のソース参照。
        private static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette)
        {
            IntPtr halftonePalette = Graphics.GetHalftonePalette();
            IntPtr result = SelectPalette(dc, halftonePalette, (force ? 0 : 1));
            if (result != IntPtr.Zero && realizePalette)
            {
                RealizePalette(dc);
            }
            return result;
        }

        private Bitmap bitmap;

        public Bitmap NativeClientBitmap {
            get {
                var cs = Owner.ClientSize;
                if (bitmap == null)
                {
                    bitmap = new Bitmap(cs.Width, cs.Height);
                }
                else if (!bitmap.Size.Equals(cs))
                {
                    bitmap.Dispose();
                    bitmap = new Bitmap(cs.Width, cs.Height);
                }
                DrawNativeClient(bitmap);
                return bitmap;
            }
        }

        public void DrawNativeClient(Bitmap bmp)
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var hdc = graphics.GetHdc();
                try
                {
                    Message m = Message.Create(Owner.Handle, WM_ERASEBKGND, hdc, IntPtr.Zero);
                    base.DefWndProc(ref m);
                    m = Message.Create(Owner.Handle, WM_PAINT, hdc, IntPtr.Zero);
                    base.DefWndProc(ref m);
                }
                finally
                {
                    graphics.ReleaseHdc();
                }
            }
        }

        #region WINAPI

        private class ExternDll
        {
            public const string Gdi32 = "gdi32.dll";
            public const string User32 = "user32.dll";
        }

        private const int WM_NCDESTROY = 0x0082;
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_PAINT = 0x000F;
        private const int WM_PRINTCLIENT = 0x0318;

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
        private struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            private int reserved1;
            private int reserved2;
            private int reserved3;
            private int reserved4;
            private int reserved5;
            private int reserved6;
            private int reserved7;
            private int reserved8;
        }

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.Gdi32)]
        private static extern IntPtr SelectPalette(IntPtr hdc, IntPtr hpal, int bForceBackground);

        [DllImport(ExternDll.Gdi32)]
        private static extern int RealizePalette(IntPtr hDC);

        #endregion

    }
}
