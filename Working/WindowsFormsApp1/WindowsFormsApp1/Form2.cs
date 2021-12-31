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
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2() {
            InitializeComponent();
        }

        [StructLayout(LayoutKind.Sequential)]
        private class DATETIMEPICKERINFO
        {
            public int cbSize;
            public RECT rcCheck;
            public int stateCheck;
            public RECT rcButton;
            public int stateButton;
            public IntPtr hwndEdit;
            public IntPtr hwndUD;
            public IntPtr hwndDropDown;

            public DATETIMEPICKERINFO() {
                cbSize = Marshal.SizeOf(this);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
            public Rectangle ToRectangle() {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }
        }

        private const int DTM_FIRST = 0x1000;
        private const int DTM_GETDATETIMEPICKERINFO = DTM_FIRST + 14;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
                    IntPtr hWnd,
                    int Msg,
                    IntPtr wParam,
                    DATETIMEPICKERINFO lParam);

        private void button1_Click(object sender, EventArgs e) {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            var dti = new DATETIMEPICKERINFO();
            SendMessage(this.Handle, DTM_GETDATETIMEPICKERINFO, IntPtr.Zero, dti);
            Debug.Print("hwndEdit={0} hwndUD={1} hwndDropDown={2}", dti.hwndEdit, dti.hwndUD, dti.hwndDropDown);
        }
    }
}
