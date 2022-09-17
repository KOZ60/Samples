namespace VisualStyle.Animation
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// QueryCurrentState イベントのイベントデータを提供します.
    /// </summary>
    public class QueryCurrentStateEventArgs : QueryCurrentStateEventArgs<AnimationState>
    {
        internal QueryCurrentStateEventArgs() : base() { }
    }

    /// <summary>
    /// QueryDuration イベントのイベントデータを提供します.
    /// </summary>
    public class QueryDurationEventArgs : QueryDurationEventArgs<AnimationState>
    {
        internal QueryDurationEventArgs(int duration, AnimationState oldState, AnimationState newState)
            : base(duration, oldState, newState) { }
    }

    /// <summary>
    /// DrawControl イベントのイベントデータを提供します.
    /// </summary>
    public class DrawControlEventArgs : DrawControlEventArgs<AnimationState>
    {
        internal DrawControlEventArgs(AnimationState state, Graphics graphics, Rectangle clipRect)
            : base(state, graphics, clipRect) { }
    }

    /// <summary>
    /// QueryCurrentState イベントのイベントデータを提供します.
    /// </summary>
    /// <typeparam name="T">現在の状態を示す列挙体の型。</typeparam>
    public class QueryCurrentStateEventArgs<T> : EventArgs where T : struct
    {
        internal QueryCurrentStateEventArgs() { }

        /// <summary>
        /// 現在の状態を示す列挙体の値を取得または設定します。
        /// </summary>
        public T State { get; set; }
    }

    /// <summary>
    /// QueryDuration イベントのイベントデータを提供します.
    /// </summary>
    /// <typeparam name="T">現在の状態を示す列挙体の型。</typeparam>
    public class QueryDurationEventArgs<T> : EventArgs where T : struct
    {
        internal QueryDurationEventArgs(int duration, T oldState, T newState) {
            Duration = duration;
            OldState = oldState;
            NewState = newState;
        }

        /// <summary>
        /// アニメーションの長さを取得または設定します。(ミリ秒)
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// コントロールの変化前の状態を示す列挙体の値を取得します。
        /// </summary>
        public T OldState { get; }
        /// <summary>
        /// コントロールの変化後の状態を示す列挙体の値を取得します。
        /// </summary>
        public T NewState { get; }
    }

    /// <summary>
    /// DrawControl イベントのイベントデータを提供します.
    /// </summary>
    /// <typeparam name="T">現在の状態を示す列挙体の型。</typeparam>
    /// <remarks>PaintEventArgs クラスを継承しています。</remarks>
    public class DrawControlEventArgs<T> : PaintEventArgs where T : struct
    {
        internal DrawControlEventArgs(T state, Graphics graphics, Rectangle clipRect)
            : base(graphics, clipRect) {
            State = state;
        }

        /// <summary>
        /// 描画するコントロールの状態を示す列挙体の値を取得します。
        /// </summary>
        public T State { get; }
    }
}
