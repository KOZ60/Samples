using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Globalization;
using Debug = System.Diagnostics.Debug;

namespace WindowsFormsApp1
{
    public class TextEditor : TextBox
    {
        private readonly WordBreakController wbController;

        public TextEditor() {
            wbController = new WordBreakController(this);
            base.Multiline = true;
            base.ScrollBars = ScrollBars.Both;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private int lockCount = 0;

        protected int LockWindow() {
            if (lockCount == 0) {
                NativeMethods.LockWindowUpdate(new HandleRef(this, Handle));
            }
            lockCount += 1;
            return lockCount;
        }

        protected int UnlockWindow() {
            lockCount -= 1;
            if (lockCount <= 0) {
                NativeMethods.LockWindowUpdate(new HandleRef(this, IntPtr.Zero));
                lockCount = 0;
            }
            return lockCount;
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_PAINT:
                    WmPaint(ref m);
                    break;

                case NativeMethods.WM_PRINTCLIENT:
                    WmPrintClient(ref m);
                    break;

                case NativeMethods.WM_ERASEBKGND:
                    break;

                case NativeMethods.WM_KEYDOWN:
                    WmKeyDown(ref m);
                    break;

                case NativeMethods.WM_PASTE:
                case NativeMethods.EM_SETSEL:
                case NativeMethods.WM_HSCROLL:
                case NativeMethods.WM_VSCROLL:
                    WndProcWithLock(ref m);
                    break;

                default:
                    base.WndProc(ref m);
                    // マウス操作があったら Invalidate
                    if (m.Msg >= NativeMethods.WM_MOUSEFIRST && 
                        m.Msg <= NativeMethods.WM_MOUSELAST) {
                        Invalidate();
                    }
                    break;
            }
        }

        // 描画ロックして Invalidate
        private void WndProcWithLock(ref Message m) {
            LockWindow();
            base.WndProc(ref m);
            UnlockWindow();
            Invalidate();
        }

        private void WmKeyDown(ref Message m) {
            long KeyStateMask = 0x40000000;
            bool isRepeat = (m.LParam.ToInt64() & KeyStateMask) == KeyStateMask;
            LockWindow();
            base.WndProc(ref m);
            UnlockWindow();
            // リピート中は Refresh そうでなければ Invalidate
            if (isRepeat) {
                Refresh();
            } else {
                Invalidate();
            }
        }

        private void WmPaint(ref Message m) {
            if (m.WParam == IntPtr.Zero) {
                var ps = new NativeMethods.PAINTSTRUCT();
                IntPtr hdc = NativeMethods.BeginPaint(new HandleRef(this, m.HWnd), ref ps);
                var clip = ps.rcPaint.Rectangle;
                if (clip.Width > 0 && clip.Height > 0) {
                    IntPtr oldPal = IntPtr.Zero;
                    BufferedGraphics bufferedGraphics = null;
                    try {
                        oldPal = NativeMethods.SetUpPalette(hdc, false, false);
                        var bufferContext = BufferedGraphicsManager.Current;
                        bufferedGraphics = bufferContext.Allocate(hdc, ClientRectangle);
                        bufferedGraphics.Graphics.SetClip(clip);
                        using (var e = new PaintEventArgs(bufferedGraphics.Graphics, clip)) {
                            OnCustomDraw(e);
                        }
                        bufferedGraphics.Render();

                    } finally {
                        if (oldPal != IntPtr.Zero) {
                            NativeMethods.SelectPalette(new HandleRef(this, hdc), new HandleRef(null, oldPal), 0);
                        }
                        if (bufferedGraphics != null) {
                            bufferedGraphics.Dispose();
                        }
                    }
                }
                NativeMethods.EndPaint(new HandleRef(this, m.HWnd), ref ps);
            } else {
                using (var g = Graphics.FromHdcInternal(m.WParam)) {
                    using (var e = new PaintEventArgs(g, ClientRectangle)) {
                        OnCustomDraw(e);
                    }
                }
            }
        }

        private void WmPrintClient(ref Message m) {
            using (var g = Graphics.FromHdcInternal(m.WParam)) {
                using (var e = new PaintEventArgs(g, ClientRectangle)) {
                    OnCustomDraw(e);
                }
            }
        }

        protected virtual void DrawNative(PaintEventArgs e) {
            e.Graphics.Clear(BackColor);
            IntPtr hdc = e.Graphics.GetHdc();
            var m = Message.Create(this.Handle, NativeMethods.WM_PAINT, hdc, IntPtr.Zero);
            base.DefWndProc(ref m);
            e.Graphics.ReleaseHdc();
        }

        protected virtual void OnCustomDraw(PaintEventArgs e) {
            TextEditorRenderer.DrawClient(this, e.Graphics);
        }

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            Invalidate();
        }

        // -------------------------------------------------------------------------------
        // プロパティ
        // -------------------------------------------------------------------------------

        private static class Category
        {
            public const string Marker = "マーカー";
            public const string Editor = "エディタ拡張";
        }

        private static class ObsoleteMessage
        {
            public const string CannotChangeProperty = "プロパティを変更できません";
        }

        [DefaultValue(true)]
        public override bool Multiline { 
            get {
                return base.Multiline;
            }
            set {
                base.Multiline = value;
            }
        }

