using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomTextBox
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void numberText1_TextChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("{0} numberText1_TextChanged", DateTime.Now);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(numberText1.Value.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numberText1.Value = 123456789;
        }
    }
}
