using System;
using System.Collections;
using System.Collections.Generic;

namespace Koz.Fx.Interop
{
    internal class ComWrapperCollection : IEnumerable<ComWrapper>
    {
        Dictionary<SafeComObject, WeakReference> _Dictionary
                        = new Dictionary<SafeComObject, WeakReference>();

        public ComWrapperCollection() { }

        public int Count {
            get {
                return _Dictionary.Count;
            }
        }

        public WeakReference Add(ComWrapper item) {
            lock (SyncRoot) {
                SafeComObject key = item.ComObject;
                if (_Dictionary.ContainsKey(key)) {
                    _Dictionary.Remove(key);
                }
                WeakReference wr = new WeakReference(item);
                _Dictionary.Add(key, wr);
                return wr;
            }
        }

        public void Remove(ComWrapper item) {
            lock (SyncRoot) {
                SafeComObject key = item.ComObject;
                if (_Dictionary.ContainsKey(key)) {
                    _Dictionary.Remove(key);
                }
            }
        }

        private  object SyncRoot {
            get {
                return ((ICollection)_Dictionary).SyncRoot;
            }
        }

        public bool TryGetValue(SafeComObject comObject, out ComWrapper item) {
            lock (SyncRoot) {
                if (_Dictionary.TryGetValue(comObject, out WeakReference wr)) {
                    if (wr.Target != null) {
                        item = (ComWrapper)wr.Target;
                        return true;
                    } else {
                        _Dictionary.Remove(comObject);
                    }
                }
            }
            item = null;
            return false;
        }

        public List<ComWrapper> ToList() {
            List<ComWrapper> list = new List<ComWrapper>(this.Count);
            lock (SyncRoot) {
                foreach (WeakReference wr in _Dictionary.Values) {
                    if (wr.Target != null) {
                        list.Add((ComWrapper)wr.Target);
                    }
                }
            }
            return list;
        }

        public IEnumerator<ComWrapper> GetEnumerator() {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ToList().GetEnumerator();
        }
    }
}
