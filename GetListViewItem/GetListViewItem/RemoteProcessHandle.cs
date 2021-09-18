using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace GetListViewItem
{
    public class RemoteProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private const int PROCESS_QUERY_INFORMATION = 0x0400;
        private const int PROCESS_VM_OPERATION = 0x0008;
        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_VM_WRITE = 0x0020;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetCurrentProcessId();

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int flags, bool inherit, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr h);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsWow64Process(IntPtr hProcess, ref bool Wow64Process);

        public RemoteProcessHandle(IntPtr hwnd) : base(true)
        {
            int processId;
            GetWindowThreadProcessId(hwnd, out processId);
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, processId);
            if (Environment.Is64BitOperatingSystem)
            {
                bool isWow64 = false;
                IsWow64Process(hProcess, ref isWow64);
                Is32Bit = isWow64;
            }
            else
            {
                Is32Bit = true;
            }
            SetHandle(hProcess);
        }

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }

        public bool Is32Bit { get; }
    }
}
