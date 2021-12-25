using System;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public abstract class NativeWindowBase<T> : NativeWindow where T : Control
    {
        protected readonly T Owner;

        public NativeWindowBase(T owner) {
            Owner = owner;
            Owner.HandleCreated += Owner_HandleCreated;
            Owner.HandleDestroyed += Owner_HandleDestroyed;
            Owner.Disposed += Owner_Disposed;
            if (Owner.IsHandleCreated) {
                Owner_HandleCreated(owner, EventArgs.Empty);
            }
        }

        private void Owner_HandleCreated(object sender, EventArgs e) {
            Control con = (Control)sender;
            AssignHandle(con.Handle);
            OnHandleCreated(e);
        }

        private void Owner_HandleDestroyed(object sender, EventArgs e) {
            OnHandleDestroyed(e);
        }

        private void Owner_Disposed(object sender, EventArgs e) {
            Control con = (Control)sender;
            con.HandleCreated -= Owner_HandleCreated;
            Owner.HandleDestroyed -= Owner_HandleDestroyed;
            con.Disposed -= Owner_Disposed;
            OnDisposed(e);
        }

        protected virtual void OnHandleCreated(EventArgs e) { }
        protected virtual void OnDisposed(EventArgs e) { }
        protected virtual void OnHandleDestroyed(EventArgs e) { }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_NCDESTROY:
                    base.WndProc(ref m);
                    ReleaseHandle();
                    break;

                default:
                    base.WndProc(ref m);
                    break;

            }
        }
    }
}
