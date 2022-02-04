using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace OlympicCursor
{
    public partial class Form1 : Form
    {
        public Form1() {
            InitializeComponent();
            this.Cursor = CreateCursor();
        }

        static Cursor CreateCursor() {
            using (var bmp = new Bitmap(32, 32)) {
                using (var g = Graphics.FromImage(bmp)) {
                    var es = new Size(10, 10);
                    g.DrawEllipse(Pens.Blue, new Rectangle(new Point(0, 0), es));
                    g.DrawEllipse(Pens.Yellow, new Rectangle(new Point(es.Width / 2, es.Height / 2), es));
                    g.DrawEllipse(Pens.Black, new Rectangle(new Point(es.Width, 0), es));
                    g.DrawEllipse(Pens.Green, new Rectangle(new Point(es.Width * 3 / 2, es.Height / 2), es));
                    g.DrawEllipse(Pens.Red, new Rectangle(new Point(es.Width * 2, 0), es));
                }
                using (var tmpIcon =  new Cursor(bmp.GetHicon())) {
                    var info = new ICONINFO();
                    GetIconInfo(tmpIcon.Handle, info);
                    info.xHotspot = 0;
                    info.yHotspot = 0;
                    info.fIcon = 0;
                    IntPtr newIcon  = CreateIconIndirect(info);
                    return new Cursor(newIcon);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private class ICONINFO
        {
            public int fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask = IntPtr.Zero;
            public IntPtr hbmColor = IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern bool GetIconInfo(IntPtr hIcon, ICONINFO piconinfo);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect([In] ICONINFO iconInfo);

    }
}
