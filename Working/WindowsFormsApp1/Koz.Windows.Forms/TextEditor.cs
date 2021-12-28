using System;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class TextEditor : Control
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

        private readonly WrapModeController WrapModeController;
        private readonly OwnerDrawController OwnerDrawController;
        private readonly CaretController CaretController;

        public TextEditor() {

            OwnerDrawController = new OwnerDrawController(this);
            WrapModeController = new WrapModeController(this);
            CaretController = new CaretController(this);
            this.WrapMode = WrapModeController.DefaultWrapMode;

            OwnerDrawController.OwnerDraw += DrawController_OwnerDraw;
            WrapModeController.WordBreak += WrapModeController_WordBreak;

            SetStyle(ControlStyles.StandardClick
                            | ControlStyles.StandardDoubleClick
                            | ControlStyles.UseTextForAccessibility
                            | ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
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

        internal bool IsDesignMode {
            get {
                return base.DesignMode;
            }
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            ReAllocMemory(0, false);
            InitializeHandle(Handle);
            WrapModeController.Install();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            InitializeHandle(Handle);
        }

        protected virtual void InitializeHandle(IntPtr handle) {
            HandleRef hwnd = new HandleRef(this, handle);

            // 長さを無制限に
            NativeMethods.SendMessage(hwnd, NativeMethods.EM_LIMITTEXT, IntPtr.Zero, IntPtr.Zero);

            // Tab 幅をセットする
            NativeMethods.SendMessage(hwnd, NativeMethods.EM_SETTABSTOPS, (IntPtr)1, new int[] { TabWidth * 4 });

            // 両端のマージンをゼロに
            IntPtr wp = new IntPtr(NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN);
            NativeMethods.SendMessage(hwnd, NativeMethods.EM_SETMARGINS, wp, IntPtr.Zero);

            // 右端を半角２文字分あける
            var rect = new NativeMethods.RECT();
            NativeMethods.SendMessage(hwnd, NativeMethods.EM_GETRECT, IntPtr.Zero, ref rect);

            // 左右を半角２文字分空ける
            Size avg = UTL.GetFontSizeAverage(Font);
            var cs = ClientRectangle;
            rect.left = ClientRectangle.Left + avg.Width;
            rect.right = ClientRectangle.Right - avg.Width * 2;
            NativeMethods.SendMessage(hwnd, NativeMethods.EM_SETRECT, IntPtr.Zero, ref rect);
        }

        protected virtual void ReAllocMemory(int stringLength, bool force) {
            int nSizeNew = stringLength * 4;

            if (nSizeNew < INITIAL_MEMORY_SIZE) {
                nSizeNew = INITIAL_MEMORY_SIZE;
            }

            HandleRef hwnd = new HandleRef(this, Handle);

            IntPtr hMemOld = NativeMethods.SendMessage(hwnd, NativeMethods.EM_GETHANDLE, IntPtr.Zero, IntPtr.Zero);

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

            NativeMethods.SendMessage(hwnd, NativeMethods.EM_SETHANDLE, hMemNew, IntPtr.Zero);

            NativeMethods.LocalFree(hMemOld);

            // 選択状態を戻す
            SetSelect(selectStart, selectEnd);
        }

        private void DrawController_OwnerDraw(object sender, PaintEventArgs e) {
            TextEditorRenderer.DrawClient(this, e);
        }

        private void WrapModeController_WordBreak(object sender, WordBreakEventArgs e) {
            OnWordBreak(e);
        }

        //int count = 0;

        protected override void WndProc(ref Message m) {
            //System.Diagnostics.Debug.Print("+{0} {1}", new string('-', count), m);
            //count++;

            switch (m.Msg) {

                case NativeMethods.EM_SETSEL:
                case NativeMethods.WM_HSCROLL:
                case NativeMethods.WM_VSCROLL:
                case NativeMethods.WM_LBUTTONDBLCLK:
                case NativeMethods.WM_SETTEXT:
                    WndProcWithLock(ref m);
                    break;

                case NativeMethods.WM_KEYDOWN:
                    WmKeyDown(ref m);
                    break;

                case NativeMethods.WM_CHAR:
                    WmChar(ref m);
                    break;

                case NativeMethods.WM_PASTE:
                    WmPaste(ref m);
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
                    // マウス操作があったら Invalidate
                    if (m.Msg >= NativeMethods.WM_MOUSEFIRST &&
                        m.Msg <= NativeMethods.WM_MOUSELAST) {
                        Invalidate();
                    }
                    break;
            }
            //count--;
            //System.Diagnostics.Debug.Print(" {0} {1}", new string('-', count), m);
        }

        private void WmReflectCommand(ref Message m) {
            switch (UTL.HIWORD(m.WParam)) {

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

        // 描画ロックして Invalidate
        private void WndProcWithLock(ref Message m) {
            UTL.LockWindow(Handle);
            base.WndProc(ref m);
            UTL.UnlockWindow();
            Invalidate(true);
        }

        private void WmKeyDown(ref Message m) {
            Rectangle clip = GetCaretClip();
            GetSelect(out int start, out int end);
            
            UTL.LockWindow(Handle);
            base.WndProc(ref m);
            UTL.UnlockWindow();

            if (end - start > 0) {
                Invalidate();
            } else {
                NativeMethods.Keystroke keystroke = NativeMethods.GetKeystroke(ref m);
                if (keystroke.HasFlag(NativeMethods.Keystroke.KF_REPEAT)) {
                    Refresh(clip);
                } else {
                    Invalidate(clip, true);
                }
            }
        }

        // キャレットの上下１行をクリップする
        private Rectangle GetCaretClip() {
            NativeMethods.GetCaretPos(out NativeMethods.POINTL pt);
            int currentLine = GetLineFromPosition(new Point(pt.x, pt.y));

            int startLine = currentLine > 0 ? currentLine - 1 : currentLine;
            Point startPt = GetFirstCharPositionFromLine(startLine);

            int lastIndex = GetLineCount() - 1;
            int endLine = currentLine + 1 > lastIndex ? lastIndex : currentLine + 1;
            Point endPt = GetFirstCharPositionFromLine(endLine);

            int top = startPt.Y;
            int bottom = endPt.Y + UTL.GetFontSizeAverage(this.Font).Height;
            return Rectangle.FromLTRB(ClientRectangle.Left, top, ClientRectangle.Right, bottom);
        }

        private void WmChar(ref Message m) {
            GetSelect(out int start, out int end);
            if (end - start > 0) {
                UTL.LockWindow(Handle);
                base.WndProc(ref m);
                UTL.UnlockWindow();
                Invalidate();
            } else {
                Rectangle clip = GetCaretClip();
                UTL.LockWindow(Handle);
                base.WndProc(ref m);
                UTL.UnlockWindow();
                Refresh(clip);
            }
        }

        private void WmPaste(ref Message m) {
            if (Clipboard.ContainsText()) {
                string str = Clipboard.GetText();
                Paste(str);
            }
        }

        private void WmSetFont(ref Message m) {
            base.WndProc(ref m);
            InitializeHandle(m.HWnd);
        }

        public virtual int TextLength {
            get {
                if (IsHandleCreated && Marshal.SystemDefaultCharSize == 2) {
                    return NativeMethods.GetWindowTextLength(new HandleRef(this, Handle));
                } else {
                    return Text.Length;
                }
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
            if (IsHandleCreated) {
                int end = start + length;
                SetSelect(start, end);
            }
        }

        internal void SetSelect(int start, int end) {
            SendMessage(NativeMethods.EM_SETSEL, (IntPtr)start, (IntPtr)end);
        }

        internal void GetSelect(out int start, out int end) {
            NativeMethods.SendMessage(
                             new HandleRef(this, Handle),
                             NativeMethods.EM_GETSEL,
                             out start, out end);
        }

        public void ClearUndo() {
            if (IsHandleCreated) {
                SendMessage(NativeMethods.EM_EMPTYUNDOBUFFER);
            }
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
                SendMessage(NativeMethods.EM_REPLACESEL, new IntPtr(0), text);
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

        public Point GetFirstCharPositionFromLine(int lineIndex) {
            int charIndex = GetFirstCharIndexFromLine(lineIndex);
            return GetPositionFromCharIndex(charIndex);
        }

        public virtual Point GetPositionFromCharIndex(int charIndex) {
            IntPtr pt = SendMessage(NativeMethods.EM_POSFROMCHAR, charIndex);
            if (pt == NativeMethods.INVALID_HANDLE_VALUE) {
                return UTL.InvalidPoint;
            }
            return new Point(UTL.SignedLOWORD(pt), UTL.SignedHIWORD(pt));
        }

        public virtual int GetCharIndexFromPosition(Point pt) {
            int longPoint = UTL.MAKELONG(pt.X, pt.Y);
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

        // -------------------------------------------------------------------------------
        // BorderStyle プロパティ
        // -------------------------------------------------------------------------------
        private object EventBorderStyle = new object();
        private BorderStyle _BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

        /// <summary>
        /// コントロールの罫線の表示方法を取得または設定します。
        /// </summary>
        [Description("コントロールの罫線の表示方法を取得または設定します。")]
        [Category("TextBox")]
        [DefaultValue(System.Windows.Forms.BorderStyle.Fixed3D)]
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
