using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace OlympicCursor
{
    public class CursorResource : IDisposable
    {
        public static CursorResource FromBitmap(Bitmap bmp, Point hotSpot) {
            return new CursorResource(bmp, hotSpot);
        }

        private readonly ICONINFO iconInfo = new ICONINFO();
        private readonly IntPtr hCursor;

        private CursorResource(Bitmap bmp, Point hotSpot) {
            IntPtr hIcon = bmp.GetHicon();
            GetIconInfo(hIcon, iconInfo);
            DestroyIcon(hIcon);

            iconInfo.xHotspot = hotSpot.X;
            iconInfo.yHotspot = hotSpot.Y;
            iconInfo.fIcon = 0;

            hCursor = CreateIconIndirect(iconInfo);

            var b1 = DeleteObject(iconInfo.hbmColor);
            var b2 = DeleteObject(iconInfo.hbmMask);

            Cursor = new Cursor(hCursor) {
                Tag = this
            };
        }

        public Cursor Cursor { get; }

        [StructLayout(LayoutKind.Sequential)]
        private class ICONINFO
        {
            public int fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask = IntPtr.Zero;
            public IntPtr hbmColor = IntPtr.Zero;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetIconInfo(IntPtr hIcon, ICONINFO piconinfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateIconIndirect([In] ICONINFO iconInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyCursor(IntPtr hIcon);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        private bool disposedValue;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                disposedValue = true;
                if (hCursor != IntPtr.Zero) DestroyCursor(hCursor);
                if (iconInfo.hbmMask != IntPtr.Zero) DeleteObject(iconInfo.hbmMask);
                if (iconInfo.hbmColor != IntPtr.Zero) DeleteObject(iconInfo.hbmColor);
            }
        }

        ~CursorResource() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class Character1
    {
        public string name;
        public int exp;

        public Character1() : this("no name") { }

        public Character1(string name)  {
            this.name = name;
        }
    }

    public class Character2
    {
        public string name;
        public int exp;

        public Character2() {
            Initialize("no name");
        }

        public Character2(string name)  {
            Initialize(name);
        }

        private void Initialize(string name) {
            this.name = name;
        }
    }

    public class Character3
    {
        public string name;
        public int exp;

        public Character3() {
            // 初期処理
        }

        public Character3(string name) : this() {
            this.name = name;
        }
    }

}
