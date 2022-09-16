namespace VisualStyle.Animation
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// アニメーションをコントロールするクラスです。
    /// </summary>
    /// <typeparam name="T">コントロールの状態を示す列挙体の型。</typeparam>
    /// <remarks>
    /// QueryCurrentState/QueryDuration/DrawControl イベントを実装してください。
    /// </remarks>
    public class AnimationController<T> where T : struct
    {
        [ThreadStatic]
        private static Initializer initializer;

        private class Initializer : ThreadInitializer
        {
            protected override void OnInitialize() {
                NativeMethods.BufferedPaintInit();
            }

            protected override void OnExitThread() {
                NativeMethods.BufferedPaintUnInit();
            }
        }

        /// <summary>
        /// AnimationController がアニメーションの長さを問い合わせます。
        /// </summary>
        public event EventHandler<QueryDurationEventArgs<T>> QueryDuration;
        /// <summary>
        /// AnimationController が現在のステータスを確認するときに呼ばれます。
        /// </summary>
        public event EventHandler<QueryCurrentStateEventArgs<T>> QueryCurrentState;
        /// <summary>
        /// 現在のステータスによってコントロールを描画するときに呼ばれます。
        /// </summary>
        public event EventHandler<DrawControlEventArgs<T>> DrawControl;

        private readonly Control control;
        private readonly Hooker hooker;
        private T oldState;

        /// <summary>
        /// AnimationController のインスタンスを作成します。
        /// </summary>
        /// <param name="control">アニメーションを実装するコントロール</param>
        public AnimationController(Control control) {
            if (initializer == null) {
                initializer = new Initializer();
            }
            this.control = control;
            control.GotFocus += Control_GotFocus;
            control.LostFocus += Control_LostFocus;
            control.MouseCaptureChanged += Control_MouseCaptureChanged;
            control.MouseEnter += Control_MouseEnter;
            control.MouseLeave += Control_MouseLeave;
            control.MouseDown += Control_MouseDown;
            control.MouseUp += Control_MouseUp;
            hooker = new Hooker(this);
            hooker.Install(control);
        }

        /// <summary>
        /// 現在の状態が前の状態と違っていないか確認します。
        /// 標準ではフォーカスの状態およびマウス動作によって呼び出されますが、コントロール独自のタイミングで確認したいときに呼び出してください。
        /// </summary>
        public void CheckCurrentState() {
            if (!oldState.Equals(GetCurrentState())) {
                control.Invalidate();
            }
        }

        private void Control_LostFocus(object sender, EventArgs e) {
            CheckCurrentState();
        }

        private void Control_GotFocus(object sender, EventArgs e) {
            CheckCurrentState();
        }

        private void Control_MouseCaptureChanged(object sender, EventArgs e) {
            CheckCurrentState();
        }

        private void Control_MouseEnter(object sender, EventArgs e) {
            CheckCurrentState();
        }

        private void Control_MouseLeave(object sender, EventArgs e) {
            CheckCurrentState();
        }

        private void Control_MouseDown(object sender, MouseEventArgs e) {
            CheckCurrentState();
        }

        private void Control_MouseUp(object sender, MouseEventArgs e) {
            CheckCurrentState();
        }

        private T SetState(T newState) {
            T tmp = oldState;
            oldState = newState;
            return tmp;
        }

        private int GetDuration(T oldState, T newState) {
            var e = new QueryDurationEventArgs<T>(Duration, oldState, newState);
            OnQueryDuration(e);
            return e.Duration;
        }

        private T GetCurrentState() {
            QueryCurrentStateEventArgs<T> e = new QueryCurrentStateEventArgs<T>();
            OnQueryCurrentState(e);
            return e.State;
        }

        /// <summary>
        /// QueryDuration イベントを発生します。
        /// </summary>
        /// <param name="e">イベント データを格納している QueryDurationEventArgs。</param>
        protected void OnQueryDuration(QueryDurationEventArgs<T> e) {
            QueryDuration?.Invoke(this, e);
        }

        /// <summary>
        /// QueryCurrentState イベントを発生します。
        /// </summary>
        /// <param name="e">イベント データを格納している QueryCurrentStateEventArgs。</param>
        protected virtual void OnQueryCurrentState(QueryCurrentStateEventArgs<T> e) {
            QueryCurrentState?.Invoke(this, e);
        }

        /// <summary>
        /// DrawControl イベントを発生します。
        /// </summary>
        /// <param name="e">イベント データを格納している AnimationPaintEventArgs。</param>
        protected virtual void OnDrawControl(DrawControlEventArgs<T> e) {
            DrawControl?.Invoke(this, e);
        }

        /// <summary>
        /// デフォルトのアニメーション間隔です。(200ミリ秒)
        /// </summary>
        public const int DefaultDuration = 200;

        /// <summary>
        /// QueryDuration イベントの呼び出しに応じなかった場合に適用されるアニメーション間隔を取得または設定します。
        /// </summary>
        [DefaultValue(DefaultDuration)]
        public int Duration { get; set; } = DefaultDuration;

        /// <summary>
        /// アニメーション動作を有効にするかどうかを示す値を取得または設定します。
        /// </summary>
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        private void CallDrawControl(IntPtr hdc, Rectangle clip, T state, bool doubleBufferd) {
            if (doubleBufferd) {
                var context = BufferedGraphicsManager.Current;
                using (var bufferedGraphics = context.Allocate(hdc, control.ClientRectangle)) {
                    bufferedGraphics.Graphics.SetClip(clip);
                    using (var ape = new DrawControlEventArgs<T>(state, bufferedGraphics.Graphics, clip)) {
                        OnDrawControl(ape);
                    }
                    bufferedGraphics.Render();
                }
            } else {
                using (var g = Graphics.FromHdc(hdc)) {
                    using (var ape = new DrawControlEventArgs<T>(state, g, clip)) {
                        OnDrawControl(ape);
                    }
                }
            }
        }

        private class Hooker : NativeWindow
        {
            private readonly AnimationController<T> owner;

            public Hooker(AnimationController<T> owner) {
                this.owner = owner;
            }

            public void Install(Control control) {
                control.HandleCreated += Control_HandleCreated;
                if (control.IsHandleCreated) {
                    Initialize(control);
                }
            }

            private void Control_HandleCreated(object sender, EventArgs e) {
                Initialize((Control)sender);
            }

            private void Initialize(Control control) {
                AssignHandle(control.Handle);
                owner.oldState = owner.GetCurrentState();
            }

            protected override void WndProc(ref Message m) {
                switch (m.Msg) {
                    case NativeMethods.WM_NCDESTROY:
                        base.WndProc(ref m);
                        ReleaseHandle();
                        break;

                    case NativeMethods.WM_PAINT:
                        if (owner.Enabled) {
                            WmPaint(ref m);
                        } else {
                            base.WndProc(ref m);
                        }
                        break;

                    case NativeMethods.WM_PRINTCLIENT:
                        if (owner.Enabled) {
                            WmPrintClient(ref m);
                        } else {
                            base.WndProc(ref m);
                        }
                        break;

                    default:
                        base.WndProc(ref m);
                        break;
                }
            }

            private void WmPrintClient(ref Message m) {
                owner.CallDrawControl(m.WParam, owner.control.ClientRectangle,
                                    owner.GetCurrentState(), false);
            }

            private void WmPaint(ref Message m) {
                // 最新ステータスを取得
                T newState = owner.GetCurrentState();     
                T oldState = owner.SetState(newState);
                if (m.WParam == IntPtr.Zero) {
                    var ps = new NativeMethods.PAINTSTRUCT();
                    IntPtr hdc = NativeMethods.BeginPaint(m.HWnd, ref ps);
                    IntPtr hPal = NativeMethods.SetUpPalette(hdc, false, false);
                    try {
                        // バッファされたアニメーションの次のフレームを描画
                        if (!NativeMethods.BufferedPaintRenderAnimation(m.HWnd, hdc)) {
                            // 次のフレームがない時
                            Rectangle clip = Rectangle.FromLTRB(ps.rcPaint_left, ps.rcPaint_top,
                                                        ps.rcPaint_right, ps.rcPaint_bottom);
                            if (clip.Width > 0 && clip.Height > 0) {

                                // アニメーションバッファを作成
                                var animParams = new NativeMethods.BP_ANIMATIONPARAMS {
                                    cbSize = Marshal.SizeOf(typeof(NativeMethods.BP_ANIMATIONPARAMS)),
                                    style = NativeMethods.BP_ANIMATIONSTYLE.Linear,
                                    dwDuration = owner.GetDuration(oldState, newState)
                                };
                                NativeMethods.RECT rcTarget = new NativeMethods.RECT(clip);
                                IntPtr hBuffer = NativeMethods.BeginBufferedAnimation(
                                                m.HWnd,
                                                hdc,
                                                ref rcTarget,
                                                NativeMethods.BP_BUFFERFORMAT.CompatibleBitmap,
                                                IntPtr.Zero,
                                                ref animParams,
                                                out IntPtr hdcFrom, out IntPtr hdcTo);

                                if (hBuffer != IntPtr.Zero) {
                                    try {
                                        if (hdcFrom != IntPtr.Zero) {
                                            owner.CallDrawControl(hdcFrom, clip, oldState, false);
                                        }
                                        if (hdcTo != IntPtr.Zero) {
                                            owner.CallDrawControl(hdcTo, clip, newState, false);
                                        }
                                    } finally {
                                        // 作成終了
                                        NativeMethods.EndBufferedAnimation(hBuffer, true);
                                    }
                                } else {
                                    // アニメーションバッファが作れなかった時
                                    owner.CallDrawControl(hdc, clip, newState, true);
                                }
                            }
                        }
                    } finally {
                        NativeMethods.SelectPalette(hdc, hPal, 0);
                        NativeMethods.EndPaint(m.HWnd, ref ps);
                    }
                } else {
                    // WParam に hdc が入ってきたとき
                    owner.CallDrawControl(m.WParam, owner.control.ClientRectangle, newState, false);
                }
            }
        }
    }
}
