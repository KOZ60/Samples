using System;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop
{
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        //SUCCEEDED(hr) (((HRESULT)(hr)) >= 0)

        public static bool SUCCEEDED(int hr) {
            return hr >= 0;
        }

        public static bool FAILED(int hr) {
            return hr < 0;
        }
    }
}
