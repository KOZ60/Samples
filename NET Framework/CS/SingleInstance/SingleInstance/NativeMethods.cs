using System;
using System.Runtime.InteropServices;

internal class NativeMethods
{
    public const string GUID = "{BD00244D-64BE-4881-A702-2F497A7B2F90}";
    public static int WM_APPLICATION_EXIT = RegisterWindowMessage(GUID);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int RegisterWindowMessage(string lpString);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow(string className, string windowName);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool PostMessage(IntPtr hWnd, int Msg,
                                          IntPtr wParam, IntPtr lParam);
}
