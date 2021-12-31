using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Koz.Windows.Forms;
using Koz.Windows.Forms.Tools;

namespace WindowsFormsApp2
{
    class OwnerDrawTextBox : TextBox
    {

        private readonly OwnerDrawSupporter supporter;

        public OwnerDrawTextBox() {
            supporter = new OwnerDrawSupporter(this);
            SetStyle(ControlStyles.ResizeRedraw
                            | ControlStyles.UserPaint
                            | ControlStyles.OptimizedDoubleBuffer
                            | ControlStyles.AllPaintingInWmPaint, true);
        }

        public void BeginUpdate() {
            supporter.BeginUpdate();
        }

        public void EndUpdate() {
            supporter.EndUpdate();
        }

        protected override void OnPaint(PaintEventArgs e) {

            e.Graphics.Clear(Color.Yellow);
            string text = base.Text;

            try {
                using (var g = supporter.GetGdiGraphics(e.Graphics.GetHdc())) {

                    if (Focused || !HideSelection) {
                        int selStart = SelectionStart;
                        int selLength = SelectionLength;

                        int start = 0;
                        int length  = selStart;
                        g.SetBkMode(BkModeConstans.Transparent);
                        TextOut(g, text, start, length, ForeColor, BackColor);

                        g.SetBkMode(BkModeConstans.Opaque);
                        start = selStart;
                        length = selLength;
                        TextOut(g, text, start, length, SystemColors.HighlightText, SystemColors.Highlight);

                        g.SetBkMode(BkModeConstans.Transparent);
                        start = selStart + selLength;
                        length = TextLength - start;
                        TextOut(g, text, start, length, ForeColor, BackColor);

                    } else {
                        g.SetBkMode(BkModeConstans.Transparent);
                        TextOut(g, text, 0, TextLength, ForeColor, BackColor);
                    }
                }

            }
            finally {
                e.Graphics.ReleaseHdc();
            }

            // FixedSingle はクライアント領域に線が書かれる
            if (BorderStyle == BorderStyle.FixedSingle) {
                Rectangle borderRect = LayoutUtils.DeflateRect(ClientRectangle, new Padding(1));
                e.Graphics.DrawRectangle(SystemPens.WindowFrame, borderRect);
            }

            base.OnPaint(e);
        }

        private void TextOut(GdiGraphics g, string text,int start, int length, Color foreColor, Color backColor) {
            if (length == 0) return;
            Point pt = GetPositionFromCharIndex(start);
            string drawText = text.Substring(start, length);
            g.SetColor(foreColor, backColor);
            g.TextOut(pt.X, pt.Y + 1, drawText);
        }

        protected override void WndProc(ref Message m) {
            try {
                MessageMonitor.Enter<EditMessage>(ref m);
                base.WndProc(ref m);

            } finally {
                MessageMonitor.Exit<EditMessage>(ref m);
            }
        }
    }
}
