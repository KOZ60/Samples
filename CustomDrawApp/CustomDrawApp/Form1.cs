namespace CustomDrawApp
{
    using System.Windows.Forms;
    using System.Drawing;
    using System;
    using System.Linq;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            propertyGrid1.GoToTop();
            propertyGrid2.GoToTop();
        }
    }
}
