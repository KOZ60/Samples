namespace VisualStyle.Animation
{
    using System;
    using System.Windows.Forms.VisualStyles;
#pragma warning disable CS1591 

    /// <summary>
    /// Windows のテーマに関するクラス
    /// </summary>
    public class ThemeData : IDisposable
    {
        private readonly IntPtr hTheme;

        private ThemeData(string className) {
            hTheme = NativeMethods.OpenThemeData(IntPtr.Zero, className);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                disposedValue = true;
                if (disposing) {
                }
                NativeMethods.CloseThemeData(hTheme);
            }
        }

        ~ThemeData() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private int GetThemeTransitionDuration(int part, int iFromState, int iToState) {
            NativeMethods.GetThemeTransitionDuration(hTheme, part, iFromState, iToState,
                    NativeMethods.TMT_TRANSITIONDURATIONS, out int dwDuration);
            return dwDuration;
        }

        /// <summary>
        /// 指定されたトランジションの継続時間を取得します。
        /// </summary>
        /// <param name="oldElement">遷移前の VisualStyleElement。</param>
        /// <param name="newElement">遷移後の VisualStyleElement。</param>
        /// <returns>遷移にかかる時間(ミリ秒)</returns>
        public static int GetThemeTransitionDuration(VisualStyleElement oldElement, VisualStyleElement newElement) {
            if (oldElement.ClassName != newElement.ClassName) {
                throw new ArgumentException("クラス名が違います。");
            }
            if (oldElement.Part != newElement.Part) {
                throw new ArgumentException("スタイル要素の部品をを示す値が違います。");
            }
            using (var data = new ThemeData(oldElement.ClassName)) {
                return data.GetThemeTransitionDuration(oldElement.Part, oldElement.State, newElement.State);
            }
        }
    }
}
