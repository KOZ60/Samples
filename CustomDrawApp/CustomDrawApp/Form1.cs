namespace CustomDrawApp
{
    using System.Windows.Forms;
    using System.Drawing;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, System.EventArgs e) {
            textBoxEx1.CaretColor = Color.Red;
            textBoxEx1.Focus();
        }

        private void button2_Click(object sender, System.EventArgs e) {
            textBoxEx1.CaretColor = Color.Blue;
            textBoxEx1.Focus();
        }

    }
}
