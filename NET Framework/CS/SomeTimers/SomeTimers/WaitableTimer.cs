using System;
using System.ComponentModel;
using System.Threading;

[DesignerCategory("code")]
public class WaitableTimer : TimerComponent
{
    public WaitableTimer() : base() { }

    public WaitableTimer(IContainer container) : base(container) { }

    protected override void TimerThread() {
        long ticks = IntervalTicks;
        DateTime utc = DateTime.UtcNow;
        long dueTime = utc.ToFileTimeUtc() + ticks;
        using (var hTimer = new WaitbleTimerSlim()) {
            WaitHandle[] handles = new WaitHandle[] { CancelHandle, hTimer };
            hTimer.SetWaitableTimer(dueTime, 0);
            while (WaitHandle.WaitAny(handles) == 1) {
                OnElapsed(EventArgs.Empty);
                dueTime += ticks;
                hTimer.SetWaitableTimer(dueTime, 0);
            }
        }
    }
}

