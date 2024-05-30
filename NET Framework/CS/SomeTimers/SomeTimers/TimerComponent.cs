using System;
using System.ComponentModel;
using System.Threading;

[DesignerCategory("code")]
[DefaultEvent("Elapsed")]
public abstract class TimerComponent : Component
{
    private static readonly object eventElapsed = new object();

    private long intervalTicks;
    private bool enabled;

    private readonly EventWaitHandle cancelHandle = new ManualResetEvent(false);
    private Thread thread;

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

    public decimal IntervalMillisecond {
        get {
            return IntervalTicks / TimeSpan.TicksPerMillisecond;
        }
        set {
            IntervalTicks = (long)(value * TimeSpan.TicksPerMillisecond);
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

