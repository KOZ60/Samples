namespace CustomDrawApp
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private CustomDrawWindow hooker;

        private void Form1_Load(object sender, EventArgs e)
        {
            hooker = new CustomDrawWindow(textBox1);
            hooker.CustomDraw += TextBox1_CustomDraw;
            textBox1.TextChanged += TextBox1_TextChanged;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            var box = (TextBox)sender;
            if (box.TextLength == 0)
            {
                box.Invalidate();
            }
        }

        private const int EM_GETRECT = 0x00B2;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
            public Rectangle Rectangle {
                get {
                    return Rectangle.FromLTRB(Left, Top, Right, Bottom);
                }
            }
        }

        private string PlaceHolderText = "入力してください";

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref RECT lParam);

        private void TextBox1_CustomDraw(object sender, PaintEventArgs e)
        {
            // ネイティブイメージを描画
            var cdw = (CustomDrawWindow)sender;
            var box = (TextBox)cdw.Owner;
            e.Graphics.DrawImage(cdw.NativeClientBitmap, box.ClientRectangle);

            // テキスト表示エリアを取得
            var rect = new RECT();
            SendMessage(box.Handle, EM_GETRECT, IntPtr.Zero, ref rect);

            // 入力可能で未入力ならプレースホルダを表示
            if (box.Enabled && box.TextLength == 0)
            {
                TextRenderer.DrawText(
                    e.Graphics, PlaceHolderText, box.Font,
                    rect.Rectangle, SystemColors.GrayText, 
                    TextFormatFlags.TextBoxControl);
            }
        }
    }
}
