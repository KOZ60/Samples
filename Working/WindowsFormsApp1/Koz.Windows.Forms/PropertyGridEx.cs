using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class PropertyGridEx : PropertyGrid
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new VScrollBar VerticalScroll {
            get {
                return FindVScrollBar();
            }
        }

        private VScrollBar FindVScrollBar() {
            return Controls.OfType<Control>()
                .Where(ctl => ctl.AccessibilityObject.Role == AccessibleRole.Table)
                .First().Controls.OfType<VScrollBar>().First();
        }

        protected override void OnSelectedObjectsChanged(EventArgs e) {
            base.OnSelectedObjectsChanged(e);
            GotoTop();
        }

        public virtual void GotoTop() {
            if (SelectedObject != null) {
                var scrollBar = FindVScrollBar();
                if (scrollBar != null) {
                    scrollBar.Value = scrollBar.Minimum;
                }
            }
        }
    }
}
