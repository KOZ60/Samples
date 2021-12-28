using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public class TextEditorRenderer
    {
        private const char DISPLAY_WIDE_SPACE = '\u2610';
        private const char DISPLAY_HALF_SPACE = '_';
        private const char DISPLAY_NEWLINE = '↓';
        private const char DISPLAY_TAB = '\u0350';

        // -------------------------------------------------------------------------------
        // Static
        // -------------------------------------------------------------------------------
        [ThreadStatic]
        private static Dictionary<char, char> markers;
        private static Dictionary<char, char> Markers {
            get {
                if (markers == null) {
                    markers = new Dictionary<char, char>();
                    markers.Add('　', DISPLAY_WIDE_SPACE);
                    markers.Add(' ', DISPLAY_HALF_SPACE);
                    markers.Add('\r', DISPLAY_NEWLINE);
                    markers.Add('\t', DISPLAY_TAB);
                }
                return markers;
            }
        }

        public static void DrawClient(TextEditor editor, PaintEventArgs e) {
            DrawClient(editor, e.Graphics, e.ClipRectangle);
        }

        public static void DrawClient(TextEditor editor, Graphics graphics, Rectangle clip) {
            HandleRef hwnd = new HandleRef(editor, editor.Handle);
            IntPtr hMem = NativeMethods.SendMessage(hwnd, NativeMethods.EM_GETHANDLE, IntPtr.Zero, IntPtr.Zero);
            IntPtr ptr = NativeMethods.LocalLock(hMem);
            try {
                unsafe {
                    TextEditorRenderer renderer = new TextEditorRenderer(editor);
                    renderer.DrawClient(graphics, clip, (char*)ptr.ToPointer());
                }
            } finally {
                NativeMethods.LocalUnlock(hMem);
            }
        }

        // -------------------------------------------------------------------------------
        // Instance
        // -------------------------------------------------------------------------------

        private TextEditor owner;
        private SelectionRange selectionRange;
        private DrawColors drawColors;
        private Size fontSizeAverage;

        private TextEditorRenderer(TextEditor editor) {
            this.owner = editor;
        }

        public class HdcWrapper : SafeHandle
        {
            public HdcWrapper() : base(IntPtr.Zero, true) {

            }

            protected override bool ReleaseHandle() {

                handle = IntPtr.Zero;
                return true;
            }

            public override bool IsInvalid {
                get {
                    return handle == IntPtr.Zero;
                }
            }
        }

        protected unsafe virtual void DrawClient(Graphics graphics, Rectangle clip, char* text) {

            this.selectionRange = new SelectionRange(owner);
            this.drawColors = new DrawColors(owner, owner.MarkerColor);

            using (var brush = new SolidBrush(owner.BackColor)) {
                graphics.FillRectangle(brush, clip);
            }

            DrawText(graphics, clip, text);

            // FixedSingle はクライアント領域に線が書かれる
            if (owner.BorderStyle == BorderStyle.FixedSingle) {
                Rectangle borderRect = UTL.DeflateRect(owner.ClientRectangle, new Padding(1));
                graphics.DrawRectangle(SystemPens.WindowFrame, borderRect);
            }
        }

        protected unsafe virtual void DrawText(Graphics graphics, Rectangle clip, char* text) {

            if (text == null) return;
            int textLength = NativeMethods.lstrlenW(text);
            if (textLength == 0) return;

            HandleRef hdc = new HandleRef(this, graphics.GetHdc());
            HandleRef hFont = new HandleRef(this, owner.Font.ToHfont());
            HandleRef oldFont = new HandleRef(this, NativeMethods.SelectObject(hdc, hFont));
            fontSizeAverage = UTL.GetFontSizeAverageFromHdc(hdc);

            try {
                int startLine = owner.GetFirstVisibleLine();
                int lineCount = owner.GetLineCount();
                int bottom = clip.Bottom;
                Rectangle cr = owner.ClientRectangle;

                int endLine = startLine;

                var lst = new List<DrawRange>();

                for (int i = startLine; i < lineCount; i++) {
                    // 行の最初の文字位置を取得
                    int lineStart = owner.GetFirstCharIndexFromLine(i);
                    if (lineStart == -1) {
                        break;
                    }
                    // 行の最初の文字の座標を取得
                    Point pt = owner.GetPositionFromCharIndex(lineStart);
                    if (pt.IsInvalid() || pt.Y > bottom) {
                        break;
                    }

                    // 行の長さを取得(改行が含まれない)
                    int lineLength = owner.GetLineLength(i);
                    int lineEnd = lineStart + lineLength - 1;

                    // 次の文字が改行で同一行なら含める
                    int checkCharIndex = lineEnd + 1;
                    if (checkCharIndex < textLength && text[checkCharIndex] == '\r') {
                        int checkLineIndex = owner.GetLineFromCharIndex(checkCharIndex);
                        if (checkLineIndex == i) {
                            lineEnd = checkCharIndex;
                        }
                    }

                    DrawRange drawRange = new DrawRange(pt, lineStart, lineEnd);
                    if (drawRange.Length > 0) {
                        lst.Add(drawRange);
                    }
                    endLine = i;
                }

                //System.Diagnostics.Debug.Print("{0} {1} ～ {2} を描画", DateTime.Now, startLine, endLine); ;

                for (int i = 0; i < lst.Count; i++) {
                    DrawLine(hdc, lst[i], text);
                }

            } finally {
                NativeMethods.SelectObject(hdc, oldFont);
                NativeMethods.DeleteObject(hFont);
                graphics.ReleaseHdc();
            }
        }

        private unsafe void DrawLine(HandleRef hdc, DrawRange drawRange, char* text) {
            Point pt = drawRange.Location;
            DrawMode? prevMode = null;
            var sb = new StringBuilder(drawRange.Length * 2);
            for (int pos = drawRange.Start; pos <= drawRange.End; pos++) {
                char c = text[pos];
                DrawMode mode = GetDrawMode(pos, ref c);
                if (prevMode.HasValue) {
                    if (mode != prevMode || mode.HasFlag(DrawMode.Tab)) {
                        DrawPart(hdc, prevMode.Value, pt, sb);
                        sb.Clear();
                        pt = owner.GetPositionFromCharIndex(pos);
                    }
                }
                sb.Append(c);
                prevMode = mode;
            }
            if (sb.Length > 0) {
                DrawPart(hdc, prevMode.Value, pt, sb);
            }
        }

        private DrawMode GetDrawMode(int pos, ref char c) {
            DrawMode mode = DrawMode.Normal;
            if (c == '\t') {
                mode |= DrawMode.Tab;
            }
            if (owner.ShowMarker && Markers.TryGetValue(c, out char outchar)) {
                c = outchar;
                mode |= DrawMode.Marker;
            }
            if ((!HideSelection || owner.Focused) && selectionRange.Contains(pos)) {
                mode |= DrawMode.Highlight;
            }
            if (!owner.Enabled) {
                return DrawMode.Disable;
            }
            return mode;
        }

        private void DrawPart(HandleRef hdc, DrawMode mode, Point pt, StringBuilder sb) {
            DrawColor drawColor = drawColors[mode];
            NativeMethods.SetTextColor(hdc, drawColor.Win32ForeColor);
            NativeMethods.SetBkColor(hdc, drawColor.Win32BackColor);
            if (mode.HasFlag(DrawMode.Tab)) {
                if (mode.HasFlag(DrawMode.Highlight)) {
                    DrawTabHighlight(hdc, pt);
                }
                NativeMethods.TextOut(hdc, pt.X + 3, pt.Y, sb, sb.Length);
            } else {
                NativeMethods.TextOut(hdc, pt.X, pt.Y, sb, sb.Length);
            }
        }

        private void DrawTabHighlight(HandleRef hdc, Point pt) {
            Point minPt = owner.GetPositionFromCharIndex(0);
            int X = pt.X - minPt.X;
            int tabPixelWidth = fontSizeAverage.Width * owner.TabWidth;
            int width = ((X + tabPixelWidth) / tabPixelWidth) * tabPixelWidth - X;
            var rect = new Rectangle(pt, new Size(width, fontSizeAverage.Height));
            using (var g = Graphics.FromHdcInternal(hdc.Handle)) {
                g.FillRectangle(SystemBrushes.Highlight, rect);
            }
        }

        protected IntPtr SendMessage(int msg) {
            return SendMessage(msg, IntPtr.Zero, IntPtr.Zero);
        }

        protected IntPtr SendMessage(int msg, IntPtr wParam, IntPtr lParam) {
            return NativeMethods.SendMessage(
                            new HandleRef(this, owner.Handle),
                            msg,
                            wParam,
                            lParam);
        }

        [Flags]
        private enum DrawMode
        {
            Normal = 0x0000,
            Marker = 0x0001,
            Highlight = 0x0003,
            Tab = 0x0010,
            TabMarker = Tab | Marker,
            TabHighlight = Tab | Highlight,
            Disable = 0x0020,
        }

        private class DrawColor
        {
            public DrawColor(Color foreColor, Color backColor) {
                Win32ForeColor = ColorTranslator.ToWin32(foreColor);
                Win32BackColor = ColorTranslator.ToWin32(backColor);
            }

            public int Win32ForeColor { get; private set; }
            public int Win32BackColor { get; private set; }
        }

        private class DrawColors : Dictionary<DrawMode, DrawColor>
        {
            private Control Editor;
            private Color MarkerColor;

            public DrawColors(Control editor, Color markerColor) {
                Editor = editor;
                MarkerColor = markerColor;
                Add(DrawMode.Normal, new DrawColor(editor.ForeColor, editor.BackColor));
                Add(DrawMode.Marker, new DrawColor(markerColor, editor.BackColor));
                Add(DrawMode.Highlight, new DrawColor(SystemColors.HighlightText, SystemColors.Highlight));
                Add(DrawMode.Tab, new DrawColor(editor.ForeColor, editor.BackColor));
                Add(DrawMode.TabMarker, new DrawColor(markerColor, editor.BackColor));
                Add(DrawMode.TabHighlight, new DrawColor(SystemColors.HighlightText, SystemColors.Highlight));
                Add(DrawMode.Disable, new DrawColor(SystemColors.GrayText, editor.BackColor));
            }

            public new DrawColor this[DrawMode key] {
                get {
                    return base[key];
                }
            }
        }

        private class Range
        {
            protected Range() { }

            public Range(int start, int end) {
                this.Start = start;
                this.End = end;
            }

            public int Start;
            public int End;
            public int Length {
                get {
                    return End - Start + 1;
                }
            }

            public bool Contains(int pos) {
                return (pos >= Start && pos < End);
            }
        }

        private class DrawRange : Range
        {
            public DrawRange(Point pt, int start, int end)
                : base(start, end) {
                Location = pt;
            }
            public Point Location;
        }

        private class SelectionRange : Range
        {
            public SelectionRange(TextEditor editor) {
                editor.GetSelect(out Start, out End);
            }
        }

        protected int WindowStyle {
            get {
                return unchecked((int)(long)NativeMethods.GetWindowLong(new HandleRef(this, owner.Handle), NativeMethods.GWL_STYLE));
            }
        }

        protected bool HideSelection {
            get {
                return (WindowStyle & NativeMethods.ES_NOHIDESEL) == 0;
            }
        }
    }
}
