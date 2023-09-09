namespace VisualStyle.Animation
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// アニメーションをコントロールするクラスです。
    /// 一般的に実装される状態(AnimationState)をサポートします。
    /// </summary>
    public class AnimationController : AnimationController<AnimationState>
    {
        /// <summary>
        /// 現在のステータスによってコントロールを描画するときに呼ばれます。
        /// </summary>
        public new event EventHandler<DrawControlEventArgs> DrawControl;
        /// <summary>
        /// AnimationController がアニメーションの長さを問い合わせます。
        /// </summary>
        public new event EventHandler<QueryDurationEventArgs> QueryDuration;
        /// <summary>
        /// AnimationController が現在のステータスを確認するときに呼ばれます。
        /// </summary>
        public new event EventHandler<QueryCurrentStateEventArgs> QueryCurrentState;

        /// <summary>
        /// AnimationController のインスタンスを作成します。
        /// </summary>
        /// <param name="control">アニメーションを実装するコントロール</param>
        public AnimationController(Control control) : base(control) { }

#pragma warning disable CS1591 

        protected override DrawControlEventArgs<AnimationState> CreateDrawControlEventArgs
            (AnimationState state, Graphics graphics, Rectangle clipRect) {
            return new DrawControlEventArgs(state, graphics, clipRect);
        }

        protected override QueryCurrentStateEventArgs<AnimationState> CreateQueryCurrentStateEventArgs() {
            return new QueryCurrentStateEventArgs();
        }

        protected override QueryDurationEventArgs<AnimationState> CreateQueryDurationEventArgs
            (int duration, AnimationState oldState, AnimationState newState) {
            return new QueryDurationEventArgs(duration, oldState, newState);
        }

        protected override void OnDrawControl(DrawControlEventArgs<AnimationState> e) {
            DrawControl?.Invoke(this, (DrawControlEventArgs)e);
        }

        protected override void OnQueryCurrentState(QueryCurrentStateEventArgs<AnimationState> e) {
            QueryCurrentState?.Invoke(this, (QueryCurrentStateEventArgs)e);
        }

        protected override void OnQueryDuration(QueryDurationEventArgs<AnimationState> e) {
            QueryDuration?.Invoke(this, (QueryDurationEventArgs)e);
        }
#pragma warning restore CS1591 

    }

    /// <summary>
    /// アニメーションをコントロールするクラスです。
    /// </summary>
    /// <typeparam name="T">コントロールの状態を示す列挙体の型。</typeparam>
    /// <remarks>
    /// QueryCurrentState/QueryDuration/DrawControl イベントを実装してください。
    /// </remarks>
    public class AnimationController<T> where T : struct
    {
        /// <summary>
        /// 現在のステータスによってコントロールを描画するときに呼ばれます。
        /// </summary>
        public event EventHandler<DrawControlEventArgs<T>> DrawControl;
        /// <summary>
        /// AnimationController がアニメーションの長さを問い合わせます。
        /// </summary>
        public event EventHandler<QueryDurationEventArgs<T>> QueryDuration;
        /// <summary>
        /// AnimationController が現在のステータスを確認するときに呼ばれます。
        /// </summary>
        public event EventHandler<QueryCurrentStateEventArgs<T>> QueryCurrentState;

        private readonly Control control;
        private readonly Hooker hooker;
        private T oldState;

        /// <summary>
        /// AnimationController のインスタンスを作成します。
        /// </summary>
        /// <param name="control">アニメーションを実装するコントロール</param>
        public AnimationController(Control control) {
            this.control = control;
            control.HandleCreated += Control_HandleCreated;
            control.HandleDestroyed += Control_HandleDestroyed;
            control.EnabledChanged += Control_EnabledChanged;
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

        private void Control_HandleCreated(object sender, EventArgs e) {
            oldState = GetCurrentState();
            NativeMethods.BufferedPaintInit();
        }

        private void Control_HandleDestroyed(object sender, EventArgs e) {
            NativeMethods.BufferedPaintUnInit();
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

        private void Control_EnabledChanged(object sender, EventArgs e) {
            CheckCurrentState();
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
            var e = CreateQueryDurationEventArgs(Duration, oldState, newState);
            OnQueryDuration(e);
            return e.Duration;
        }

        /// <summary>
        /// マウスがコントロールの上にあるかどうかを示す値を取得します。
        /// </summary>
        public bool MouseIsOver {
            get {
                if (!control.IsHandleCreated) {
                    return false;
                }
                var screenPosition = Control.MousePosition;
                if (NativeMethods.WindowFromPoint(screenPosition) != control.Handle) {
                    return false;
                }
                var clientPosition = control.PointToClient(screenPosition);
                if (!control.ClientRectangle.Contains(clientPosition)) {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// QueryDurationEventArgs のインスタンスを作成します。
        /// </summary>
        /// <param name="duration">QueryDuration の呼び出しに応答しなかった場合に適用されるアニメーションの長さ(ミリ秒)。</param>
        /// <param name="oldState">遷移前の状態を示す値。</param>
        /// <param name="newState">遷移後の状態を示す値。</param>
        /// <returns>QueryDurationEventArgs のインスタンス</returns>
        protected virtual QueryDurationEventArgs<T> CreateQueryDurationEventArgs(int duration, T oldState, T newState) {
            return new QueryDurationEventArgs<T>(duration, oldState, newState);
        }

        /// <summary>
        /// QueryCurrentStateEventArgs のインスタンスを作成します。
        /// </summary>
        /// <returns></returns>
        protected virtual QueryCurrentStateEventArgs<T> CreateQueryCurrentStateEventArgs() {
            return new QueryCurrentStateEventArgs<T>();
        }

        /// <summary>
        /// DrawControlEventArgs のインスタンスを作成します。
        /// </summary>
        /// <param name="state">描画する状態を示す列挙体の値。</param>
        /// <param name="graphics">項目の描画に使用される System.Drawing.Graphics。</param>
        /// <param name="clipRect">描画先の四角形を表す System.Drawing.Rectangle。</param>
        /// <returns></returns>
        protected virtual DrawControlEventArgs<T> CreateDrawControlEventArgs(T state, Graphics graphics, Rectangle clipRect) {
            return new DrawControlEventArgs<T>(state, graphics, clipRect);
        }

        private T GetCurrentState() {
            QueryCurrentStateEventArgs<T> e = CreateQueryCurrentStateEventArgs();
            OnQueryCurrentState(e);
            return e.State;
        }

        /// <summary>
        /// QueryDuration イベントを発生します。
        /// </summary>
        /// <param name="e">イベント データを格納している QueryDurationEventArgs。</param>
        protected virtual void OnQueryDuration(QueryDurationEventArgs<T> e) {
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
        /// <param name="e">イベント データを格納している DrawControlEventArgs。</param>
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
                    using (var ape = CreateDrawControlEventArgs(state, bufferedGraphics.Graphics, clip)) {
                        OnDrawControl(ape);
                    }
                    bufferedGraphics.Render();
                }
            } else {
                using (var g = Graphics.FromHdc(hdc)) {
                    using (var ape = CreateDrawControlEventArgs(state, g, clip)) {
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
