using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop
{
    /// <summary>
    /// オブジェクト、メソッド、およびプロパティを、自動化をサポートするプログラミングツールやその他のアプリケーションに公開します。
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00020400-0000-0000-C000-000000000046")]
    public interface IDispatch
    {
        /// <summary>
        /// オブジェクトが提供する型情報インターフェイスの数 (0 または 1) を取得します。
        /// </summary>
        /// <param name="pctinfo">オブジェクトにより提供される型タイプ情報インターフェイスの数を受け取る場所を指定します。</param>
        [PreserveSig]
        void GetTypeInfoCount(out uint pctinfo);

        /// <summary>
        /// オブジェクトの型情報を取得します。この型情報を使用して、インターフェイスの型情報を取得できます。
        /// </summary>  
        /// <param name="iTInfo">返される型情報。</param>
        /// <param name="lcid">型情報のロケール ID。</param>
        /// <param name="info">要求された型情報オブジェクトへのポインター。</param>
        [PreserveSig]
        void GetTypeInfo(uint iTInfo, int lcid, out IntPtr info);

        /// <summary>
        /// 一連の名前を対応する一連のディスパッチ識別子に割り当てます。
        /// </summary>
        /// <param name="iid">将来使用するために予約されています。 Guid.Empty にする必要があります。</param>
        /// <param name="names">マッピング対象として渡される名前の配列。</param>
        /// <param name="cNames">マッピングされる名前のカウント。</param>
        /// <param name="lcid">名前を解釈するロケール コンテキスト。</param>
        /// <param name="rgDispId">名前に対応する ID を受け取る、呼び出し元が割り当てた配列。</param>
        [PreserveSig]
        void GetIDsOfNames(
            ref Guid iid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 2)]
                string[] names,
            uint cNames,
            int lcid,
            [Out]
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I4, SizeParamIndex = 2)]
                int[] rgDispId);

        /// <summary>
        /// オブジェクトによって公開されたプロパティおよびメソッドへのアクセスを提供します。
        /// </summary>
        /// <param name="dispIdMember">メンバーを識別します。</param>
        /// <param name="riid">将来使用するために予約されています。 Guid.Empty にする必要があります。</param>
        /// <param name="lcid">引数を解釈する対象のロケール コンテキスト。</param>
        /// <param name="wFlags">呼び出しのコンテキストを記述するフラグ。</param>
        /// <param name="pDispParams">引数の配列、名前付き引数の DISPID の配列、配列内の要素数のカウントを格納している構造体へのポインター。</param>
        /// <param name="pvarResult">結果が格納される場所へのポインター。</param>
        /// <param name="pExcepInfo">例外情報を格納する構造体へのポインター。</param>
        /// <param name="puArgErr">エラーが存在する最初の引数のインデックス。</param>
        [PreserveSig]
        void Invoke(
            int dispIdMember,
            ref Guid riid,
            int lcid,
            ComTypes.INVOKEKIND wFlags,
            ref ComTypes.DISPPARAMS pDispParams,
            IntPtr pvarResult,
            IntPtr pExcepInfo,
            IntPtr puArgErr);
    }

}
