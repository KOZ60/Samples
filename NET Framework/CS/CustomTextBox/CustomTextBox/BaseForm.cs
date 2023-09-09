using System;
using System.Windows.Forms;

namespace CustomTextBox
{
    public partial class BaseForm : Form
    {
        public BaseForm() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            object obj = propertyGrid1.SelectedObject;
            if (obj != null) {
                Text = obj.GetType().Name;
            }
        }
    }
}
