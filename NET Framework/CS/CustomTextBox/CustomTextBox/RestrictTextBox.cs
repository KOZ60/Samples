using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// TextBox to restrict input
/// </summary>
[DesignerCategory("code")]
public class RestrictTextBox : TextBox
{
    private const char BACKSPACE = '\b';      // Backspace
    private const char CTRL_A = '\x01';       // CTRL+A（Select All）
    private const char CTRL_C = '\x03';       // CTRL+C（Copy）
    private const char CTRL_V = '\x16';       // CTRL+V（Paste）
    private const char CTRL_X = '\x18';       // CTRL+X（Cut）
    private const char CTRL_Z = '\x1A';       // CTRL+Z（Undo）

    private bool canRaiseEvents = true;

    protected override void OnKeyDown(KeyEventArgs e) {
        bool valid = true;
        switch (e.KeyCode) {
            case Keys.Delete:
                GetUnSelectedText(out string left, out string right);
                if (SelectionLength > 0) {
                    valid = IsValidText(left + right);
                } else {
                    if (right.Length > 0) {
                        valid = IsValidText(left + right.Substring(1));
                    }
                }
                break;
        }
        if (!valid) {
            e.Handled = true;
            e.SuppressKeyPress = true;
            return;
        }
        base.OnKeyDown(e);
    }

    protected virtual void GetUnSelectedText(out string left, out string right) {
        string prevText = base.Text;
        left = prevText.Substring(0, SelectionStart);
        right = prevText.Substring(SelectionStart + SelectionLength);
    }

    protected override void OnKeyPress(KeyPressEventArgs e) {
        bool valid = true;
        string left, right;
        switch (e.KeyChar) {
            case BACKSPACE:
                GetUnSelectedText(out left, out right);
                if (SelectionLength > 0) {
                    // selected characters delete
                    valid = IsValidText(left + right);
                } else {
                    if (left.Length > 0) {
                        // right side delete
                        valid = IsValidText(left.Substring(0, left.Length - 1) + right);
                    }
                }
                break;

                // Clipboard Key - Process with window messages
            case CTRL_A:
            case CTRL_C:
            case CTRL_V:
            case CTRL_X:
            case CTRL_Z:
                break;

                // others
            default:
                if (IsValidChar(e.KeyChar)) {
                    GetUnSelectedText(out left, out right);
                    StringBuilder checkText = StringBuilderCache.Acquire();
                    checkText.Append(left);
                    checkText.Append(e.KeyChar);
                    checkText.Append(right);
                    valid = IsValidText(StringBuilderCache.GetStringAndRelease(checkText));
                } else {
                    valid = false;
                }
                break;
        }
        // cancel when has error
        if (!valid) {
            e.Handled = true;
            return;
        }
        base.OnKeyPress(e);
    }

    [DebuggerStepThrough]
    protected override void WndProc(ref Message m) {
        switch (m.Msg) {
            case WM_CUT:
                WmCut(ref m);
                break;

            case WM_CLEAR:
                WmClear(ref m);
                break;

            case WM_COPY:
                base.WndProc(ref m);
                break;

            case WM_PASTE:
                WmPaste(ref m);
                break;

            case WM_UNDO:
            case EM_UNDO:
                WmUndo(ref m);
                break;

            case WM_PAINT:
                WmPaint(ref m);
                break;

            case WM_ERASEBKGND:
                break;

            case WM_PRINTCLIENT:
                WmPrintClient(ref m);
                break;

            default:
                base.WndProc(ref m);
                break;
        }
    }

    protected virtual void WmCut(ref Message m) {
        GetUnSelectedText(out string left, out string right);
        if (IsValidText(left + right)) {
            base.WndProc(ref m);
        }
    }

    protected virtual void WmClear(ref Message m) {
        GetUnSelectedText(out string left, out string right);
        if (IsValidText(left + right)) {
            base.WndProc(ref m);
        }
    }

