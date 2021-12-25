using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Koz.Windows.Forms
{
    public class OwnerDrawController : NativeWindowBase<Control>
    {
        public event PaintEventHandler OwnerDraw;

        public OwnerDrawController(Control control) : base(control) { }

        private int lockCount = 0;

        public int LockWindow() {
            if (lockCount == 0) {
                NativeMethods.LockWindowUpdate(new HandleRef(this, Handle));
            }
            lockCount += 1;
            return lockCount;
        }

        public int UnlockWindow() {
            lockCount -= 1;
            if (lockCount <= 0) {
                NativeMethods.LockWindowUpdate(new HandleRef(this, IntPtr.Zero));
                lockCount = 0;
            }
            return lockCount;
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_PAINT:
                    WmPaint(ref m);
                    break;

                case NativeMethods.WM_PRINTCLIENT:
                    WmPrintClient(ref m);
                    break;

                case NativeMethods.WM_ERASEBKGND:
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmPaint(ref Message m) {
            if (m.WParam == IntPtr.Zero) {
                var ps = new NativeMethods.PAINTSTRUCT();
                IntPtr hdc = NativeMethods.BeginPaint(new HandleRef(this, m.HWnd), ref ps);
                var clip = ps.rcPaint.Rectangle;
                if (clip.Width > 0 && clip.Height > 0) {
                    HandleRef oldPal = new HandleRef(this, NativeMethods.SetUpPalette(hdc, false, false));
                    try {
                        var bufferContext = BufferedGraphicsManager.Current;
                        using (var bufferedGraphics = bufferContext.Allocate(hdc, Owner.ClientRectangle)) {
                            bufferedGraphics.Graphics.SetClip(clip);
                            using (var e = new PaintEventArgs(bufferedGraphics.Graphics, clip)) {
                                OnOwnerDraw(e);
                            }
                            bufferedGraphics.Render();
                        }
                    } finally {
                        NativeMethods.SelectPalette(new HandleRef(this, hdc), oldPal, 0);
                    }
                }
                NativeMethods.EndPaint(new HandleRef(this, m.HWnd), ref ps);
            } else {
                using (var g = Graphics.FromHdcInternal(m.WParam)) {
                    using (var e = new PaintEventArgs(g, Owner.ClientRectangle)) {
                        OnOwnerDraw(e);
                    }
                }
            }
        }

        private void WmPrintClient(ref Message m) {
            using (var g = Graphics.FromHdcInternal(m.WParam)) {
                using (var e = new PaintEventArgs(g, Owner.ClientRectangle)) {
                    OnOwnerDraw(e);
                }
            }
        }

        public virtual void DrawNative(PaintEventArgs e) {
            e.Graphics.Clear(Owner.BackColor);
            IntPtr hdc = e.Graphics.GetHdc();
            var m = Message.Create(this.Handle, NativeMethods.WM_PAINT, hdc, IntPtr.Zero);
            base.DefWndProc(ref m);
            e.Graphics.ReleaseHdc();
        }

        protected virtual void OnOwnerDraw(PaintEventArgs e) {
            OwnerDraw?.Invoke(this, e);
        }

    }
}
