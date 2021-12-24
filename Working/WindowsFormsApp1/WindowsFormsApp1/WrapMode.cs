using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /// <summary>
    /// 行の折り返し方法を指定します。
    /// </summary>
    public enum WrapMode
    {
        /// <summary>折り返しません。</summary>
        NoWrap,
        /// <summary>単語単位で行を折り返します。</summary>
        WordWrap,
        /// <summary>文字単位で行を折り返します。</summary>
        CharWrap,
    }
}
