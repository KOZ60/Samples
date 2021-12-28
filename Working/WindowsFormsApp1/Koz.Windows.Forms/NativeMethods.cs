using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32.SafeHandles;

namespace Koz.Windows.Forms
{
    internal static class NativeMethods
    {
        private static class ExternDll
        {
            public const string Kernel32 = "kernel32.dll";
            public const string User32 = "user32.dll";
            public const string Gdi32 = "gdi32.dll";
        }

        public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public const int
            WM_NULL = 0x0000,
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_NCDESTROY = WM_DESTROY,
            WM_MOVE = 0x0003,
            WM_SIZE = 0x0005,
            WM_ACTIVATE = 0x0006,
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_ENABLE = 0x000A,
            WM_SETREDRAW = 0x000B,
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_PAINT = 0x000F,
            WM_CLOSE = 0x0010,
            WM_QUIT = 0x0012,
            WM_ERASEBKGND = 0x0014,
            WM_SHOWWINDOW = 0x0018,
            WM_CTLCOLOR = 0x0019,
            WM_NEXTDLGCTL = 0x0028,
            WM_DRAWITEM = 0x002B,
            WM_MEASUREITEM = 0x002C,
            WM_DELETEITEM = 0x002D,
            WM_VKEYTOITEM = 0x002E,
            WM_CHARTOITEM = 0x002F,
            WM_SETFONT = 0x0030,
            WM_GETFONT = 0x0031,
            WM_COMPAREITEM = 0x0039,
            WM_WINDOWPOSCHANGED = 0x0047,
            WM_NOTIFY = 0x004E,
            WM_NCCALCSIZE = 0x0083,
            WM_NCHITTEST = 0x0084,
            WM_NCPAINT = 0x0085,
            WM_GETDLGCODE = 0x0087,
            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6;

        public const int
            WM_KEYFIRST = 0x0100,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_KEYLAST = 0x0108;

        public const int
            WM_INITDIALOG = 0x0110,
            WM_COMMAND = 0x0111,
            WM_SYSCOMMAND = 0x0112,
            WM_TIMER = 0x0113,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115;

        public const int
            WM_CTLCOLORMSGBOX = 0x0132,
            WM_CTLCOLOREDIT = 0x0133,
            WM_CTLCOLORLISTBOX = 0x0134,
            WM_CTLCOLORBTN = 0x0135,
            WM_CTLCOLORDLG = 0x0136,
            WM_CTLCOLORSCROLLBAR = 0x0137,
            WM_CTLCOLORSTATIC = 0x0138;

        public const int
            WM_MOUSEFIRST = 0x0200,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_XBUTTONDBLCLK = 0x020D,
            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSELAST = 0x020A;

        public const int
            WM_CUT = 0x0300,
            WM_COPY = 0x0301,
            WM_PASTE = 0x0302,
            WM_CLEAR = 0x0303,
            WM_UNDO = 0x0304;

        public const int
            WM_PRINT = 0x0317,
            WM_PRINTCLIENT = 0x0318,
            WM_APPCOMMAND = 0x0319,
            WM_USER = 0x0400,
            WM_APP = 0x8000,
            WM_REFLECT = WM_USER + 0x1C00;

        public const int
            WS_BORDER = 0x00800000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_NOINHERITLAYOUT = 0x00100000;