    protected virtual void WmPaste(ref Message m) {
        if (!Clipboard.ContainsText()) {
            return;
        }
        GetUnSelectedText(out string left, out string right);
        StringBuilder pasteText = StringBuilderCache.Acquire();
        foreach (char c in Clipboard.GetText()) {
            if (IsValidChar(c)) {
                StringBuilder checkText = StringBuilderCache.Acquire();
                checkText.Append(left);
                checkText.Append(pasteText.ToString());
                checkText.Append(c);
                checkText.Append(right);
                if (IsValidText(StringBuilderCache.GetStringAndRelease(checkText))) {
                    pasteText.Append(c);
                }
            }
        }
        if (pasteText.Length > 0) {
            Paste(StringBuilderCache.GetStringAndRelease(pasteText));
        } else {
            StringBuilderCache.Release(pasteText);
        }
        m.Result = IntPtr.Zero;
    }

    protected virtual void WmUndo(ref Message m) {
        LockWindowUpdate(m.HWnd);
        try {
            // Undo, and restore if there is not valid
            string prevText = base.Text;
            base.WndProc(ref m);
            if (!IsValidText(base.Text)) {
                SetTextWithoutEvents(prevText);
            }
        } finally {
            LockWindowUpdate(IntPtr.Zero);
            Invalidate();
        }
    }

    protected override bool CanRaiseEvents {
        get {
            return canRaiseEvents;
        }
    }

    // Text Change No Events
    protected void SetTextWithoutEvents(string value) {
        if (base.Text != value) {
            canRaiseEvents = false;
            try {
                base.Text = value;
            } finally {
                canRaiseEvents = true;
            }
        }
    }

    // check valid character
    protected virtual bool IsValidChar(char keyChar) {
        return true;
    }

    // check valid text
    protected virtual bool IsValidText(string text) {
        return (MaxLength == 0 || text.Length <= MaxLength);
    }

    // un format cursor enters
    protected override void OnEnter(EventArgs e) {
        OnBeforeEnter(e);
        base.OnEnter(e);
    }

    protected virtual void OnBeforeEnter(EventArgs e) {
        SetTextWithoutEvents(UnFormat(base.Text));
        SelectAll();
    }

    // format cursor leaves
    protected override void OnLeave(EventArgs e) {
        OnBeforeLeave(e);
        base.OnLeave(e);
    }

    protected virtual void OnBeforeLeave(EventArgs e) {
        SetTextWithoutEvents(Format(UnFormat(base.Text)));
    }

    // un format

    protected virtual string UnFormat(string value) {
        StringBuilder sb = StringBuilderCache.Acquire();
        foreach (char c in value) {
            if (IsValidChar(c)) {
                sb.Append(c);
            }
        }
        return StringBuilderCache.GetStringAndRelease(sb);
    }

    // format

    protected virtual string Format(string value) {
        return value;
    }

    // Sorry, Microsoft development team. And thx.
    // It's such a great class that I couldn't help but copy it.
    protected class StringBuilderCache
    {
        private const int MAX_BUILDER_SIZE = 360;
        private const int DefaultCapacity = 16;

        [ThreadStatic]
        private static StringBuilder CachedInstance;

        public static StringBuilder Acquire(int capacity = DefaultCapacity) {
            if (capacity <= MAX_BUILDER_SIZE) {
                StringBuilder sb = CachedInstance;
                if (sb != null) {
                    if (capacity <= sb.Capacity) {
                        CachedInstance = null;
                        sb.Clear();
                        return sb;
                    }
                }
            }
            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder sb) {
            if (sb.Capacity <= MAX_BUILDER_SIZE) {
                CachedInstance = sb;
            }
        }

        public static string GetStringAndRelease(StringBuilder sb) {
            string result = sb.ToString();
            Release(sb);
            return result;
        }
    }

    private BufferedGraphicsContext BufferContext {
        get {
            return BufferedGraphicsManager.Current;
        }
    }

    // userpaint

    protected virtual void WmPaint(ref Message m) {
        if (m.WParam == IntPtr.Zero) {
            PAINTSTRUCT ps = new PAINTSTRUCT();
            IntPtr hdc = BeginPaint(m.HWnd, ref ps);
            var clip = ps.rcPaint.Rectangle;
            if (clip.Width > 0 && clip.Height > 0) {
                IntPtr hOldPal = SetUpPalette(hdc, false, false);
                try {
                    using (var bufferedGraphics = BufferContext.Allocate(hdc, ClientRectangle)) {
                        bufferedGraphics.Graphics.SetClip(clip);
                        using (var pe = new PaintEventArgs(bufferedGraphics.Graphics, clip)) {
                            OnCustomDraw(pe);
                        }
                        bufferedGraphics.Render();
                    }
                } finally {
                    if (hOldPal != IntPtr.Zero) {
                        SelectPalette(hdc, hOldPal, 0);
                    }
                }
            }
            EndPaint(m.HWnd, ref ps);
        } else {
            using (var g = Graphics.FromHdc(m.WParam)) {
                using (var pe = new PaintEventArgs(g, ClientRectangle)) {
                    OnCustomDraw(pe);
                }
            }
        }
    }

