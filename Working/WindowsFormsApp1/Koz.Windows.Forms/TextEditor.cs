using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class TextEditor : TextBox
    {
        private readonly WrapModeController WrapModeController;
        private readonly OwnerDrawController OwnerDrawController;
        private readonly CaretController CaretController;

        public TextEditor() {

            OwnerDrawController = new OwnerDrawController(this);
            WrapModeController = new WrapModeController(this);
            CaretController = new CaretController(this);

            OwnerDrawController.OwnerDraw += DrawController_OwnerDraw;
            WrapModeController.WordBreak += WrapModeController_WordBreak;

            base.Multiline = true;
            base.MaxLength = 0;
            base.ScrollBars = ScrollBars.Both;
            this.WrapMode = WrapModeController.DefaultWrapMode;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void DrawController_OwnerDraw(object sender, PaintEventArgs e) {
            TextEditorRenderer.DrawClient(this, e);
        }

        private void WrapModeController_WordBreak(object sender, WordBreakEventArgs e) {
            OnWordBreak(e);
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {

                case NativeMethods.WM_PASTE:
                case NativeMethods.EM_SETSEL:
                case NativeMethods.WM_HSCROLL:
                case NativeMethods.WM_VSCROLL:
                case NativeMethods.WM_LBUTTONDBLCLK:
                    WndProcWithLock(ref m);
                    break;

                case NativeMethods.WM_KEYDOWN:
                    WmKeyDown(ref m);
                    break;

                case NativeMethods.WM_CHAR:
                    WmChar(ref m);
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
            OwnerDrawController.LockWindow();
            base.WndProc(ref m);
            OwnerDrawController.UnlockWindow();
            Invalidate();
        }

        private void WmKeyDown(ref Message m) {
            long KeyStateMask = 0x40000000;
            bool isRepeat = (m.LParam.ToInt64() & KeyStateMask) == KeyStateMask;
            var clip = GetCaretClip();
            OwnerDrawController.LockWindow();
            base.WndProc(ref m);
            OwnerDrawController.UnlockWindow();
            // リピート中は Refresh そうでなければ Invalidate
            if (isRepeat) {
                Refresh(clip);
            } else {
                Invalidate();
            }
        }

        private void WmChar(ref Message m) {
            if (Focused) {
                Rectangle clip = GetCaretClip();
                HandleRef hwnd = new HandleRef(this, Handle);
                NativeMethods.HideCaret(hwnd);
                OwnerDrawController.LockWindow();
                base.WndProc(ref m);
                OwnerDrawController.UnlockWindow();
                Invalidate(clip, true);
                NativeMethods.ShowCaret(hwnd);
            } else {
                base.WndProc(ref m);
            }
        }

        // キャレットの上下１行をクリップする
        private Rectangle GetCaretClip() {
            NativeMethods.GetCaretPos(out NativeMethods.POINTL pt);
            int currentLine = GetLineFromPosition(new Point(pt.x, pt.y));

            int startLine = currentLine > 0 ? currentLine - 1 : currentLine;
            Point startPt = GetPositionFromLine(startLine);

            int lastIndex = GetLineCount() - 1;
            int endLine = currentLine + 1 > lastIndex ? lastIndex : currentLine + 1;
            Point endPt = GetPositionFromLine(endLine);

            int top = startPt.Y;
            int bottom = endPt.Y + UTL.GetFontSizeAverage(this.Font).Height;
            return Rectangle.FromLTRB(ClientRectangle.Left, top, ClientRectangle.Right, bottom);
        }

        public int GetLineCount() {
            return (int)NativeMethods.SendMessage(new HandleRef(this, Handle),
                                NativeMethods.EM_GETLINECOUNT, IntPtr.Zero, IntPtr.Zero);
        }

        public int GetLineFromPosition(Point pt) {
            int charIndex = GetCharIndexFromPosition(pt);
            return GetLineFromCharIndex(charIndex);
        }

        public Point GetPositionFromLine(int lineIndex) {
            int charIndex = GetFirstCharIndexFromLine(lineIndex);
            return GetPositionFromCharIndex(charIndex);
        }

        public virtual void Refresh(Rectangle clip) {
            Invalidate(clip, true);
            Update();
        }

        public override string Text {
            get {
                return base.Text;
            }
            set { 
                if (base.Text != value) {
                    base.Text = value;
                    if (IsHandleCreated) {
                        Invalidate();
                    }
                }
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

        // 右端を半角２文字分あける
        protected virtual void SetRightMargin() {
            Size avg = UTL.GetFontSizeAverage(Font);
            var rect = new NativeMethods.RECT();
            NativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.EM_GETRECT, IntPtr.Zero, ref rect);
            rect.right = ClientRectangle.Right - avg.Width * 2;
            NativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.EM_SETRECT, IntPtr.Zero, ref rect);
        }

        // -------------------------------------------------------------------------------
        // プロパティ
        // -------------------------------------------------------------------------------

        private static class Category
        {
            public const string Marker = "マーカー";
            public const string Editor = "エディタ拡張";
            public const string Caret = "キャレット";
        }

        private static class ObsoleteMessage
        {
            public const string CannotChangeProperty = "プロパティを変更できません";
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteMessage.CannotChangeProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Multiline { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(ObsoleteMessage.CannotChangeProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool WordWrap { get; set; }

        [DefaultValue(0)]
        public override int MaxLength {
            get {
                return base.MaxLength;
            }
            set {
                base.MaxLength = value;
            }
        }

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
        [DefaultValue(WrapModeController.DefaultWrapMode)]
        [Category(Category.Editor)]
        public WrapMode WrapMode {
            get {
                return WrapModeController.WrapMode;
            }
            set {
                if (WrapModeController.WrapMode != value) {
                    WrapModeController.WrapMode = value;
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
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

        // -------------------------------------------------------------------------------
        // CaretWidth プロパティ
        // -------------------------------------------------------------------------------
        private static readonly object EventCaretWidthChanged = new object();

        /// <summary>
        /// キャレットの幅を取得または設定します。
        /// </summary>
        [Description("キャレットの幅を取得または設定します。")]
        [Category(Category.Caret)]
        [DefaultValue(1)]
        public int CaretWidth {
            get {
                return CaretController.CaretWidth;
            }
            set {
                if (CaretController.CaretWidth != value) {
                    CaretController.CaretWidth = value;
                    if (IsHandleCreated && Focused) {
                        CaretController.ShowCaret();
                    }
                    OnCaretWidthChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// CaretWidthChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnCaretWidthChanged(EventArgs e) {
            EventHandler eh = Events[EventCaretWidthChanged] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <summary>
        /// CaretWidth プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler CaretWidthChanged {
            add {
                Events.AddHandler(EventCaretWidthChanged, value);
            }
            remove {
                Events.RemoveHandler(EventCaretWidthChanged, value);
            }
        }

        // -------------------------------------------------------------------------------
        // CaretColor プロパティ
        // -------------------------------------------------------------------------------
        private object EventCaretColorChanged = new object();

        /// <summary>
        /// キャレットの幅を取得または設定します。
        /// </summary>
        [Description("キャレットの幅を取得または設定します。")]
        [Category(Category.Caret)]
        public Color CaretColor {
            get {
                if (CaretController.CaretColor.IsEmpty) {
                    return CaretController.DefaultCaretColor;
                }
                return CaretController.CaretColor;
            }
            set {
                if (CaretController.CaretColor != value) {
                    CaretController.CaretColor = value;
                    if (IsHandleCreated && Focused) {
                        CaretController.ShowCaret();
                    }
                    OnCaretColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// CaretColorChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnCaretColorChanged(EventArgs e) {
            EventHandler eh = Events[EventCaretColorChanged] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <summary>
        /// CaretColor プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler CaretColorChanged {
            add {
                Events.AddHandler(EventCaretColorChanged, value);
            }
            remove {
                Events.RemoveHandler(EventCaretColorChanged, value);
            }
        }

        /// <summary>
        /// CaretColor の値を既定値に設定します。
        /// </summary>
        internal void ResetCaretColor() {
            CaretController.CaretColor = Color.Empty;
        }

        /// <summary>
        /// CaretColor の値が変更されているかを取得します。
        /// </summary>
        internal bool ShouldSerializeCaretColor() {
            return !CaretController.CaretColor.IsEmpty;
        }

        // -------------------------------------------------------------------------------
        // WordBreak イベント
        // -------------------------------------------------------------------------------
        private object EventWordBreak = new object();

        /// <summary>
        /// WordBreak イベントを発生させます。
        /// </summary>
        protected virtual void OnWordBreak(WordBreakEventArgs e) {
            var eh = Events[EventCaretColorChanged] as EventHandler<WordBreakEventArgs>;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <summary>
        /// TextEditor がワードラップ関数を呼び出したときに発生します。
        /// </summary>
        public event EventHandler<WordBreakEventArgs> WordBreak {
            add {
                Events.AddHandler(EventWordBreak, value);
            }
            remove {
                Events.RemoveHandler(EventWordBreak, value);
            }
        }


    }
}
