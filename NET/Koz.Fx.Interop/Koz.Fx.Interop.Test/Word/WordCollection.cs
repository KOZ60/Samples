using System.Collections.Generic;
using System.Collections;

namespace Koz.Fx.Interop.Test.Word
{
    public abstract class WordCollection<T> : WordObject, IEnumerable<T> where T : WordObject
    {
        protected WordCollection(object comObject) : base(comObject) { }

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