        public const int
            EM_GETSEL = 0x00B0,
            EM_SETSEL = 0x00B1,
            EM_GETRECT = 0x00B2,
            EM_SETRECT = 0x00B3,
            EM_SETRECTNP = 0x00B4,
            EM_SCROLL = 0x00B5,
            EM_LINESCROLL = 0x00B6,
            EM_SCROLLCARET = 0x00B7,
            EM_GETMODIFY = 0x00B8,
            EM_SETMODIFY = 0x00B9,
            EM_GETLINECOUNT = 0x00BA,
            EM_LINEINDEX = 0x00BB,
            EM_SETHANDLE = 0x00BC,
            EM_GETHANDLE = 0x00BD,
            EM_GETTHUMB = 0x00BE,
            EM_LINELENGTH = 0x00C1,
            EM_REPLACESEL = 0x00C2,
            EM_GETLINE = 0x00C4,
            EM_LIMITTEXT = 0x00C5,
            EM_CANUNDO = 0x00C6,
            EM_UNDO = 0x00C7,
            EM_FMTLINES = 0x00C8,
            EM_LINEFROMCHAR = 0x00C9,
            EM_SETTABSTOPS = 0x00CB,
            EM_SETPASSWORDCHAR = 0x00CC,
            EM_EMPTYUNDOBUFFER = 0x00CD,
            EM_GETFIRSTVISIBLELINE = 0x00CE,
            EM_SETREADONLY = 0x00CF,
            EM_SETWORDBREAKPROC = 0x00D0,
            EM_GETWORDBREAKPROC = 0x00D1,
            EM_GETPASSWORDCHAR = 0x00D2,
            EM_SETMARGINS = 0x00D3,
            EM_GETMARGINS = 0x00D4,
            EM_SETLIMITTEXT = EM_LIMITTEXT,
            EM_GETLIMITTEXT = 0x00D5,
            EM_POSFROMCHAR = 0x00D6,
            EM_CHARFROMPOS = 0x00D7,
            EM_SETIMESTATUS = 0x00D8,
            EM_GETIMESTATUS = 0x00D9;

        // Edit 通知メッセージ
        public const int
            EN_SETFOCUS = 0x0100,
            EN_KILLFOCUS = 0x0200,
            EN_CHANGE = 0x0300,
            EN_UPDATE = 0x0400,
            EN_ERRSPACE = 0x0500,
            EN_MAXTEXT = 0x0501,
            EN_HSCROLL = 0x0601,
            EN_VSCROLL = 0x0602,
            EN_ALIGN_LTR_EC = 0x0700,
            EN_ALIGN_RTL_EC = 0x0701;

        public const int
            ES_LEFT = 0x0000,
            ES_CENTER = 0x0001,
            ES_RIGHT = 0x0002,
            ES_MULTILINE = 0x0004,
            ES_UPPERCASE = 0x0008,
            ES_LOWERCASE = 0x0010,
            ES_AUTOVSCROLL = 0x0040,
            ES_AUTOHSCROLL = 0x0080,
            ES_NOHIDESEL = 0x0100,
            ES_READONLY = 0x0800,
            ES_PASSWORD = 0x0020;

        public const int
            EC_LEFTMARGIN = 0x0001,
            EC_RIGHTMARGIN = 0x0002,
            EC_USEFONTINFO = 0xffff;

        public const int
            DLGC_WANTARROWS = 0x0001,
            DLGC_WANTTAB = 0x0002,
            DLGC_WANTALLKEYS = 0x0004,
            DLGC_WANTCHARS = 0x0080,
            DLGC_WANTMESSAGE = 0x0004, 
            DLGC_HASSETSEL = 0x0008;

        public const int
            SBM_SETPOS = 0x00E0,
            SBM_GETPOS = 0x00E1,
            SBM_SETRANGE = 0x00E2,
            SBM_SETRANGEREDRAW = 0x00E6,
            SBM_GETRANGE = 0x00E3,
            SBM_ENABLE_ARROWS = 0x00E4,
            SBM_SETSCROLLINFO = 0x00E9,
            SBM_GETSCROLLINFO = 0x00EA,
            SBM_GETSCROLLBARINFO = 0x00EB;

        public const int
            GCL_WNDPROC = (-24),
            GWL_WNDPROC = (-4),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_ID = (-12);

        [Flags]
        public enum Keystroke : int
        {
            KF_EXTENDED = 0x0100,
            KF_DLGMODE = 0x0800,
            KF_MENUMODE = 0x1000,
            KF_ALTDOWN = 0x2000,
            KF_REPEAT = 0x4000,
            KF_UP = 0x8000
        }

