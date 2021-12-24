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

            //textEditor1.BackgroundImageLayout = ImageLayout.Zoom;
            //textEditor1.BackgroundImage = Image.FromFile(@"D:\TEST\BACK.jpg");
        }

        private void button1_Click(object sender, EventArgs e) {
            for (int i = 0; i < textEditor1.TextLength; i++) {
                Debug.Print("{0} {1} {2}", i, textEditor1.Text[i], textEditor1.GetPositionFromCharIndex(i));
            }
        }

        private void propertyGrid1_Click(object sender, EventArgs e) {

        }
    }
}
