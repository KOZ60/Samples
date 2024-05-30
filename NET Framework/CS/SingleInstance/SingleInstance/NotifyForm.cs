using System.Windows.Forms;
using SingleInstance;

[System.ComponentModel.DesignerCategory("Code")]
internal class NotifyForm : Form
{
    public NotifyForm() {
        Text = NativeMethods.GUID;
    }

    protected override void SetVisibleCore(bool value) {
        if (!IsHandleCreated) {
            CreateHandle();
        }
        base.SetVisibleCore(false);
    }

    protected override void WndProc(ref Message m) {
        base.WndProc(ref m);
        // 終了要求を受信したら終了します。
        if (m.Msg == NativeMethods.WM_APPLICATION_EXIT) {
            Application.Exit();
        }
    }
}
