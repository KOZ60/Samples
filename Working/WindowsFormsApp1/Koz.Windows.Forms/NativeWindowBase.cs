﻿using System;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public abstract class NativeWindowBase<T> : NativeWindow where T : Control
    {
        private T _Owner;

        protected T Owner { 
            get {
                return _Owner;
            }
            private set {
                if (_Owner != null) {
                    _Owner.HandleCreated -= Owner_HandleCreated;
                    _Owner.HandleDestroyed -= Owner_HandleDestroyed;
                    _Owner.Disposed -= Owner_Disposed;
                }
                _Owner = value;
                if (_Owner != null) {
                    _Owner.HandleCreated += Owner_HandleCreated;
                    _Owner.HandleDestroyed += Owner_HandleDestroyed;
                    _Owner.Disposed += Owner_Disposed;
                }
            }
        }

        public NativeWindowBase(T owner) {
            Owner = owner;
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
            OnDisposed(e);
            Owner = null;
        }

        protected virtual void OnHandleCreated(EventArgs e) { }
        protected virtual void OnDisposed(EventArgs e) { }
        protected virtual void OnHandleDestroyed(EventArgs e) { }

    }
}
