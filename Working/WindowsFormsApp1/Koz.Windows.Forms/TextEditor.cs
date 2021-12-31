using System;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class TextEditor : Control, IWrapModeControl
    {
        private const int INITIAL_MEMORY_SIZE = 32767;

        private static class MessageText
        {
            public const string EN_ERRSPACE = "重大なエラー：メモリを割り当てることができません。";
        }

        private static class MessageTitle
        {
            public const string ERROR = "エラー";
        }

        private WrapModeController<TextEditor> WrapModeController;

        public TextEditor() {

            WrapModeController = new WrapModeController<TextEditor>(this);

            SetStyle(ControlStyles.StandardClick
                            | ControlStyles.StandardDoubleClick
                            | ControlStyles.UseTextForAccessibility, false);

            SetStyle(ControlStyles.ResizeRedraw
                            | ControlStyles.UserPaint
                            | ControlStyles.OptimizedDoubleBuffer
                            | ControlStyles.AllPaintingInWmPaint, true);

        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            fontHandleWrapper?.Dispose();
            caretBitmap?.Dispose();
        }

        protected override CreateParams CreateParams {
            get {
                CreateParams cp = base.CreateParams;

                // TextBoxBase
                cp.ClassName = "EDIT";
                cp.Style |= NativeMethods.ES_AUTOHSCROLL | NativeMethods.ES_AUTOVSCROLL;
                cp.ExStyle &= (~NativeMethods.WS_EX_CLIENTEDGE);
                cp.Style &= (~NativeMethods.WS_BORDER);

                switch (BorderStyle) {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }

                cp.Style |= NativeMethods.ES_MULTILINE;
                if (WrapMode != WrapMode.NoWrap) {
                    cp.Style &= ~NativeMethods.ES_AUTOHSCROLL;
                }

                // TextBox
                cp.ExStyle &= ~NativeMethods.WS_EX_RIGHT;
                cp.Style |= NativeMethods.ES_LEFT;
                if (WrapMode == WrapMode.NoWrap) {
                    cp.Style |= NativeMethods.WS_HSCROLL;
                }
                cp.Style |= NativeMethods.WS_VSCROLL;

                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            ReAllocMemory(0, false);
            SetWindowFont();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            InitializeHandle(Handle);
        }

        protected virtual void InitializeHandle(IntPtr handle) {
            HandleRef hwnd = new HandleRef(this, handle);

            // 長さを無制限に
            SendMessage(NativeMethods.EM_LIMITTEXT);

            // Tab 幅をセットする
            NativeMethods.SendMessage(hwnd, NativeMethods.EM_SETTABSTOPS, (IntPtr)1, new int[] { TabWidth * 4 });

            // 両端のマージンをゼロに
            IntPtr wp = new IntPtr(NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN);
            SendMessage(NativeMethods.EM_SETMARGINS, wp, IntPtr.Zero);

            // 右端を半角２文字分あける
            var rect = new NativeMethods.RECT();
            NativeMethods.SendMessage(hwnd, NativeMethods.EM_GETRECT, IntPtr.Zero, ref rect);

            // 右を半角２文字分空ける(↓を表示するため)
            rect.right = ClientRectangle.Right - FontAverageSize.Width * 2;
            NativeMethods.SendMessage(hwnd, NativeMethods.EM_SETRECT, IntPtr.Zero, ref rect);
        }

        protected virtual void ReAllocMemory(int stringLength, bool force) {
            int nSizeNew = stringLength * 4;

            if (nSizeNew < INITIAL_MEMORY_SIZE) {
                nSizeNew = INITIAL_MEMORY_SIZE;
            }

            IntPtr hMemOld = SendMessage(NativeMethods.EM_GETHANDLE);

            int nSizeOld = (int)NativeMethods.LocalSize(hMemOld);
            if (!force && nSizeNew < nSizeOld) {
                return;
            }

            // メモリ差し替えの前に選択状態を取得
            GetSelect(out int selectStart, out int selectEnd);

            int flags = NativeMethods.LocalFlags(hMemOld);

            IntPtr hMemNew = NativeMethods.LocalAlloc(flags, new IntPtr(nSizeNew));
            if (hMemNew == IntPtr.Zero) {
                return;
            }

            IntPtr ptrOld = NativeMethods.LocalLock(hMemOld);
            IntPtr ptrNew = NativeMethods.LocalLock(hMemNew);

            int copySize = Math.Min(nSizeOld, nSizeNew);
            NativeMethods.RtlMoveMemory(ptrNew, ptrOld, copySize);

            NativeMethods.LocalUnlock(hMemOld);
            NativeMethods.LocalUnlock(hMemNew);

            SendMessage(NativeMethods.EM_SETHANDLE, hMemNew, IntPtr.Zero);

            NativeMethods.LocalFree(hMemOld);

            // 選択状態を戻す
            SetSelect(selectStart, selectEnd);
        }

        protected override void OnPaint(PaintEventArgs e) {
            TextEditorRenderer.DrawClient(this, e);
            base.OnPaint(e);
        }

        private FontHandleWrapper fontHandleWrapper;

        internal protected FontHandleWrapper FontHandleWrapper {
            get {
                if (fontHandleWrapper == null) {
                    fontHandleWrapper = new FontHandleWrapper(Font);
                } else if (!fontHandleWrapper.IsEquals(Font)) {
                    fontHandleWrapper.Dispose();
                    fontHandleWrapper = new FontHandleWrapper(Font);
                }
                return fontHandleWrapper;
            }
        }

        public Size FontAverageSize {
            get {
                return FontHandleWrapper.AverageSize;
            }
        }

        protected void SetWindowFont() {
            SendMessage(NativeMethods.WM_SETFONT, FontHandleWrapper.Handle, IntPtr.Zero);
        }

        protected override void OnFontChanged(EventArgs e) {
            if (IsHandleCreated) {
                SetWindowFont();
                if (Focused) {
                    ShowCaret();
                }
            }
            base.OnFontChanged(e);
        }

        CaretBitmap caretBitmap;

        protected CaretBitmap CaretBitmap {
            get {
                Color caretColor = _CaretColor;
                Size caretSize = new Size(CaretWidth, FontAverageSize.Height);
                if (caretBitmap == null) {
                    caretBitmap = new CaretBitmap(caretColor, caretSize);
                } else if (caretBitmap.Color != caretColor || caretBitmap.Size != caretSize) {
                    caretBitmap.Dispose();
                    caretBitmap = new CaretBitmap(caretColor, caretSize);
                }
                return caretBitmap;
            }
        }

        protected virtual void ShowCaret() {
            if (DesignMode) return;
            HandleRef hwnd = new HandleRef(this, Handle);
            NativeMethods.DestroyCaret();
            NativeMethods.CreateCaret(hwnd, CaretBitmap, CaretBitmap.Width, CaretBitmap.Height);
            NativeMethods.ShowCaret(hwnd);
        }

        protected override void WndProc(ref Message m) {

            switch (m.Msg) {

                case NativeMethods.EM_SETSEL:
                case NativeMethods.WM_HSCROLL:
                case NativeMethods.WM_VSCROLL:
                case NativeMethods.WM_LBUTTONDBLCLK:
                case NativeMethods.WM_SETTEXT:
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_LBUTTONDOWN:
                    LockWndProc(ref m);
                    break;

                case NativeMethods.WM_MOUSEMOVE:
                    if (MouseIsDown) {
                        LockBegin();
                    }
                    base.WndProc(ref m);
                    break;

                case NativeMethods.WM_CHAR:
                    WmChar(ref m);
                    break;

                case NativeMethods.WM_SETFOCUS:
                    WmSetFocus(ref m);
                    break;

                case NativeMethods.WM_SETFONT:
                    WmSetFont(ref m);
                    break;

                case NativeMethods.WM_REFLECT + NativeMethods.WM_COMMAND:
                    WmReflectCommand(ref m);
                    break;

                case NativeMethods.WM_GETDLGCODE:
                    WmGetDlgCode(ref m);
                    break;

                case NativeMethods.WM_SIZE:
                    base.WndProc(ref m);
                    InitializeHandle(m.HWnd);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        int locCount = 0;

        protected void LockBegin() {
            if (locCount == 0) {
                locCount++;
                NativeMethods.LockWindowUpdate(new HandleRef(this, Handle));
                BeginInvoke((Action)LockEnd);
            }
        }

        private void LockEnd() {
            LockEnd(null);
        }

        private void LockEnd(Rectangle? clip) {
            if (locCount > 0) {
                locCount = 0;
                NativeMethods.LockWindowUpdate(new HandleRef(this, IntPtr.Zero));
                if (clip.HasValue) {
                    Refresh(clip.Value);
                } else {
                    Refresh();
                }
                if (Focused) {
                    NativeMethods.HideCaret(new HandleRef(this, Handle));
                    NativeMethods.ShowCaret(new HandleRef(this, Handle));
                }
            }
        }


        bool mouseIsOver = false;

        protected bool MouseIsOver {
            get {
                return mouseIsOver;
            }
            set {
                if (mouseIsOver != value) {
                    mouseIsOver = value;
                    Invalidate();
                }
            }
        }

        bool mouseIsDown = false;

        protected bool MouseIsDown {
            get {
                return mouseIsDown;
            }
            set {
                if (mouseIsDown != value) {
                    mouseIsDown = value;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) {
                MouseIsDown = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent) {
            base.OnMouseUp(mevent);
            if (mevent.Button == MouseButtons.Left) {
                MouseIsDown = false;
            }
        }

        protected override void OnMouseCaptureChanged(EventArgs e) {
            base.OnMouseCaptureChanged(e);
            MouseIsDown = false;
            MouseIsOver = false;
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            MouseIsOver = true;
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            MouseIsOver = false;
        }

        private void WmSetFocus(ref Message m) {
            base.WndProc(ref m);
            ShowCaret();
        }

        private void WmReflectCommand(ref Message m) {
            switch (NativeMethods.Util.HIWORD(m.WParam)) {

                case NativeMethods.EN_CHANGE:
                    OnTextChanged(EventArgs.Empty);
                    break;

                case NativeMethods.EN_UPDATE:
                    break;

                case NativeMethods.EN_ERRSPACE:
                    MessageBox.Show(MessageText.EN_ERRSPACE,
                                    MessageTitle.ERROR,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    break;

                default:
                    break;
            }
        }

        private void WmGetDlgCode(ref Message m) {
            base.WndProc(ref m);
            m.Result = (IntPtr)(unchecked((int)(long)m.Result) | NativeMethods.DLGC_WANTTAB);
        }


        // キャレットの上下１行をクリップする
        private Rectangle GetCaretClip() {
            NativeMethods.GetCaretPos(out NativeMethods.POINTL pt);
            int currentLine = GetLineFromPosition(new Point(pt.x, pt.y));

            int startLine = currentLine > 0 ? currentLine - 1 : currentLine;
            Point? startPt = GetFirstCharPositionFromLine(startLine);
            int top = startPt.HasValue ? startPt.Value.Y : 0;

            int lastIndex = GetLineCount() - 1;
            int endLine = currentLine + 1 > lastIndex ? lastIndex : currentLine + 1;
            Point? endPt = GetFirstCharPositionFromLine(endLine);
            int bottom = (endPt.HasValue ? endPt.Value.Y : 0);
            bottom += FontAverageSize.Height;
            return Rectangle.FromLTRB(ClientRectangle.Left, top, ClientRectangle.Right, bottom);
        }

        private void WmChar(ref Message m) {
            GetSelect(out int start, out int end);
            if (end - start > 0) {
                LockWndProc(ref m);
            } else {
                Rectangle clip = GetCaretClip();
                LockWndProc(ref m);
                char c = (char)m.WParam.ToInt32();
                if (TextEditorRenderer.Markers.ContainsKey(c)) {
                    LockEnd(clip);
                }
            }
        }

        private void LockWndProc(ref Message m) {
            LockBegin();
            base.WndProc(ref m);
        }

        private void WmSetFont(ref Message m) {
            base.WndProc(ref m);
            InitializeHandle(m.HWnd);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            bool returnValue = base.ProcessCmdKey(ref msg, keyData);
            if (keyData == (Keys.Control | Keys.A)) {
                SelectAll();
                return true;
            }
            return returnValue;
        }

        public void SelectAll() {
            Select(0, TextLength);
        }

        public void Select(int start, int length) {
            int end = start + length;
            SetSelect(start, end);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int TextLength {
            get {
                if (IsHandleCreated && Marshal.SystemDefaultCharSize == 2) {
                    return NativeMethods.GetWindowTextLength(new HandleRef(this, Handle));
                } else {
                    return Text.Length;
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionStart {
            get {
                GetSelect(out int start, out int end);
                return start;
            }
            set {
                Select(value, SelectionLength);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength {
            get {
                GetSelect(out int start, out int end);
                return end - start;
            }
            set {
                Select(SelectionStart, value);
            }
        }

        internal protected void SetSelect(int start, int end) {
            SendMessage(NativeMethods.EM_SETSEL, (IntPtr)start, (IntPtr)end);
        }

        internal protected void GetSelect(out int start, out int end) {
            NativeMethods.SendMessage(
                             new HandleRef(this, Handle),
                             NativeMethods.EM_GETSEL,
                             out start, out end);
        }

        public void ClearUndo() {
            SendMessage(NativeMethods.EM_EMPTYUNDOBUFFER);
        }

        public void Cut() {
            SendMessage(NativeMethods.WM_CUT);
        }

        public void Copy() {
            SendMessage(NativeMethods.WM_COPY);
        }

        public void Paste() {
            SendMessage(NativeMethods.WM_PASTE);
        }

        public void Undo() {
            SendMessage(NativeMethods.EM_UNDO);
        }

        public void Paste(string text) {
            SetSelectedTextInternal(text, false);
        }

        protected virtual void SetSelectedTextInternal(string text, bool clearUndo) {

            if (text == null) {
                text = "";
            }

            if (clearUndo) {
                SendMessage(NativeMethods.EM_REPLACESEL, IntPtr.Zero, text);
                SendMessage(NativeMethods.EM_SETMODIFY);
                ClearUndo();
            } else {
                SendMessage(NativeMethods.EM_REPLACESEL, new IntPtr(-1), text);
            }
        }

        protected IntPtr SendMessage(int msg) {
            return SendMessage(msg, IntPtr.Zero, IntPtr.Zero);
        }

        protected IntPtr SendMessage(int msg, int wParam) {
            return SendMessage(msg, new IntPtr(wParam), IntPtr.Zero);
        }

        protected IntPtr SendMessage(int msg, IntPtr wParam, IntPtr lParam) {
            return NativeMethods.SendMessage(
                            new HandleRef(this, Handle),
                            msg, wParam, lParam);
        }

        protected IntPtr SendMessage(int msg, IntPtr wParam, string lParam) {
            return NativeMethods.SendMessage(
                            new HandleRef(this, Handle),
                            msg, wParam, lParam);
        }

        protected int SendMessageInt(int msg) {
            return SendMessageInt(msg, IntPtr.Zero, IntPtr.Zero);
        }

        protected int SendMessageInt(int msg, int wParam) {
            return SendMessageInt(msg, new IntPtr(wParam), IntPtr.Zero);
        }

        protected int SendMessageInt(int msg, IntPtr wParam, IntPtr lParam) {
            return unchecked((int)(long)SendMessage(msg, wParam, lParam));
        }

        protected int SendMessageInt(int msg, IntPtr wParam, string lParam) {
            return unchecked((int)(long)SendMessage(msg, wParam, lParam));
        }

        public virtual int GetLineCount() {
            return SendMessageInt(NativeMethods.EM_GETLINECOUNT);
        }

        public virtual int GetFirstVisibleLine() {
            return SendMessageInt(NativeMethods.EM_GETFIRSTVISIBLELINE);
        }

        public virtual int GetLineLength(int lineIndex) {
            int charIndex = GetFirstCharIndexFromLine(lineIndex);
            return SendMessageInt(NativeMethods.EM_LINELENGTH, charIndex);
        }

        public virtual int GetFirstCharIndexFromLine(int lineIndex) {
            return SendMessageInt(NativeMethods.EM_LINEINDEX, lineIndex);
        }

        public virtual int GetLineFromPosition(Point pt) {
            int charIndex = GetCharIndexFromPosition(pt);
            return GetLineFromCharIndex(charIndex);
        }

        public virtual int GetLineFromCharIndex(int charIndex) {
            return SendMessageInt(NativeMethods.EM_LINEFROMCHAR, charIndex);
        }

        public Point? GetFirstCharPositionFromLine(int lineIndex) {
            int charIndex = GetFirstCharIndexFromLine(lineIndex);
            return GetPositionFromCharIndex(charIndex);
        }

        public virtual Point? GetPositionFromCharIndex(int charIndex) {
            IntPtr pt = SendMessage(NativeMethods.EM_POSFROMCHAR, charIndex);
            if (pt == NativeMethods.INVALID_HANDLE_VALUE) {
                return null;
            }
            return new Point(NativeMethods.Util.SignedLOWORD(pt), 
                            NativeMethods.Util.SignedHIWORD(pt));
        }

        public virtual int GetCharIndexFromPosition(Point pt) {
            int longPoint = NativeMethods.Util.MAKELONG(pt.X, pt.Y);
            return SendMessageInt(NativeMethods.EM_CHARFROMPOS, IntPtr.Zero, new IntPtr(longPoint));
        }

        public virtual void Refresh(Rectangle clip) {
            Invalidate(clip, true);
            Update();
        }

        // -------------------------------------------------------------------------------
        // プロパティ
        // -------------------------------------------------------------------------------

        private static class Category
        {
            public const string Marker = "マーカー設定";
            public const string Editor = "エディタ拡張";
            public const string Caret = "キャレット設定";
        }

        private static class ObsoleteMessage
        {
            public const string CannotChangeProperty = "プロパティを変更できません";
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

        public override void ResetBackColor() {
            BackColor = Color.Empty;
        }

        protected override Cursor DefaultCursor {
            get {
                return Cursors.IBeam;
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
                        InitializeHandle(Handle);
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
        public const WrapMode DefaultWrapMode = WrapMode.NoWrap;
        private static readonly object EventWrapModeChanged = new object();
        private WrapMode _WrapMode = DefaultWrapMode;

        /// <summary>
        /// 複数行の場合の折り返しルールを取得または設定します。
        /// </summary>
        [Description("複数行の場合の折り返しルールを取得または設定します。")]
        [DefaultValue(DefaultWrapMode)]
        [Category(Category.Editor)]
        public WrapMode WrapMode {
            get {
                return _WrapMode;
            }
            set {
                if (_WrapMode != value) {
                    _WrapMode = value;
                    if (IsHandleCreated) {
                        RecreateHandle();
                        Invalidate();
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
        private int _CaretWidth = 2;

        /// <summary>
        /// キャレットの幅を取得または設定します。
        /// </summary>
        [Description("キャレットの幅を取得または設定します。")]
        [Category(Category.Caret)]
        [DefaultValue(2)]
        public int CaretWidth {
            get {
                return _CaretWidth;
            }
            set {
                if (_CaretWidth != value) {
                    _CaretWidth = value;
                    if (IsHandleCreated && Focused) {
                        ShowCaret();
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
        private Color DefaultCaretColor = Color.Black;
        private Color _CaretColor = Color.Empty;

        /// <summary>
        /// キャレットの幅を取得または設定します。
        /// </summary>
        [Description("キャレットの幅を取得または設定します。")]
        [Category(Category.Caret)]
        public Color CaretColor {
            get {
                if (_CaretColor.IsEmpty) {
                    return DefaultCaretColor;
                }
                return _CaretColor;
            }
            set {
                if (_CaretColor != value) {
                    _CaretColor = value;
                    if (IsHandleCreated && Focused) {
                        ShowCaret();
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
        public virtual void ResetCaretColor() {
            CaretColor = Color.Empty;
        }

        /// <summary>
        /// CaretColor の値が変更されているかを取得します。
        /// </summary>
        internal bool ShouldSerializeCaretColor() {
            return !_CaretColor.IsEmpty;
        }

        // -------------------------------------------------------------------------------
        // WordBreak イベント
        // -------------------------------------------------------------------------------
        private object EventWordBreak = new object();

        void IWrapModeControl.WordBreakCallback(WordBreakEventArgs e) {
            OnWordBreak(e);
        }

        /// <summary>
        /// WordBreak イベントを発生させます。
        /// </summary>
        protected virtual void OnWordBreak(WordBreakEventArgs e) {
            e.Result = WrapModeController.DefaultWordBreakProc(e);
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

        // -------------------------------------------------------------------------------
        // BorderStyle プロパティ
        // -------------------------------------------------------------------------------
        private object EventBorderStyle = new object();
        private BorderStyle _BorderStyle = BorderStyle.Fixed3D;

        /// <summary>
        /// コントロールの罫線の表示方法を取得または設定します。
        /// </summary>
        [Description("コントロールの罫線の表示方法を取得または設定します。")]
        [Category("TextBox")]
        [DefaultValue(BorderStyle.Fixed3D)]
        public BorderStyle BorderStyle {
            get {
                return _BorderStyle;
            }
            set {
                if (_BorderStyle != value) {
                    _BorderStyle = value;
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
                    OnBorderStyleChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// BorderStyleChanged イベントを発生させます。
        /// </summary>
        protected virtual void OnBorderStyleChanged(EventArgs e) {
            var eh = Events[EventCaretColorChanged] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }

        }

        /// <summary>
        /// BorderStyle プロパティの値が変更された場合に発生します。
        /// </summary>
        public event EventHandler BorderStyleChanged {
            add {
                Events.AddHandler(EventBorderStyle, value);
            }
            remove {
                Events.RemoveHandler(EventBorderStyle, value);
            }
        }

    }
}
