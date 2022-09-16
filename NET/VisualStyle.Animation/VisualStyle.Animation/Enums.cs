namespace VisualStyle.Animation
{
    /// <summary>
    /// 標準でコントロールに実装されるであろうステータスを示す列挙体。
    /// </summary>
    public enum AnimationState
    {
        /// <summary>通常の状態</summary>
        Normal,
        /// <summary>マウスがホバーしホットな状態</summary>
        Hot,
        /// <summary>マウスボタンが押された状態</summary>
        Pressed,
        /// <summary>コントロールが無効な状態</summary>
        Disabled,
    }
}
