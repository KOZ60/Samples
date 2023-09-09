using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Koz.Fx.Interop.Test.Word
{
    [ComImport] 
    [Guid(ApplicationEvents2.EventsId)] 
	public interface IApplicationEvents2
	{
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1), TypeLibFunc((short)0x41)]
		void Startup();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        void Quit();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        void DocumentChange();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        void DocumentOpen([In, MarshalAs(UnmanagedType.Interface)] object Doc);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)]
        void DocumentBeforeClose([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In] ref bool Cancel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)]
        void DocumentBeforePrint([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In] ref bool Cancel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)]
        void DocumentBeforeSave([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In] ref bool SaveAsUI, [In] ref bool Cancel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)]
        void NewDocument([In, MarshalAs(UnmanagedType.Interface)] object Doc);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)]
        void WindowActivate([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In, MarshalAs(UnmanagedType.Interface)] object Wn);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)]
        void WindowDeactivate([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In, MarshalAs(UnmanagedType.Interface)] object Wn);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)]
        void WindowSelectionChange([In, MarshalAs(UnmanagedType.Interface)] object Sel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)]
        void WindowBeforeRightClick([In, MarshalAs(UnmanagedType.Interface)] object Sel, [In] ref bool Cancel);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)]
        void WindowBeforeDoubleClick([In, MarshalAs(UnmanagedType.Interface)] object Sel, [In] ref bool Cancel);

    }
}