        public static Keystroke GetKeystroke(ref Message m) {
            return (Keystroke)(int)m.LParam.ToInt64();
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int Msg, out int wParam, out int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, ref POINTL lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, ref RECT lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int Msg, ref POINTL wParam, IntPtr lParam);

        [DllImport(ExternDll.User32)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, int[] lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            public int x;
            public int y;

            public override string ToString() {
                return string.Format("POINTL({0},{1})", x, y);
            }
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(Rectangle r) {
                this.left = r.Left;
                this.top = r.Top;
                this.right = r.Right;
                this.bottom = r.Bottom;
            }

            public override string ToString() {
                return string.Format("RECT({0},{1},{2},{3})", left, top, right, bottom);
            }

            public int Width {
                get {
                    return this.right - this.left;
                }
            }

            public int Height {
                get {
                    return this.bottom - this.top;
                }
            }

            public Rectangle Rectangle {
                get {
                    return Rectangle.FromLTRB(left, top, right, bottom);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
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

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr BeginPaint(HandleRef hWnd, [In, Out] ref NativeMethods.PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern bool EndPaint(HandleRef hWnd, ref NativeMethods.PAINTSTRUCT lpPaint);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SelectPalette(HandleRef hdc, HandleRef hpal, int bForceBackground);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int RealizePalette(HandleRef hDC);

        public static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette) {
            IntPtr halftonePalette = Graphics.GetHalftonePalette();
            IntPtr result = SelectPalette(new HandleRef(null, dc), new HandleRef(null, halftonePalette), (force ? 0 : 1));
            if (result != IntPtr.Zero && realizePalette) {
                RealizePalette(new HandleRef(null, dc));
            }
            return result;
        }

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeleteObject(HandleRef hObject);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetSysColorBrush(int nIndex);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int SetTextColor(HandleRef hDC, int crColor);

        [DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int SetBkColor(HandleRef hDC, int clr);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool TextOut(HandleRef hdc, int nXStart, int nYStart, ref char lpString, int cbString);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool TextOut(HandleRef hdc, int nXStart, int nYStart, string lpString, int cbString);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool TextOut(HandleRef hdc, int nXStart, int nYStart, StringBuilder lpString, int cbString);

        [DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SelectObject(HandleRef hdc, HandleRef hgdiobj);

        [DllImport(ExternDll.User32)]
        public static extern bool LockWindowUpdate(HandleRef hWndLock);

        [StructLayout(LayoutKind.Sequential)]
        public class SCROLLINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(SCROLLINFO));
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;

            public SCROLLINFO() {
            }

            public SCROLLINFO(int mask, int min, int max, int page, int pos) {
                fMask = mask;
                nMin = min;
                nMax = max;
                nPage = page;
                nPos = pos;
            }
        }

        public const int
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2;

        public const int
            SIF_RANGE = 0x0001,
            SIF_PAGE = 0x0002,
            SIF_POS = 0x0004,
            SIF_TRACKPOS = 0x0010,
            SIF_ALL = (0x0001 | 0x0002 | 0x0004 | 0x0010);

        [DllImport(ExternDll.User32)]
        public static extern int SetScrollInfo(HandleRef hWnd, int fnBar, SCROLLINFO si, bool redraw);

        [DllImport(ExternDll.User32)]
        public static extern bool GetScrollInfo(HandleRef hWnd, int fnBar, SCROLLINFO si);

        [DllImport(ExternDll.Kernel32)]
        public static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

        [DllImport(ExternDll.User32)]
        public static extern bool MapDialogRect(HandleRef hWnd, ref RECT lpRect);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hwnd, StringBuilder lpString, int maxCount);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(HandleRef hwnd);

        public static string GetWindowText(Control control) {
            int nSize = GetWindowTextLength(new HandleRef(control, control.Handle));
            var sb = new StringBuilder((nSize + 1) * 2);
            nSize = GetWindowText(new HandleRef(control, control.Handle), sb, nSize + 1);
            return sb.ToString();
        }

        [DllImport(ExternDll.User32)]
        public static extern IntPtr GetDC(HandleRef hWnd);

        [DllImport(ExternDll.User32)]
        public static extern bool ReleaseDC(HandleRef hWnd, HandleRef hDC);

        [DllImport(ExternDll.Gdi32)]
        public static extern bool DeleteDC(HandleRef hdc);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LOGFONT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName;

            public LOGFONT() {
            }

            public LOGFONT(LOGFONT lf) {
                this.lfHeight = lf.lfHeight;
                this.lfWidth = lf.lfWidth;
                this.lfEscapement = lf.lfEscapement;
                this.lfOrientation = lf.lfOrientation;
                this.lfWeight = lf.lfWeight;
                this.lfItalic = lf.lfItalic;
                this.lfUnderline = lf.lfUnderline;
                this.lfStrikeOut = lf.lfStrikeOut;
                this.lfCharSet = lf.lfCharSet;
                this.lfOutPrecision = lf.lfOutPrecision;
                this.lfClipPrecision = lf.lfClipPrecision;
                this.lfQuality = lf.lfQuality;
                this.lfPitchAndFamily = lf.lfPitchAndFamily;
                this.lfFaceName = lf.lfFaceName;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TEXTMETRICW
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public ushort tmFirstChar;
            public ushort tmLastChar;
            public ushort tmDefaultChar;
            public ushort tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }

        [DllImport(ExternDll.Gdi32)]
        public static extern bool GetTextMetricsW(HandleRef hdc, out TEXTMETRICW lptm);

        [DllImport(ExternDll.Gdi32)]
        public static extern int SetBkMode(HandleRef hdc, BkModeConstans iBkMode);

        public enum BkModeConstans
        {
            Transparent = 1,
            Opqque = 2,
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msgId, IntPtr wParam, Delegate lParam);

        [DllImport(ExternDll.User32)]
        public static extern bool ShowCaret(HandleRef hWnd);

        [DllImport(ExternDll.User32)]
        public static extern bool HideCaret(HandleRef hWnd);

        [DllImport(ExternDll.User32)]
        public static extern bool GetCaretPos(out POINTL lpPoint);

        [DllImport(ExternDll.User32)]
        public static extern bool CreateCaret(
                HandleRef hwnd, SafeHandle hbitmap, int width, int height);

        [DllImport(ExternDll.User32)]
        public static extern bool DestroyCaret();

        public static IntPtr GetWindowLong(HandleRef hWnd, int nIndex) {
            if (IntPtr.Size == 4) {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
        public static extern IntPtr GetWindowLong32(HandleRef hWnd, int nIndex);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
        public static extern IntPtr GetWindowLongPtr64(HandleRef hWnd, int nIndex);

        [DllImport(ExternDll.Kernel32)]
        public static extern IntPtr LocalAlloc(int uFlags, IntPtr uBytes);

        [DllImport(ExternDll.Kernel32)]
        public static extern IntPtr LocalSize(IntPtr hMem);

        [DllImport(ExternDll.Kernel32)]
        public static extern int LocalFlags(IntPtr hMem);

        [DllImport(ExternDll.Kernel32)]
        public static extern IntPtr LocalLock(IntPtr hMem);

        [DllImport(ExternDll.Kernel32)]
        public static extern bool LocalUnlock(IntPtr hMem);

        [DllImport(ExternDll.Kernel32, SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport(ExternDll.Kernel32)]
        public static extern void RtlMoveMemory(IntPtr dest, IntPtr src, int size);

        [DllImport(ExternDll.Kernel32)]
        public static extern void RtlMoveMemory(byte[] dest, IntPtr src, int size);


        [DllImport(ExternDll.Kernel32)]
        public static extern int lstrlenW(IntPtr lpString);

        [DllImport(ExternDll.Kernel32)]
        public unsafe static extern int lstrlenW(char* lpString);
    }
}
