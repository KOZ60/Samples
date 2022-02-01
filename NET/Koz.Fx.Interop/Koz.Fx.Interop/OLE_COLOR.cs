using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Koz.Fx.Interop
{
    /// <summary>
    /// CCM のカラー値(Double) と System.Drawing.Color の相互変換を行うクラス
    /// </summary>
    [ComVisible(true)]
    public class OLE_COLOR : IComparable, ICloneable, IEquatable<OLE_COLOR>
    {
        private int m_OleColor;

        private OLE_COLOR(int value) {
            m_OleColor = value;
        }

        private OLE_COLOR(Color value) {
            m_OleColor = ColorTranslator.ToOle(value);
        }

        /// <summary>
        /// Double から OLE_COLOR への変換を行います。
        /// </summary>
        public static implicit operator OLE_COLOR(double value) {
            return new OLE_COLOR((int)value);
        }

        /// <summary>
        /// Color から OLE_COLOR への変換を行います。
        /// </summary>
        public static implicit operator OLE_COLOR(Color value) {
            return new OLE_COLOR(value);
        }

        /// <summary>
        /// OLE_COLOR から double への変換を行います。
        /// </summary>
        public static implicit operator double(OLE_COLOR value) {
            return value.m_OleColor;
        }

        /// <summary>
        /// OLE_COLOR から Color への変換を行います。
        /// </summary>
        public static implicit operator Color(OLE_COLOR value) {
            return ColorTranslator.FromOle(value.m_OleColor);
        }

        /// <summary>
        /// OLE_COLOR の比較を行います。
        /// </summary>
        public static int Compare(OLE_COLOR a, OLE_COLOR b) {

            // 両方 Null かインスタンスが同一ならゼロ
            if (object.ReferenceEquals((object)a, (object)b)) {
                return 0;
            }

            // a が null なら b のほうが大きい
            if ((object)a == null) {
                return -1;
            }

            // b が null なら a のほうが大きい
            if ((object)b == null) {
                return 1;
            }

            return a.m_OleColor - b.m_OleColor;
        }

        /// <summary>
        /// オブジェクトの比較を行います。
        /// </summary>
        public int CompareTo(object obj) {
            if (obj is int) {
                return Compare(this, (int)obj);
            } else if (obj is Color) {
                return Compare(this, (Color)obj);
            } else {
                return Compare(this, (OLE_COLOR)obj);
            }
        }

        /// <summary>
        /// オブジェクトの比較を行い等しければ True を返します。
        /// </summary>
        public override bool Equals(object obj) {
            return this.CompareTo(obj) == 0;
        }

        /// <summary>
        /// オブジェクトのハッシュ値を返します。
        /// </summary>
        public override int GetHashCode() {
            return m_OleColor.GetHashCode();
        }

        /// <summary>
        /// オブジェクトを表す文字列を返します。
        /// </summary>
        public override string ToString() {
            return string.Format("OLE_COLOR:{0}", ((Color)this).ToString());
        }

        /// <summary>
        /// オブジェクトをコピーします。
        /// </summary>
        public object Clone() {
            return new OLE_COLOR(m_OleColor);
        }

        /// <summary>
        /// オブジェクトの比較を行います。
        /// </summary>
        public bool Equals(OLE_COLOR other) {
            return this.CompareTo(other) == 0;
        }

        /// <summary>= 演算子</summary>
        public static bool operator ==(OLE_COLOR a, OLE_COLOR b) {
            return Compare(a, b) == 0;
        }

        /// <summary>!= 演算子</summary>
        public static bool operator !=(OLE_COLOR a, OLE_COLOR b) {
            return Compare(a, b) != 0;
        }
    }
}
