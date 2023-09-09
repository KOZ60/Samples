namespace CustomDrawApp
{
    using System;
    using System.Reflection;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class CustomDrawWindow : NativeWindow
    {
        public Control Owner { get; }

        private delegate void OnPaintDelegate(PaintEventArgs pe);
        private delegate void OnPaintBackgroundDelegate(PaintEventArgs pe);
        private delegate void DefWndProcDelegate(ref Message m);

        private readonly OnPaintDelegate OwnerOnPaint;
        private readonly OnPaintBackgroundDelegate OwnerOnPaintBackground;
        private readonly DefWndProcDelegate OwnerDefWndProc;

        public CustomDrawWindow(Control owner) {
            Owner = owner;
            OwnerOnPaint = CreateDelegate<OnPaintDelegate>("OnPaint");
            OwnerOnPaintBackground = CreateDelegate<OnPaintBackgroundDelegate>("OnPaintBackground");
            OwnerDefWndProc = CreateDelegate<DefWndProcDelegate>("DefWndProc");
            Owner.HandleCreated += Owner_HandleCreated;
            if (Owner.IsHandleCreated) {
                Owner_HandleCreated(owner, EventArgs.Empty);
            }
        }

        private void Owner_HandleCreated(object sender, EventArgs e) {
            // Window が作成されたら AssignHandle
            AssignHandle(((Control)sender).Handle);
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
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

        private void WmNcDestroy(ref Message m) {
            base.WndProc(ref m);
            // WM_NCDESTROY が来たら ReleaseHandle
            ReleaseHandle();
        }

        private void WmPrintClient(ref Message m) {
            // WPARAM にセットされた HDC に描画を行う
            DrawDeviceContext(m.WParam);
        }

        private void WmPaint(ref Message m) {
            if (m.WParam == IntPtr.Zero) {
                // コントロールに描画する
                DrawControl(ref m);
            } else {
                // WPARAM にセットされた HDC に描画を行う
                DrawDeviceContext(m.WParam);
            }
        }

        private void DrawControl(ref Message m) {
            var ps = new PAINTSTRUCT();
            var hdc = BeginPaint(m.HWnd, ref ps);
            if (hdc != IntPtr.Zero) {
                var clip = ps.rcPaint.Rectangle;
                if (clip.Width > 0 && clip.Height > 0) {
                    IntPtr oldPal = SetUpPalette(hdc, false, false);
                    // ダブルバッファの処理
                    var bufferContext = BufferedGraphicsManager.Current;
                    using (var bufferedGraphics = bufferContext.Allocate(hdc,
                                                            Owner.ClientRectangle)) {
                        bufferedGraphics.Graphics.SetClip(clip);
                        using (var pevent = new PaintEventArgs(bufferedGraphics.Graphics, clip)) {
                            CallUserPaint(pevent);
                            bufferedGraphics.Render();
                        }
                    }
                    if (oldPal != IntPtr.Zero) {
                        SelectPalette(hdc, oldPal, 0);
                    }
                }
                EndPaint(m.HWnd, ref ps);
            }
        }

        private void DrawDeviceContext(IntPtr hdc) {
            using (var g = Graphics.FromHdc(hdc)) {
                using (PaintEventArgs e = new PaintEventArgs(g, Owner.ClientRectangle)) {
                    CallUserPaint(e);
                }
            }
        }

        private void CallUserPaint(PaintEventArgs pevent) {
            var g = pevent.Graphics;
            var state = g.Save();

            var hdc = g.GetHdc();
            CallDefWndProc(WM_ERASEBKGND, hdc, IntPtr.Zero);
            g.ReleaseHdc();

            OwnerOnPaintBackground(pevent);

            if (state != null) {
                g.Restore(state);
            }

            hdc = g.GetHdc();
            CallDefWndProc(WM_PAINT, hdc, IntPtr.Zero);
            g.ReleaseHdc();

            OwnerOnPaint(pevent);
        }

        private void CallDefWndProc(int msg, IntPtr wparam, IntPtr lparam) {
            var m = Message.Create(Owner.Handle, msg, wparam, lparam);
            OwnerDefWndProc(ref m);
        }

        // 減色モードでの描画を高品質にする(らしい) .NET のソース参照。
        private static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette) {
            IntPtr halftonePalette = Graphics.GetHalftonePalette();
            IntPtr result = SelectPalette(dc, halftonePalette, (force ? 0 : 1));
            if (result != IntPtr.Zero && realizePalette) {
                RealizePalette(dc);
            }
            return result;
        }

        public Bitmap CreateNativeBitmap() {
            var cs = Owner.ClientSize;
            var bitmap = new Bitmap(cs.Width, cs.Height,
                   System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            DrawNativeClient(bitmap);
            return bitmap;
        }

        public void DrawNativeClient(Bitmap bmp) {
            using (var graphics = Graphics.FromImage(bmp)) {
                var hdc = graphics.GetHdc();
                try {
                    CallDefWndProc(WM_ERASEBKGND, hdc, IntPtr.Zero);
                    CallDefWndProc(WM_PAINT, hdc, IntPtr.Zero);
                } finally {
                    graphics.ReleaseHdc();
                }
            }
        }

        protected T CreateDelegate<T>(string methodName) where T : Delegate {
            var info = typeof(Control).GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)Delegate.CreateDelegate(typeof(T), Owner, info);
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
