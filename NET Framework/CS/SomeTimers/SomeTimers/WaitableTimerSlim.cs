using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

public class WaitbleTimerSlim : WaitHandle
{
    public WaitbleTimerSlim() {
        var hTimer = CreateWaitableTimer(IntPtr.Zero, false, null);
        if (hTimer == null || hTimer.IsInvalid) {
            throw new Win32Exception();
        }
        SafeWaitHandle = hTimer;
    }

    public void SetWaitableTimer(long dueTime, int period) {
        if (!SetWaitableTimer(SafeWaitHandle,
                                ref dueTime,
                                period,
                                IntPtr.Zero,
                                IntPtr.Zero, false)) {
            throw new Win32Exception();
        }
    }

    public void Cancel() {
        if (!CancelWaitableTimer(SafeWaitHandle)) {
            throw new Win32Exception();
        }
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern SafeWaitHandle CreateWaitableTimer(
        IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetWaitableTimer(
        SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod,
        IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, bool fResume);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CancelWaitableTimer(SafeWaitHandle hTimer);
}
