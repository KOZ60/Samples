namespace VisualStyle.Animation
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// スレッド単位に初期処理および終了処理が必要な機能をサポートします。
    /// </summary>
    public abstract class ThreadInitializer
    {
        private readonly int threadId;

        /// <summary>
        /// ThreadInitializer のインスタンスを作成します。
        /// </summary>
        public ThreadInitializer() {
            threadId = Thread.CurrentThread.ManagedThreadId;
            Application.ThreadExit += Application_ThreadExit;
            OnInitialize();
        }

        /// <summary>
        /// 初期処理を行います。
        /// </summary>
        protected abstract void OnInitialize();

        private void Application_ThreadExit(object sender, EventArgs e) {
            if (threadId == Thread.CurrentThread.ManagedThreadId) {
                Application.ThreadExit -= Application_ThreadExit;
                OnExitThread();
            }
        }

        /// <summary>
        /// スレッドが終了する際に呼ばれます。終了処理を行ってください。
        /// </summary>
        protected abstract void OnExitThread();
    }
}
