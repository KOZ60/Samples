using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace CustomTextBox
{
    class SafeStringBuffer : SafeHandleMinusOneIsInvalid
    {
        public SafeStringBuffer(int nSize) : base(true) {
            handle = Marshal.AllocCoTaskMem(nSize);
        }

        protected override bool ReleaseHandle() {
            Marshal.FreeCoTaskMem(handle);
            return true;
        }

        public IntPtr Handle {
            get {
                return handle;
            }
        }
    }
}
