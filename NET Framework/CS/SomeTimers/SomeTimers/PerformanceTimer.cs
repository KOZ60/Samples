using System.ComponentModel;
using System.Threading;
using System;
using System.Diagnostics;

public class PerformanceTimer : TimerComponent
{
    public PerformanceTimer() :base() { }

    public PerformanceTimer(IContainer container) : base(container) { }

    protected override void TimerThread() {
        var sw = new Stopwatch();
        try {
            sw.Start();
            long nextTime = intervalTicks;
            while (!CancelHandle.WaitOne(0)) {
                while (sw.ElapsedTicks < nextTime) {
                    Thread.Sleep(0);
                }
                OnElapsed(EventArgs.Empty);
                nextTime += intervalTicks;
            }
        } finally {
            sw.Stop();
        }
    }
}
