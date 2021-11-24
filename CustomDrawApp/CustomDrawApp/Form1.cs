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
            textBoxEx1.Focus();
            textBoxEx1.CaretColor = Color.Red;
        }

        private void textBoxEx2_TextChanged(object sender, System.EventArgs e) {

        }

        private void button2_Click(object sender, System.EventArgs e) {
            textBoxEx1.Focus();
            textBoxEx1.CaretColor = Color.Blue;
        }
    }
}
