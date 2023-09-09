using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Koz.Fx.Interop.Test.Excel
{
    [ComImport]
    [Guid(AppEvents.EventsId)]
    public interface IAppEvents
    {
        [DispId(2612)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AfterCalculate();

        [DispId(1565)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void NewWorkbook([In] object Wb);

        [DispId(2909)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ProtectedViewWindowActivate([In] object Pvw);

        [DispId(2906)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ProtectedViewWindowBeforeClose([In] object Pvw, [In] int Reason, [In][Out] ref bool Cancel);

        [DispId(2905)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ProtectedViewWindowBeforeEdit([In] object Pvw, [In][Out] ref bool Cancel);

        [DispId(2910)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ProtectedViewWindowDeactivate([In] object Pvw);

        [DispId(2903)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ProtectedViewWindowOpen([In] object Pvw);

        [DispId(2908)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ProtectedViewWindowResize([In] object Pvw);

        [DispId(1561)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetActivate([In] object Sh);

        [DispId(3079)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetBeforeDelete([In] object Sh);

        [DispId(1559)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetBeforeDoubleClick([In] object Sh, [In] object Target, [In][Out] ref bool Cancel);

        [DispId(1560)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetBeforeRightClick([In] object Sh, [In] object Target, [In][Out] ref bool Cancel);

        [DispId(1563)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetCalculate([In] object Sh);

        [DispId(1564)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetChange([In] object Sh, [In] object Target);

        [DispId(1562)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetDeactivate([In] object Sh);

        [DispId(1854)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetFollowHyperlink([In] object Sh, [In] object Target);

        [DispId(3075)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetLensGalleryRenderComplete([In] object Sh);

        [DispId(2895)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetPivotTableAfterValueChange([In] object Sh, [In] object TargetPivotTable, [In] object TargetRange);

        [DispId(2896)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetPivotTableBeforeAllocateChanges([In] object Sh, [In] object TargetPivotTable, [In] int ValueChangeStart, [In] int ValueChangeEnd, [In][Out] ref bool Cancel);

        [DispId(2897)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetPivotTableBeforeCommitChanges([In] object Sh, [In] object TargetPivotTable, [In] int ValueChangeStart, [In] int ValueChangeEnd, [In][Out] ref bool Cancel);

        [DispId(2898)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetPivotTableBeforeDiscardChanges([In] object Sh, [In] object TargetPivotTable, [In] int ValueChangeStart, [In] int ValueChangeEnd);

        [DispId(2157)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetPivotTableUpdate([In] object Sh, [In] object Target);

        [DispId(1558)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetSelectionChange([In] object Sh, [In] object Target);

        [DispId(3076)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SheetTableUpdate([In] object Sh, [In] object Target);

        [DispId(1556)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WindowActivate([In] object Wb, [In] object Wn);

        [DispId(1557)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WindowDeactivate([In] object Wb, [In] object Wn);

        [DispId(1554)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WindowResize([In] object Wb, [In] object Wn);

        [DispId(1568)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookActivate([In] object Wb);

        [DispId(1574)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookAddinInstall([In] object Wb);

        [DispId(1575)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookAddinUninstall([In] object Wb);

        [DispId(2911)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookAfterSave([In] object Wb, [In] bool Success);

        [DispId(2293)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookAfterXmlExport([In] object Wb, [In] object Map, [In] string Url, [In] int Result);

        [DispId(2291)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookAfterXmlImport([In] object Wb, [In] object Map, [In] bool IsRefresh, [In] int Result);

        [DispId(1570)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookBeforeClose([In] object Wb, [In][Out] ref bool Cancel);

        [DispId(1572)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookBeforePrint([In] object Wb, [In][Out] ref bool Cancel);

        [DispId(1571)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookBeforeSave([In] object Wb, [In] bool SaveAsUI, [In][Out] ref bool Cancel);

        [DispId(2292)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookBeforeXmlExport([In] object Wb, [In] object Map, [In] string Url, [In][Out] ref bool Cancel);

        [DispId(2290)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookBeforeXmlImport([In] object Wb, [In] object Map, [In] string Url, [In] bool IsRefresh, [In][Out] ref bool Cancel);

        [DispId(1569)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookDeactivate([In] object Wb);

        [DispId(3080)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookModelChange([In] object Wb, [In] object Changes);

        [DispId(2912)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookNewChart([In] object Wb, [In] object Ch);

        [DispId(1573)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookNewSheet([In] object Wb, [In] object Sh);

        [DispId(1567)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookOpen([In] object Wb);

        [DispId(2160)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookPivotTableCloseConnection([In] object Wb, [In] object Target);

        [DispId(2161)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookPivotTableOpenConnection([In] object Wb, [In] object Target);

        [DispId(2611)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookRowsetComplete([In] object Wb, [In] string Description, [In] string Sheet, [In] bool Success);

        [DispId(2289)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WorkbookSync([In] object Wb, [In] object SyncEventType);
    }
}
