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
            using (var bmp = new Bitmap(32, 32)) {
                using (var g = Graphics.FromImage(bmp)) {
                    var es = new Size(10, 10);
                    g.DrawEllipse(Pens.Blue, new Rectangle(new Point(0, 0), es));
                    g.DrawEllipse(Pens.Yellow, new Rectangle(new Point(es.Width / 2, es.Height / 2), es));
                    g.DrawEllipse(Pens.Black, new Rectangle(new Point(es.Width, 0), es));
                    g.DrawEllipse(Pens.Green, new Rectangle(new Point(es.Width * 3 / 2, es.Height / 2), es));
                    g.DrawEllipse(Pens.Red, new Rectangle(new Point(es.Width * 2, 0), es));
                }
                Cursor = CursorResource.FromBitmap(bmp, new Point(0, 0)).Cursor;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
            //var cursorResource = Cursor.Tag as CursorResource;
            //if (cursorResource != null) {
            //    cursorResource.Dispose();
            //}
        }
    }
}
