namespace IniFileForNotepad
{
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class NativeMethods
    {
        const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileInt(
            string lpAppName,
            string lpKeyName,
            int nDefault,
            string lpFileName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileSection(
            string lpAppName,
            StringBuilder lpszReturnBuffer,
            int nSize,
            string lpFileName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WritePrivateProfileSection(
            string lpAppName,
            string lpString,
            string lpFileName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);
    }
}
