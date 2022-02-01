using System.Collections;
using System.Collections.Generic;

namespace Koz.Fx.Interop.Test.Excel
{
    public abstract class ExcelCollection<T> : ExcelObject, IEnumerable<T> where T : ExcelObject
    {
        protected ExcelCollection(object comObject) : base(comObject) { }

        public int Count {
            get {
                return GetProperty<int>("Count");
            }
        }

        public T this[int index] {
            get {
                return GetWrapperProperty<T>("Item", index);
            }
        }

        public T this[string name] {
            get {
                return GetWrapperProperty<T>("Item", name);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            int count = this.Count;
            for (int i = 1; i <= count; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            int count = this.Count;
            for (int i = 1; i <= count; i++) {
                yield return this[i];
            }
        }

    }
}
