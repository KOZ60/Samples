using System;
using System.ComponentModel;
using System.Threading;

[DesignerCategory("code")]
[DefaultEvent("Elapsed")]
public abstract class TimerComponent : Component
{
    private static readonly object eventElapsed = new object();
    public static readonly decimal MaxInterval 
                        = decimal.MaxValue / TimeSpan.TicksPerMillisecond;

    protected long intervalTicks;
    protected bool enabled;

    protected readonly EventWaitHandle cancelHandle = new ManualResetEvent(false);
    protected Thread thread;

    protected TimerComponent() { }

    protected TimerComponent(IContainer container) : this() {
        if (container == null) {
            throw new ArgumentNullException("container");
        }
        container.Add(this);
    }

    public EventWaitHandle CancelHandle {
        get {
            return cancelHandle;
        }
    }

    protected override void Dispose(bool disposing) {
        Enabled = false;
        base.Dispose(disposing);
    }

    public void Start() {
        Enabled = true;
    }

    public void Stop() {
        Enabled = false;
    }

    public bool Enabled {
        get {
            return enabled;
        }
        set {
            lock (this) {
                if (enabled != value) {
                    enabled = value;
                    if (!DesignMode) {
                        if (value) {
                            cancelHandle.Reset();
                            thread = new Thread(TimerThread);
                            thread.Start();
                        } else {
                            cancelHandle.Set();
                            thread.Join();
                            thread = null;
                        }
                    }
                }
            }
        }
    }

    [RefreshProperties(RefreshProperties.Repaint)]
    public long IntervalTicks {
        get {
            return intervalTicks;
        }
        set {
            if (value < 0) {
                throw new ArgumentException();
            }
            intervalTicks = value;
        }
    }

    [RefreshProperties(RefreshProperties.Repaint)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public decimal Interval {
        get {
            return (decimal)intervalTicks / TimeSpan.TicksPerMillisecond;
        }
        set {
            if (value < decimal.Zero || value > MaxInterval) {
                throw new ArgumentException();
            }
            value *= TimeSpan.TicksPerMillisecond;
            if (value > long.MaxValue) {
                throw new ArgumentException();
            }
            intervalTicks = (long)value;
        }
    }

    protected virtual void OnElapsed(EventArgs e) {
        var handler = (EventHandler)Events[eventElapsed];
        handler?.Invoke(this, e);
    }

    public event EventHandler Elapsed {
        add {
            Events.AddHandler(eventElapsed, value);
        }
        remove {
            Events.RemoveHandler(eventElapsed, value);
        }
    }

    protected abstract void TimerThread();
}

