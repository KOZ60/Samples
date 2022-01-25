using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomTextBox
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            (new Form2()).Show();
            (new Form3()).Show();
            Application.Run(new AppContext(new Form1()));
        }
    }

    class AppContext : ApplicationContext
    {
        public AppContext(Form form) {
            MainForm = form;
        }

        protected override void OnMainFormClosed(object sender, EventArgs e) {
            foreach (Form f in Application.OpenForms) {
                if (!f.InvokeRequired) {
                    MainForm = f;
                    return;
                }
            }
            base.OnMainFormClosed(sender, e);
        }
    }
}
