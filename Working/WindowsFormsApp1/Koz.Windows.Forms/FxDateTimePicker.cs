using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;
using Koz.Windows.Forms.Tools;

namespace Koz.Windows.Forms
{
    public class FxDateTimePicker : DateTimePicker
    {
        public FxDateTimePicker() { }

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

            public override string ToString() {
                return string.Format("cbSize={0}, rcCheck={1}, stateCheck={2}, rcButton={3}, stateButton={4}, hwndEdit={5}, hwndUD={6}, hwndDropDown={7}",
                    cbSize, rcCheck, stateCheck, rcButton, stateButton, hwndEdit, hwndUD, hwndDropDown);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
            public Rectangle ToRectangle() {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }

            public override string ToString() {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom).ToString();
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


        protected override void WndProc(ref Message m) {
            MessageMonitor.Enter<DateTimePickerMessage>(ref m);
            switch (m.Msg) {
                case NativeMethods.WM_PAINT:
                    base.WndProc(ref m);
                    var dti = new DATETIMEPICKERINFO();
                    SendMessage(this.Handle, DTM_GETDATETIMEPICKERINFO, IntPtr.Zero, dti);
                    Debug.Print(dti.ToString());

                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
            MessageMonitor.Exit<DateTimePickerMessage>(ref m);
        }
    }
}
