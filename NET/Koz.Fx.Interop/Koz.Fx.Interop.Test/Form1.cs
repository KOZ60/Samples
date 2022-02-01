using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Koz.Fx.Interop.Test
{
    public partial class Form1 : Form
    {
        Excel.Application xlsApp;
        ExcelCallBack excelCallBack;

        Word.Application wordApp;
        WordCallBack wordlCallBack;

        public Form1() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            label1.Text = "";
            label2.Text = "";
        }

        private void Button1_Click(object sender, EventArgs e) {
            label1.Text = "";
            label1.Refresh();

            if (xlsApp == null) {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                excelCallBack = new ExcelCallBack(xlsApp);
            } else {
                xlsApp.Quit();
                xlsApp = null;
                excelCallBack = null;
            }
        }

        private void Button2_Click(object sender, EventArgs e) {
            label2.Text = "";
            label2.Refresh();
            button2.Enabled = false;

            try {
                wordApp = new Word.Application();
            } catch {
                wordApp = null;
                button2.Enabled = true;
                return;
            }

            wordApp.QuitEvent += WordApp_QuitEvent;
            wordApp.Visible = true;
            wordlCallBack = new WordCallBack(wordApp);
        }

        private void WordApp_QuitEvent() {
            button2.Enabled = true;
            wordApp = null;
        }
    }
}
