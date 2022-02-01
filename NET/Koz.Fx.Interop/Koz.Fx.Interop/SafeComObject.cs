using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace Koz.Fx.Interop
{
    /// <summary>
    /// COM オブジェクトを安全に扱うためのクラス。
    /// ディクショナリのキーとして使うために IUnknown インターフェイスのアドレスで比較を行います。
    /// </summary>
    internal class SafeComObject : SafeHandleMinusOneIsInvalid
    {
        public SafeComObject(object comObject) : this(comObject, true) { }

        /// <summary>
        /// SafeComObject のインスタンスを作成します。
        /// </summary>
        /// <param name="comObject">COM オブジェクト。</param>
        /// <param name="ownsHandle">
        /// COM オブジェクトをリリースするときは true。
        /// ディクショナリのキーとして使用する場合は false。
        /// </param>
        public SafeComObject(object comObject, bool ownsHandle) : base(true) {
            this.OwnsHandle = ownsHandle;
            handle = Marshal.GetIUnknownForObject(comObject);
            Target = comObject;
        }

        protected override bool ReleaseHandle() {
            Marshal.Release(handle);
            if (OwnsHandle) {
                int count = Marshal.ReleaseComObject(Target);
            }
            return true;
        }

        public bool OwnsHandle { get; set; }

        public object Target { get; }

        private string typeName = null;

        public string TypeName {
            get {
                if (typeName == null) {
                    typeName = InteropUtils.GetComTypeName(Target);
                }
                return typeName;
            }
        }

        public override bool Equals(object obj) {
            SafeComObject other = obj as SafeComObject;
            return this == other;
        }

        public override int GetHashCode() {
            return handle.GetHashCode();
        }

        public static bool operator ==(SafeComObject a, SafeComObject b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }
            if (((object)a) == null || ((object)b) == null) {
                return false;
            }
            return a.handle == b.handle;
        }

        public static bool operator !=(SafeComObject a, SafeComObject b) {
            return !(a == b);
        }

    }
}
