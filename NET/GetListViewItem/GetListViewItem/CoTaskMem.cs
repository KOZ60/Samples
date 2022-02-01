using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace GetListViewItem
{
    public class CoTaskMem : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CoTaskMem(int cbSize) : base(true)
        {
            SetHandle(Marshal.AllocCoTaskMem(cbSize));
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeCoTaskMem(handle);
            return true;
        }

        public IntPtr Address {
            get {
                return handle;
            }
        }
    }
}
