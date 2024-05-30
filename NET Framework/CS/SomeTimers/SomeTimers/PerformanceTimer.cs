using System.ComponentModel;
using System.Threading;
using System;
using System.Diagnostics;

[DesignerCategory("code")]
public class PerformanceTimer : TimerComponent
{
    public PerformanceTimer() :base() { }

    public PerformanceTimer(IContainer container) : base(container) { }

    protected override void TimerThread() {
        var sw = new Stopwatch();
        try {
            sw.Start();
            long ticks = IntervalTicks;
            using (var hTimer = new WaitbleTimerSlim()) {
                WaitHandle[] handles = new WaitHandle[] { CancelHandle, hTimer };
                long nextTime = ticks;
                SetWaitableTimer(hTimer, sw, nextTime);
                while (WaitHandle.WaitAny(handles) == 1) {
                    while (sw.ElapsedTicks < nextTime) {
                    }
                    OnElapsed(EventArgs.Empty);
                    nextTime += ticks;
                    SetWaitableTimer(hTimer, sw, nextTime);
                }
            }
        } finally {
            sw.Stop();
        }
    }

    private void SetWaitableTimer(WaitbleTimerSlim hTimer, Stopwatch sw, long nextTime) {
        long dueTime = nextTime - sw.ElapsedTicks - 10;
        if (dueTime < 0) {
            dueTime = 0;
        }
        hTimer.SetWaitableTimer(-dueTime, 0);
    }
}
