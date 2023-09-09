namespace MultiThreadWindow
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using Timer = System.Windows.Forms.Timer;

    public partial class Form1 : Form
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public Form1() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            var th = new Thread(new ParameterizedThreadStart(OtherThread));
            th.Start(this.Handle);
            base.OnLoad(e);
        }

        private static void OtherThread(object args) {
            IntPtr mainWindowHandle = (IntPtr)args;
            var subForm = new ThreadForm();
            subForm.Visible = true;
            subForm.Location = Point.Empty;
            subForm.Size = new Size(100, 20);
            SetParent(subForm.Handle, mainWindowHandle);
            Application.Run(subForm);
        }

        private class ThreadForm : Form
        {
            readonly Timer timer;

            public ThreadForm() {
                TopLevel = false;
                FormBorderStyle = FormBorderStyle.None;
                ControlBox = false;
                timer = new Timer();
                timer.Interval = 1000;
                timer.Tick += Timer_Tick;
                timer.Start();
            }

            private void Timer_Tick(object sender, EventArgs e) {
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e) {
                var p = this.Parent;
                e.Graphics.Clear(BackColor);
                TextRenderer.DrawText(e.Graphics, 
                    DateTime.Now.ToString("hh:mm:ss"), Font, Point.Empty, ForeColor);
                base.OnPaint(e);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Cursor = Cursors.WaitCursor;
            Thread.Sleep(10000); // 重い処理
            this.Cursor = Cursors.Default;
        }
    }
}