        [Browsable(false)]
        [Obsolete(ObsoleteMessage.CannotChangeProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool WordWrap { get; set; }

        [DefaultValue(ScrollBars.Both)]
        public new ScrollBars ScrollBars {
            get {
                return base.ScrollBars;
            }
            set {
                base.ScrollBars = value;
            }
        }

        private Color _BackColor = Color.Empty;

        public override Color BackColor {
            get {
                if (_BackColor.IsEmpty) {
                    return SystemColors.Window;
                }
                return base.BackColor;
            }
            set {
                base.BackColor = value;
                _BackColor = value;
            }
        }

        // -------------------------------------------------------------------------------
        // MarkerColor プロパティ
        // -------------------------------------------------------------------------------
        private static readonly object EventMarkerColorChanged = new object();
        public readonly Color DefaultMarkerColor = Color.Green;
        private Color _MarkerColor = Color.Empty;

        /// <summary>
        /// マーカーの色を取得または設定します。
        /// </summary>
        [Description("マーカーの色を取得または設定します。")]
        [Category(Category.Marker)]
        public Color MarkerColor {
            get {
                if (_MarkerColor.IsEmpty) {
                    return DefaultMarkerColor;
                }
                return _MarkerColor;
            }
            set {
                if (MarkerColor != value) {
                    _MarkerColor = value;
                    if (IsHandleCreated) {
                        Invalidate();
                    }
                    OnMarkerColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// MarkerColorChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnMarkerColorChanged(EventArgs e) {
            EventHandler eh = Events[EventMarkerColorChanged] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <summary>
        /// MarkerColor プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler MarkerColorChanged {
            add {
                Events.AddHandler(EventMarkerColorChanged, value);
            }
            remove {
                Events.RemoveHandler(EventMarkerColorChanged, value);
            }
        }

        /// <summary>
        /// MarkerColor の値を既定値に設定します。
        /// </summary>
        internal void ResetMarkerColor() {
            MarkerColor = Color.Empty;
        }

        /// <summary>
        /// MarkerColor の値が変更されているかを取得します。
        /// </summary>
        internal bool ShouldSerializeMarkerColor() {
            return _MarkerColor != Color.Empty;
        }

        // -------------------------------------------------------------------------------
        // ShowMarker プロパティ
        // -------------------------------------------------------------------------------
        private static readonly object EventShowMarkerChanged = new object();
        private bool _ShowMarker = true;

        /// <summary>
        /// マーカーを表示するかどうかを取得または設定します。
        /// </summary>
        [Description("マーカーを表示するかどうかを取得または設定します。")]
        [Category(Category.Marker)]
        [DefaultValue(true)]
        public bool ShowMarker {
            get {
                return _ShowMarker;
            }
            set {
                if (ShowMarker != value) {
                    _ShowMarker = value;
                    if (IsHandleCreated) {
                        Invalidate();
                    }
                    OnShowMarkerChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// ShowMarkerChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnShowMarkerChanged(EventArgs e) {
            EventHandler eh = Events[EventShowMarkerChanged] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <summary>
        /// ShowMarker プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler ShowMarkerChanged {
            add {
                Events.AddHandler(EventShowMarkerChanged, value);
            }
            remove {
                Events.RemoveHandler(EventShowMarkerChanged, value);
            }
        }

        // -------------------------------------------------------------------------------
        // TabWidth プロパティ
        // -------------------------------------------------------------------------------
        private static readonly object EventTabWidthChanged = new object();
        private int _TabWidth = 4;

        /// <summary>
        /// タブ幅(文字数)を取得または設定します。
        /// </summary>
        [Description("タブ幅(文字数)を取得または設定します。")]
        [Category(Category.Editor)]
        [DefaultValue(4)]
        public int TabWidth {
            get {
                return _TabWidth;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException();
                }
                if (TabWidth != value) {
                    _TabWidth = value;
                    if (IsHandleCreated) {
                        SetTabStop();
                    }
                    OnTabWidthChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// TabWidthChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnTabWidthChanged(EventArgs e) {
            EventHandler eh = Events[EventTabWidthChanged] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <summary>
        /// TabWidth プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler TabWidthChanged {
            add {
                Events.AddHandler(EventTabWidthChanged, value);
            }
            remove {
                Events.RemoveHandler(EventTabWidthChanged, value);
            }
        }

        // -------------------------------------------------------------------------------
        // WrapMode プロパティ
        // -------------------------------------------------------------------------------
        private static readonly object EventWrapModeChanged = new object();

        /// <summary>
        /// 複数行の場合の折り返しルールを取得または設定します。
        /// </summary>
        [Description("複数行の場合の折り返しルールを取得または設定します。")]
        [DefaultValue(WordBreakController.DefaultWrapMode)]
        [Category(Category.Editor)]
        public WrapMode WrapMode {
            get {
                return wbController.WrapMode;
            }
            set {
                if (wbController.WrapMode != value) {
                    wbController.WrapMode = value;
                    OnWrapModeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// WrapModeChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnWrapModeChanged(EventArgs e) {
            EventHandler eh = Events[EventWrapModeChanged] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <summary>
        /// WrapMode プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler WrapModeChanged {
            add {
                Events.AddHandler(EventWrapModeChanged, value);
            }
            remove {
                Events.RemoveHandler(EventWrapModeChanged, value);
            }
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            SetTabStop();
        }

        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            SetTabStop();
        }

        // Tab 幅をセットする
        protected void SetTabStop() {
            if (IsHandleCreated) {

                NativeMethods.SendMessage(new HandleRef(this, Handle),
                        NativeMethods.EM_SETTABSTOPS, (IntPtr)1, new int[] { TabWidth * 4 });

                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            SetRightMargin();
        }

        protected virtual void SetRightMargin() {
            // 右端を半角２文字分あける
            Size avgFontSize = TextEditorRenderer.GetAverageFontSize(this);
            var rect = new NativeMethods.RECT();
            NativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.EM_GETRECT, IntPtr.Zero, ref rect);
            rect.right = ClientRectangle.Right - avgFontSize.Width * 2;
            NativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.EM_SETRECT, IntPtr.Zero, ref rect);
        }
    }
}
