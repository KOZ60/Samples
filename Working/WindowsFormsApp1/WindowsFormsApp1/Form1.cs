using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            propertyGrid1.SelectedObject = textEditor1;
            propertyGrid1.GotoTop();

        }

        private void button1_Click(object sender, EventArgs e) {
            textEditor1.Text = "TEST!!!!";
        }
    }
}
