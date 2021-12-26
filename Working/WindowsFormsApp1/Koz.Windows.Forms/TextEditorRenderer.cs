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
        // -------------------------------------------------------------------------------
        // Static
        // -------------------------------------------------------------------------------
        public static void DrawClient(TextEditor editor, PaintEventArgs e) {
            DrawClient(editor, e.Graphics, e.ClipRectangle);
        }

        public static void DrawClient(TextEditor editor, Graphics graphics, Rectangle clip) {
            DrawClient(editor, graphics, clip, 
                        editor.ShowMarker, editor.MarkerColor, editor.TabWidth);
        }

        public static void DrawClient(TextBoxBase editor, Graphics graphics, Rectangle clip,
                                        bool showMarker, Color markerColor, int tabWidth) {
            DrawClient(editor, graphics, clip, editor.Text, showMarker, markerColor, tabWidth);
        }

        public static void DrawClient(TextBoxBase editor, Graphics graphics,
                            Rectangle clip, string text,
                            bool showMarker, Color markerColor, int tabWidth) {
            TextEditorRenderer renderer = new TextEditorRenderer(editor);
            renderer.DrawClient(graphics, clip, text, showMarker, markerColor, tabWidth);
        }


        private const char DISPLAY_WIDE_SPACE = '\u2610';
        private const char DISPLAY_HALF_SPACE = '_';
        private const char DISPLAY_NEWLINE = '↓';
        private const char DISPLAY_TAB = '\u0350';

        private static readonly Dictionary<char, char> markerChars;

        static TextEditorRenderer() {
            markerChars = new Dictionary<char, char>();
            markerChars.Add('　', DISPLAY_WIDE_SPACE);
            markerChars.Add(' ', DISPLAY_HALF_SPACE);
            markerChars.Add('\r', DISPLAY_NEWLINE);
            markerChars.Add('\t', DISPLAY_TAB);
        }

        // -------------------------------------------------------------------------------
        // Instance
        // -------------------------------------------------------------------------------

        private Control owner;
        private SelectionRange selectionRange;
        private DrawColors drawColors;
        private Size fontSizeAverage;
        private bool showMarker;
        private int tabWidth;

        private TextEditorRenderer(TextBoxBase editor) {
            this.owner = editor;
        }

        protected virtual void DrawClient(Graphics graphics, Rectangle clip, string text, bool showMarker, Color markerColor, int tabWidth) {
            this.showMarker = showMarker;
            this.selectionRange = new SelectionRange(owner);
            this.drawColors = new DrawColors(owner, markerColor);
            this.tabWidth = tabWidth;

            using (var brush = new SolidBrush(owner.BackColor)) {
                graphics.FillRectangle(brush, clip);
            }

            HandleRef hdc = new HandleRef(this, graphics.GetHdc());
            HandleRef hFont = new HandleRef(this, owner.Font.ToHfont());
            HandleRef oldFont = new HandleRef(this, NativeMethods.SelectObject(hdc, hFont));
            fontSizeAverage = UTL.GetFontSizeAverageFromHdc(hdc);

            try {
                int lineCount = (int)SendMessage(NativeMethods.EM_GETLINECOUNT);
                int startLine = (int)SendMessage(NativeMethods.EM_GETFIRSTVISIBLELINE); 
                int bottom = clip.Bottom;

                for (int i = startLine; i < lineCount; i++) {

                    int lineStart = GetFirstCharIndexFromLine(i); 
                    if (lineStart == -1) {
                        break;
                    }

                    Point pt = GetPositionFromCharIndex(lineStart);
                    if (pt.IsInvalid()) {
                        continue;
                    }
                    if (pt.Y > bottom) {
                        break;
                    }

                    int lineEnd = text.IndexOf('\r', lineStart);
                    if (lineEnd == -1) {
                        lineEnd = text.Length - 1;
                    }

                    Range drawRange = new Range(lineStart, lineEnd);
                    if (drawRange.Length > 0) {
                        DrawLine(hdc, pt, text, drawRange);
                    }
                }

            } finally {
                NativeMethods.SelectObject(hdc, oldFont);
                NativeMethods.DeleteObject(hFont);
                graphics.ReleaseHdc();
            }
        }

        private void DrawLine(HandleRef hdc, Point pt, string str, Range drawRange) {
            DrawMode? prevMode = null;
            var sb = new StringBuilder(str.Length);
            for (int pos = drawRange.Start; pos <= drawRange.End; pos++) {
                char c = str[pos];
                DrawMode mode = GetDrawMode(pos, ref c);
                if (prevMode.HasValue) {
                    if (mode != prevMode || mode.HasFlag(DrawMode.Tab)) {
                        DrawPart(hdc, prevMode.Value, pt, sb);
                        sb.Clear();
                        pt = GetPositionFromCharIndex(pos);
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
            if (showMarker && markerChars.TryGetValue(c, out char outchar)) {
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
            Point minPt = GetPositionFromCharIndex(0);
            int X = pt.X - minPt.X;
            int tabPixelWidth = fontSizeAverage.Width * tabWidth;
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

        private class SelectionRange : Range
        {
            public SelectionRange(Control editor) {
                NativeMethods.SendMessage(
                                 new HandleRef(editor, editor.Handle),
                                 NativeMethods.EM_GETSEL,
                                 out Start, out End);
            }
        }

        protected int WindowStyle {
            get {
                return unchecked((int)(long)NativeMethods.GetWindowLong(new HandleRef(this,owner.Handle), NativeMethods.GWL_STYLE));
            }
        }

        protected bool HideSelection {
            get {
                return (WindowStyle & NativeMethods.ES_NOHIDESEL) == 0;
            }
        }

        protected virtual int GetFirstCharIndexFromLine(int lineNumber) {
            return unchecked((int)(long)SendMessage(NativeMethods.EM_LINEINDEX, new IntPtr(lineNumber), IntPtr.Zero));
        }

        protected virtual Point GetPositionFromCharIndex(int index) {
            IntPtr pt = SendMessage(NativeMethods.EM_POSFROMCHAR, new IntPtr(index), IntPtr.Zero);
            if (pt == NativeMethods.INVALID_HANDLE_VALUE) {
                return UTL.InvalidPoint;
            }
            return new Point(UTL.SignedLOWORD(pt), UTL.SignedHIWORD(pt));
        }
    }
}
