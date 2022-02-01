using System;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop
{
    /// <summary>
    /// COM オブジェクトのイベントを発生するクラスの基底クラス
    /// </summary>
    [ComVisible(true)]
    public abstract class SinkObject : StandardOleMarshalObject, IDisposable
    {
        private Guid guid;
        private ComTypes.IConnectionPoint connectionPoint;
        private int cookie;
        private bool isAdvised;

        /// <summary>
        /// SinkObject のインスタンスを作成します。
        /// </summary>
        /// <param name="guid"></param>
        protected SinkObject(Guid guid) {
            this.guid = guid;
        }

        internal void Advise(ComWrapper owner) {
            Owner = owner;
            var cpc = owner.ComObject.Target as ComTypes.IConnectionPointContainer;
            cpc.FindConnectionPoint(ref guid, out connectionPoint);
            connectionPoint.Advise(this, out cookie);
            isAdvised = true;
        }

        internal void Unadvise() {
            if (isAdvised) {
                connectionPoint.Unadvise(cookie);
            }
            isAdvised = false;
        }

        /// <summary>
        /// このオブジェクトのオーナーを取得します。
        /// </summary>
        protected ComWrapper Owner { get; private set; }

        /// <summary>
        /// 使用されているリソースを解放します。
        /// </summary>
        public void Dispose() {
            CallDispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// このオブジェクトが解放済みかどうかを取得します。
        /// </summary>
        public bool IsDisposed { get; private set; }

        private void CallDispose(bool disposing) {
            if (!IsDisposed) {
                IsDisposed = true;
                Dispose(disposing);
            }
        }

        /// <summary>
        /// 使用されているアンマネージ リソースを解放し、オプションでマネージ リソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージ リソースとアンマネージ リソースの両方を解放する場合は true。アンマネージ リソースだけを解放する場合は false。</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
            }
            Unadvise();
            if (connectionPoint != null) {
                Marshal.ReleaseComObject(connectionPoint);
            }
        }

        /// <summary>
        /// ガベージコレクタがオブジェクトを回収するときに呼ばれます。
        /// </summary>
        ~SinkObject() {
            CallDispose(false);
        }
    }
}
