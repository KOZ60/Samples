namespace Koz.Windows.Forms
{
    /// <summary>ワードラップ関数のアクションを指定します。</summary>
    public enum WordBreakCode
    {
        /// <summary>指定された位置の左側にある単語の先頭を検索します。</summary>
        Left,
        /// <summary>指定された位置の右側にある単語の先頭を検索します。</summary>
        Right,
        /// <summary>指定した位置の文字が区切り文字かどうかを確認します。</summary>
        IsDelimiter,
        /// <summary>
        /// 指定された位置にある文字の文字クラスと単語区切りフラグを取得します。
        /// </summary>
        /// <remarks>この値は、リッチエディットコントロールで使用するためのものです。</remarks>
        Classify,
        /// <summary>
        /// 指定された位置の左側にある単語の先頭を検索します。
        /// この値は、CTRL + LEFTキーの処理中に使用されます。
        /// </summary>
        /// <remarks>この値は、リッチエディットコントロールで使用するためのものです。</remarks>
        MoveWordLeft,
        /// <summary>
        /// 指定された位置の右側にある単語の先頭を検索します。
        /// この値は、CTRL + RIGHTキーの処理中に使用されます。
        /// </summary>
        /// <remarks>この値は、リッチエディットコントロールで使用するためのものです。</remarks>
        MoveWordRight,
        /// <summary>
        /// 指定された位置の左側にある単語の終わりの区切り文字を検索します。
        /// </summary>
        /// <remarks>この値は、リッチエディットコントロールで使用するためのものです。</remarks>
        LeftBreak,
        /// <summary>
        /// 指定された位置の右側にある単語の終わりの区切り文字を検索します。
        /// </summary>
        /// <remarks>この値は、リッチエディットコントロールで使用するためのものです。</remarks>
        RightBreak,
    }
}
