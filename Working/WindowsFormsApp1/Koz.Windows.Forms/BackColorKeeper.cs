using Microsoft.Win32.SafeHandles;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    /// <summary>
    /// 不要になりました。
    /// </summary>
    /// <remarks>Multiline でなければダメ</remarks>
    public class BackColorKeeper : NativeWindowBase<Control>
    {
        private Control parent;
        private NativeBrush backColorBrush;

        public BackColorKeeper(Control control) : base(control) {
            Owner.ParentChanged += Owner_ParentChanged;
            Parent = Owner.Parent;
        }

        protected override void OnDisposed(EventArgs e) {
            base.OnDisposed(e);
            Parent = null;
            Owner.ParentChanged -= Owner_ParentChanged;
        }

        private void Owner_ParentChanged(object sender, EventArgs e) {
            Parent = Owner.Parent;
        }

        protected Control Parent {
            get {
                return parent;
            }
            set {
                if (parent != null) {
                    if (Handle != IntPtr.Zero) {
                        ReleaseHandle();
                    }
                    parent.HandleCreated -= Parent_HandleCreated;
                }
                parent = value;
                if (parent != null) {
                    parent.HandleCreated += Parent_HandleCreated;
                    if (parent.IsHandleCreated) {
                        Parent_HandleCreated(parent, EventArgs.Empty);
                    }
                }
            }
        }

        private void Parent_HandleCreated(object sender, EventArgs e) {
            AssignHandle(Parent.Handle);
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_CTLCOLOR:
                case NativeMethods.WM_CTLCOLORBTN:
                case NativeMethods.WM_CTLCOLORDLG:
                case NativeMethods.WM_CTLCOLORMSGBOX:
                case NativeMethods.WM_CTLCOLORSCROLLBAR:
                case NativeMethods.WM_CTLCOLOREDIT:
                case NativeMethods.WM_CTLCOLORLISTBOX:
                case NativeMethods.WM_CTLCOLORSTATIC:

                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLOR:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORBTN:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORDLG:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORMSGBOX:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORSCROLLBAR:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLOREDIT:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORLISTBOX:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORSTATIC:
                    WmCtlColorControl(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmCtlColorControl(ref Message m) {
            if (Owner.IsHandleCreated && m.LParam == Owner.Handle) {
                m.Result = InitializeDCForWmCtlColor(m.WParam, m.Msg);
            } else {
                base.WndProc(ref m);
            }
        }

        protected virtual IntPtr InitializeDCForWmCtlColor(IntPtr dc, int msg) {
            NativeMethods.SetTextColor(new HandleRef(null, dc), ColorTranslator.ToWin32(Owner.ForeColor));
            NativeMethods.SetBkColor(new HandleRef(null, dc), ColorTranslator.ToWin32(Owner.BackColor));
            return BackColorBrush.Handle;
        }

        private class NativeBrush : SafeHandleZeroOrMinusOneIsInvalid
        {
            public Color Color { get; }
            private readonly bool needDelete;

            public NativeBrush(Color color) : base(true) {
                Color = color;
                if (ColorTranslator.ToOle(color) < 0) {
                    handle = NativeMethods.GetSysColorBrush(ColorTranslator.ToOle(color) & 0xFF);
                    needDelete = false;
                } else {
                    handle = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(color));
                    needDelete = true;
                }
            }

            protected override bool ReleaseHandle() {
                if (needDelete) {
                    NativeMethods.DeleteObject(new HandleRef(this, handle));
                }
                handle = IntPtr.Zero;
                return true;
            }

            public IntPtr Handle {
                get {
                    return handle;
                }
            }
        }

        private NativeBrush BackColorBrush {
            get {
                if (backColorBrush == null) {
                    backColorBrush = new NativeBrush(Owner.BackColor);

                } else if (backColorBrush.Color != Owner.BackColor) {
                    backColorBrush.Dispose();
                    backColorBrush = new NativeBrush(Owner.BackColor);
                }
                return backColorBrush;
            }
        }
    }
}