    private static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette) {
        IntPtr halftonePalette = Graphics.GetHalftonePalette();
        IntPtr result = SelectPalette(dc, halftonePalette, (force ? 0 : 1));
        if (result != IntPtr.Zero && realizePalette) {
            RealizePalette(dc);
        }
        return result;
    }

    protected virtual void WmPrintClient(ref Message m) {
        using (Graphics g = Graphics.FromHdc(m.WParam)) {
            using (PaintEventArgs e =
                    new PaintEventArgs(g, ClientRectangle)) {
                OnPrint(e);
            }
        }
    }

    private enum PaintLayer : short
    {
        Background,
        Foreground,
    }

    protected override void OnPrint(PaintEventArgs e) {
        OnCustomDraw(e);
    }

    protected void OnCustomDraw(PaintEventArgs e) {
        GraphicsState state = e.Graphics.Save();
        OnCustomDraw(e, PaintLayer.Background);
        e.Graphics.Restore(state);
        OnCustomDraw(e, PaintLayer.Foreground);
    }

    private void OnCustomDraw(PaintEventArgs e, PaintLayer layer) {
        Message m;
        IntPtr hdc = e.Graphics.GetHdc();
        try {
            switch (layer) {
                case PaintLayer.Foreground:
                    m = Message.Create(Handle, WM_PAINT, hdc, IntPtr.Zero);
                    DefWndProc(ref m);
                    break;
                case PaintLayer.Background:
                    m = Message.Create(Handle, WM_ERASEBKGND, hdc, IntPtr.Zero);
                    DefWndProc(ref m);
                    break;
                default:
                    Debug.Fail("Unknown PaintLayer " + layer);
                    break;
            }
        } finally {
            e.Graphics.ReleaseHdc(hdc);
        }

        try {
            switch (layer) {
                case PaintLayer.Foreground:
                    OnPaint(e);
                    break;
                case PaintLayer.Background:
                    OnPaintBackground(e);
                    break;
            }
        } finally {

        }
    }

    // P.Invoke

    private static class ExternDll
    {
        public const string Kernel32 = "kernel32.dll";
        public const string User32 = "user32.dll";
        public const string Gdi32 = "gdi32.dll";
    }

    private const int
        WM_PAINT = 0x000F,
        WM_ERASEBKGND = 0x0014,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CLEAR = 0x0303,
        WM_UNDO = 0x0304,
        WM_PRINTCLIENT = 0x0318;

    private const int
        EM_UNDO = 0x00C7;

    [StructLayout(LayoutKind.Sequential)]
    private struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public RECT rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        public int reserved1;
        public int reserved2;
        public int reserved3;
        public int reserved4;
        public int reserved5;
        public int reserved6;
        public int reserved7;
        public int reserved8;
    }
    [DllImport(ExternDll.User32)]
    private static extern bool LockWindowUpdate(IntPtr hWndLock);

    [DllImport(ExternDll.User32)]
    private static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

    [DllImport(ExternDll.User32)]
    private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

    [DllImport(ExternDll.Gdi32)]
    private static extern IntPtr SelectPalette(IntPtr hdc, IntPtr hpal, int bForceBackground);

    [DllImport(ExternDll.Gdi32)]
    private static extern int RealizePalette(IntPtr hDC);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(Rectangle r) {
            Left = r.Left;
            Top = r.Top;
            Right = r.Right;
            Bottom = r.Bottom;
        }

        public override string ToString() {
            return Rectangle.ToString();
        }

        public int Width {
            get { return Right - Left; }
        }

        public int Height {
            get { return Bottom - Top; }
        }

        public Rectangle Rectangle {
            get { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }
        }
    }

}
