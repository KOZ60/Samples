namespace DispatchProxyTest
{
    using System.Threading;

    public partial class Form1 : Form
    {
        Form1 childForm;

        public Form1() {
            InitializeComponent();
        }

        void ChildTask() {
            var f = new Form1();
            childForm = f.CreateProxy();
            Application.Run(childForm);
        }

        public void Test() {
            MessageBox.Show("TEST");
        }

        private void button1_Click(object sender, EventArgs e) {
            var th = new Thread(ChildTask);
            th.Start();
        }

        private void button2_Click(object sender, EventArgs e) {
            if (childForm != null) {
                childForm.Test();
            }
        }
    }
}