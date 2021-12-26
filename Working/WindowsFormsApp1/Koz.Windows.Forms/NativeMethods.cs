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
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_SETTEXT = 0x000C,
            WM_PAINT = 0x000F,
            WM_KEYDOWN = 0x0100,
            WM_CHAR = 0x0102,
            WM_ERASEBKGND = 0x0014,
            WM_PASTE = 0x0302,
            WM_PRINTCLIENT = 0x0318,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115,
            WM_NCDESTROY = 0x0082,
            WM_USER = 0x0400,
            WM_REFLECT = WM_USER + 0x1C00;

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
            WM_CTLCOLOR = 0x0019,
            WM_CTLCOLORMSGBOX = 0x0132,
            WM_CTLCOLOREDIT = 0x0133,
            WM_CTLCOLORLISTBOX = 0x0134,
            WM_CTLCOLORBTN = 0x0135,
            WM_CTLCOLORDLG = 0x0136,
            WM_CTLCOLORSCROLLBAR = 0x0137,
            WM_CTLCOLORSTATIC = 0x0138;

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

        public const int
            EC_LEFTMARGIN = 0x0001,
            EC_RIGHTMARGIN = 0x0002,
            EC_USEFONTINFO = 0xffff;
       
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

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int Msg, IntPtr wParam, IntPtr lParam);

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


        //DECLSPEC_ALLOCATOR HLOCAL LocalReAlloc(
        //  [in] _Frees_ptr_opt_ HLOCAL hMem,
        //  [in] SIZE_T uBytes,
        //  [in] UINT uFlags
        //);    

        //LPVOID LocalLock(
        //  [in] HLOCAL hMem
        //);
        //BOOL LocalUnlock(
        //  [in] HLOCAL hMem
        //);
        //SIZE_T LocalSize(
        //  [in] HLOCAL hMem
        //);
        //HLOCAL LocalFree(
        //  [in] _Frees_ptr_opt_ HLOCAL hMem
        //);
        //DECLSPEC_ALLOCATOR HLOCAL LocalAlloc(
        //  [in] UINT uFlags,
        //  [in] SIZE_T uBytes
        //);
        //UINT LocalFlags(
        //  [in] HLOCAL hMem
        //);
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

        [DllImport(ExternDll.Kernel32)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport(ExternDll.Kernel32)]
        public static extern void RtlMoveMemory(IntPtr dest, IntPtr src, int size);
    }
}
