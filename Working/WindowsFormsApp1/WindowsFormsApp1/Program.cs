using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException +=
                 new System.Threading.ThreadExceptionEventHandler(
                     Application_ThreadException);

            Application.SetUnhandledExceptionMode(
                 UnhandledExceptionMode.ThrowException);

            AppDomain.CurrentDomain.UnhandledException +=
                  new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }

        private static void Application_ThreadException(object sender,
            System.Threading.ThreadExceptionEventArgs e) {
            try {
                MessageBox.Show(e.Exception.Message, "エラー");
            } finally {
                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender,
            UnhandledExceptionEventArgs e) {
            try {
                MessageBox.Show(((Exception)e.ExceptionObject).Message, "エラー");
            } finally {
                Environment.Exit(1);
            }
        }
    }
}
