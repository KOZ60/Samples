namespace MonthPickerSample
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class MonthPicker : DateTimePicker
    {
        internal const string Default_CustomFormat = "yyyy/MM";

        public MonthPicker()
        {
            base.Format = DateTimePickerFormat.Custom;
            base.CustomFormat = Default_CustomFormat;
        }

        [DefaultValue(DateTimePickerFormat.Custom)]
        public new DateTimePickerFormat Format {
            get => base.Format;
            set => base.Format = value;
        }

        [DefaultValue(Default_CustomFormat)]
        public new string CustomFormat {
            get => base.CustomFormat;
            set => base.CustomFormat = value;
        }

        protected override void OnDropDown(EventArgs eventargs)
        {
            // ドロップダウンが表示されるとき月選択に切り替える
            var calhwnd = SendMessage(Handle, DTM_GETMONTHCAL, IntPtr.Zero, IntPtr.Zero);
            SendMessage(calhwnd, MCM_SETCURRENTVIEW, IntPtr.Zero, (IntPtr)MCMV_YEAR);
            base.OnDropDown(eventargs);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NOFITY:
                    var nmhdr = (NMHDR)m.GetLParam(typeof(NMHDR));
                    switch (nmhdr.code)
                    {
                        case MCN_VIEWCHANGE:
                            // 日選択に戻るとき閉じる
                            var vc = (NMVIEWCHANGE)m.GetLParam(typeof(NMVIEWCHANGE));
                            if (vc.dwNewView == MCMV_MONTH)
                            {
                                SendMessage(Handle, DTM_CLOSEMONTHCAL, IntPtr.Zero, IntPtr.Zero);
                            }

                            break;
                    }
                    base.WndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }


        private const int WM_NOFITY = 0x004e;

        private const int DTM_FIRST = 0x1000;
        private const int DTM_GETMONTHCAL = DTM_FIRST + 8;
        private const int DTM_CLOSEMONTHCAL = DTM_FIRST + 13;

        private const int MCM_SETCURRENTVIEW = 0x1000 + 32;

        private const int MCN_VIEWCHANGE = -750;

        private const uint MCMV_MONTH = 0;
        private const uint MCMV_YEAR = 1;
        private const uint MCMV_DECADE = 2;
        private const uint MCMV_CENTURY = 3;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public int code;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NMVIEWCHANGE
        {
            public NMHDR nmhdr;
            public uint dwOldView;
            public uint dwNewView;
        }
    }
}
