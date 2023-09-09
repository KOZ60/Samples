using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Koz.Fx.Interop
{
    /// <summary>
    /// IDisposable をサポートするクラス
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerStepThrough]
    [ComVisible(true)]
    public abstract class DisposableObject : IDisposable
    {
        /// <summary>
        /// このクラスのインスタンスを作成します。
        /// </summary>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected DisposableObject() { }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose() {
            CallDispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// このオブジェクトが Dispose されたかどうかを取得します。
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
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// ガベージコレクタがオブジェクトを回収するときに呼ばれます。
        /// </summary>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        ~DisposableObject() {
            CallDispose(false);
        }
    }
}
