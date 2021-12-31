using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms.Tools
{
    public class OwnerDrawSupporter : NativeWindowBase<Control>
    {
        public OwnerDrawSupporter(Control owner) : base(owner) {
            Owner.FontChanged += Owner_FontChanged;
            Owner.MouseEnter += Owner_MouseEnter;
            Owner.MouseLeave += Owner_MouseLeave;
            Owner.MouseUp += Owner_MouseUp;
            Owner.MouseDown += Owner_MouseDown;
            Owner.MouseCaptureChanged += Owner_MouseCaptureChanged;
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            SetWindowFont();
            updateCount = 0;
            lockCount = 0;
        }

        protected override void OnDisposed(EventArgs e) {
            base.OnDisposed(e);
            Owner.FontChanged -= Owner_FontChanged;
            Owner.MouseEnter -= Owner_MouseEnter;
            Owner.MouseLeave -= Owner_MouseLeave;
            Owner.MouseUp -= Owner_MouseUp;
            Owner.MouseDown -= Owner_MouseDown;
            Owner.MouseCaptureChanged -= Owner_MouseCaptureChanged;
            FontHandleWrapper?.Dispose();
        }

        private bool mouseIsOver = false;

        protected bool MouseIsOver {
            get {
                return mouseIsOver;
            }
            set {
                if (mouseIsOver != value) {
                    mouseIsOver = value;
                    Owner.Invalidate();
                }
            }
        }

        private bool mouseIsDown = false;

        protected bool MouseIsDown {
            get {
                return mouseIsDown;
            }
            set {
                if (mouseIsDown != value) {
                    mouseIsDown = value;
                    Owner.Invalidate();
                }
            }
        }

        private void Owner_MouseCaptureChanged(object sender, EventArgs e) {
            MouseIsDown = false;
            MouseIsOver = false;
        }

        private void Owner_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                MouseIsDown = true;
            }
        }

        private void Owner_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                MouseIsDown = false;
            }
        }

        private void Owner_FontChanged(object sender, EventArgs e) {
            SetWindowFont();
        }

        private void Owner_MouseLeave(object sender, EventArgs e) {
            MouseIsOver = false;
        }

        private void Owner_MouseEnter(object sender, EventArgs e) {
            MouseIsOver = true;
        }

        protected void SetWindowFont() {
            FontHandleWrapper = new FontHandleWrapper(Owner.Font);
            NativeMethods.SendMessage(new HandleRef(this, Handle),
                            NativeMethods.WM_SETFONT, FontHandle, IntPtr.Zero);
        }

        private FontHandleWrapper fontHandleWrapper;

        protected FontHandleWrapper FontHandleWrapper {
            get {
                if (fontHandleWrapper == null) {
                    fontHandleWrapper = new FontHandleWrapper(Owner.Font);
                }
                return fontHandleWrapper;
            }
            set {
                if (fontHandleWrapper != null) {
                    fontHandleWrapper.Dispose();
                }
                fontHandleWrapper = value;
            }
        }

        public IntPtr FontHandle {
            get {
                return FontHandleWrapper.Handle;
            }
        }

        public Size FontAverageSize {
            get {
                return FontHandleWrapper.AverageSize;
            }
        }

        private int updateCount = 0;

        public void BeginUpdate() {
            if (updateCount == 0) {
                updateCount++;
                SetRedraw(false);
                Owner.BeginInvoke((Action)EndUpdateInternal);
            }
        }

        public void EndUpdate() {
            if (updateCount > 0) {
                updateCount--;
                if (updateCount == 0) {
                    EndUpdateForce();
                }
            }
        }

        private void EndUpdateInternal() {
            if (updateCount > 0) {
                EndUpdateForce();
            }
        }

        private void EndUpdateForce() {
            updateCount = 0;
            SetRedraw(true);
            Owner.Refresh();
            if (Owner.Focused) {
                NativeMethods.HideCaret(new HandleRef(this, Handle));
                NativeMethods.ShowCaret(new HandleRef(this, Handle));
            }
        }

        private void SetRedraw(bool value) {
            IntPtr wp = value ? new IntPtr(-1) : IntPtr.Zero;
            NativeMethods.SendMessage(new HandleRef(this, Handle),
                                NativeMethods.WM_SETREDRAW, wp, IntPtr.Zero);
        }

        private int lockCount = 0;

        protected void Lock() {
            if (lockCount == 0) {
                NativeMethods.LockWindowUpdate(new HandleRef(this, Handle));
            }
            lockCount++;
        }

        protected void UnLock(bool refresh) {
            if (lockCount > 0) {
                lockCount--;
                if (lockCount == 0) {
                    NativeMethods.LockWindowUpdate(new HandleRef(this, IntPtr.Zero));
                    if (refresh) {
                        Owner.Refresh();
                    } else {
                        Owner.Invalidate(true);
                    }
                }
            } else {
                lockCount = 0;
            }
        }

        protected override void WndProc(ref Message m) {

            switch (m.Msg) {

                case NativeMethods.WM_LBUTTONDOWN:
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_LBUTTONDBLCLK:
                case NativeMethods.WM_SETFOCUS:
                    try {
                        Lock();
                        base.WndProc(ref m);
                    } finally {
                        UnLock(true);
                    }
                    break;

                case NativeMethods.WM_MOUSEMOVE:
                    if (MouseIsDown) {
                        try {
                            Lock();
                            base.WndProc(ref m);
                        } finally {
                            UnLock(true);
                        }
                    } else {
                        base.WndProc(ref m);
                    }
                    break;

                case NativeMethods.WM_CHAR:
                    try {
                        Lock();
                        base.WndProc(ref m);
                    } finally {
                        UnLock(false);
                    }
                    break;

                case NativeMethods.WM_KILLFOCUS:
                    try {
                        BeginUpdate();
                        base.WndProc(ref m);
                    } finally {
                        EndUpdate();
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        public GdiGraphics GetGdiGraphics(IntPtr hdc) {
            return new GdiGraphics(hdc, false, FontHandleWrapper.Handle, false);
        }
    }
}
