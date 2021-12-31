using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Koz.Windows.Forms.Tools;

namespace Koz.Windows.Forms
{
    public class FxTextEditor : Control, IWrapModeControl
    {
        private const int INITIAL_MEMORY_SIZE = 32767;

        private static readonly object EventBorderStyleChanged = new object();
        private static readonly object EventCaretColorChanged = new object();
        private static readonly object EventCaretWidthChanged = new object();
        private static readonly object EventMarkerColorChanged = new object();
        private static readonly object EventShowMarkerChanged = new object();
        private static readonly object EventTabWidthChanged = new object();
        private static readonly object EventWrapModeChanged = new object();
        private static readonly object EventWordBreak = new object();

        public static readonly Color DefaultCaretColor = Color.Black;
        public static readonly Color DefaultMarkerColor = Color.Green;

        public const BorderStyle DefaultBorderStyle = BorderStyle.Fixed3D;
        public const int DefaultCaretWidth = 2;
        public const WrapMode DefaultWrapMode = WrapMode.NoWrap;

        private Color backColor = Color.Empty;
        private BorderStyle borderStyle = DefaultBorderStyle;
        private CaretBitmap caretBitmap;
        private Color caretColor = Color.Empty;
        private int caretWidth = DefaultCaretWidth;
        private Color markerColor = Color.Empty;
        private bool showMarker = true;
        private int tabWidth = 4;
        private WrapMode wrapMode = DefaultWrapMode;

        private readonly WrapModeController<FxTextEditor> wrapModeController;
        private readonly OwnerDrawSupporter supporter;

        private static class MessageText
        {
            public const string EN_ERRSPACE = "重大なエラー：メモリを割り当てることができません。";
        }

        private static class MessageTitle
        {
            public const string ERROR = "エラー";
        }

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

        public FxTextEditor() {

            wrapModeController = new WrapModeController<FxTextEditor>(this);
            supporter = new OwnerDrawSupporter(this);

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

        public void BeginUpdate() {
            supporter.BeginUpdate();
        }

        public void EndUpdate() {
            supporter.EndUpdate();
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            ReAllocMemory(0, false);
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
            rect.right = ClientRectangle.Right - supporter.FontAverageSize.Width * 2;
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

        internal protected IntPtr FontHandle {
            get {
                return supporter.FontHandle;
            }
        }

        protected CaretBitmap CaretBitmap {
            get {
                Color caretColor = CaretColor;
                Size caretSize = new Size(CaretWidth, supporter.FontAverageSize.Height);
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
            //MessageMonitor.Enter<EditMessage>(ref m);
            switch (m.Msg) {

                case NativeMethods.EM_SETSEL:
                    BeginUpdate();
                    base.WndProc(ref m);
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
            //MessageMonitor.Exit<EditMessage>(ref m);
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
            bottom += supporter.FontAverageSize.Height;
            return Rectangle.FromLTRB(ClientRectangle.Left, top, ClientRectangle.Right, bottom);
        }

        private void WmSetFont(ref Message m) {
            base.WndProc(ref m);
            InitializeHandle(m.HWnd);
            if (Focused) {
                ShowCaret();
            }
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

        // -------------------------------------------------------------------------------
        // BackColor プロパティ
        // -------------------------------------------------------------------------------
        public override Color BackColor {
            get {
                if (backColor.IsEmpty) {
                    return SystemColors.Window;
                }
                return base.BackColor;
            }
            set {
                base.BackColor = value;
                backColor = value;
            }
        }

        public override void ResetBackColor() {
            BackColor = Color.Empty;
        }

        // -------------------------------------------------------------------------------
        // DefaultCursor プロパティ
        // -------------------------------------------------------------------------------
        protected override Cursor DefaultCursor {
            get {
                return Cursors.IBeam;
            }
        }

        // -------------------------------------------------------------------------------
        // MarkerColor プロパティ
        // -------------------------------------------------------------------------------
        /// <summary>
        /// マーカーの色を取得または設定します。
        /// </summary>
        [Description("マーカーの色を取得または設定します。")]
        [Category(Category.Marker)]
        public Color MarkerColor {
            get {
                if (markerColor.IsEmpty) {
                    return DefaultMarkerColor;
                }
                return markerColor;
            }
            set {
                if (MarkerColor != value) {
                    markerColor = value;
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
            return markerColor != Color.Empty;
        }

        // -------------------------------------------------------------------------------
        // ShowMarker プロパティ
        // -------------------------------------------------------------------------------
        /// <summary>
        /// マーカーを表示するかどうかを取得または設定します。
        /// </summary>
        [Description("マーカーを表示するかどうかを取得または設定します。")]
        [Category(Category.Marker)]
        [DefaultValue(true)]
        public bool ShowMarker {
            get {
                return showMarker;
            }
            set {
                if (ShowMarker != value) {
                    showMarker = value;
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
        /// <summary>
        /// タブ幅(文字数)を取得または設定します。
        /// </summary>
        [Description("タブ幅(文字数)を取得または設定します。")]
        [Category(Category.Editor)]
        [DefaultValue(4)]
        public int TabWidth {
            get {
                return tabWidth;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException();
                }
                if (TabWidth != value) {
                    tabWidth = value;
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
        /// <summary>
        /// 複数行の場合の折り返しルールを取得または設定します。
        /// </summary>
        [Description("複数行の場合の折り返しルールを取得または設定します。")]
        [DefaultValue(DefaultWrapMode)]
        [Category(Category.Editor)]
        public WrapMode WrapMode {
            get {
                return wrapMode;
            }
            set {
                if (wrapMode != value) {
                    wrapMode = value;
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
        /// <summary>
        /// キャレットの幅を取得または設定します。
        /// </summary>
        [Description("キャレットの幅を取得または設定します。")]
        [Category(Category.Caret)]
        [DefaultValue(DefaultCaretWidth)]
        public int CaretWidth {
            get {
                return caretWidth;
            }
            set {
                if (caretWidth != value) {
                    caretWidth = value;
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
        /// <summary>
        /// キャレットの色を取得または設定します。
        /// </summary>
        [Description("キャレットの色を取得または設定します。")]
        [Category(Category.Caret)]
        public Color CaretColor {
            get {
                if (caretColor.IsEmpty) {
                    return DefaultCaretColor;
                }
                return caretColor;
            }
            set {
                if (caretColor != value) {
                    caretColor = value;
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
            return !caretColor.IsEmpty;
        }

        // -------------------------------------------------------------------------------
        // WordBreak イベント
        // -------------------------------------------------------------------------------

        void IWrapModeControl.WordBreakCallback(WordBreakEventArgs e) {
            OnWordBreak(e);
        }

        /// <summary>
        /// WordBreak イベントを発生させます。
        /// </summary>
        protected virtual void OnWordBreak(WordBreakEventArgs e) {
            e.Result = wrapModeController.DefaultWordBreakProc(e);
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
        /// <summary>
        /// コントロールの罫線の表示方法を取得または設定します。
        /// </summary>
        [Description("コントロールの罫線の表示方法を取得または設定します。")]
        [Category("TextBox")]
        [DefaultValue(DefaultBorderStyle)]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }
            set {
                if (borderStyle != value) {
                    borderStyle = value;
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
                Events.AddHandler(EventBorderStyleChanged, value);
            }
            remove {
                Events.RemoveHandler(EventBorderStyleChanged, value);
            }
        }

    }
}
