using System;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop.Test.Excel
{
    [ComVisible(true)]
    public class AppEvents : SinkObject, IAppEvents
    {
        public const string EventsId = "00024413-0000-0000-C000-000000000046";
        public static Guid Guid = new Guid(EventsId);

        internal AppEvents() : base(Guid) { }

        internal Application Application {
            get { return (Application)Owner; }
        }

        [DispId(2612)]
        public void AfterCalculate() {
            Application.OnAfterCalculate();
        }

        [DispId(1565)]
        public void NewWorkbook([In] object Wb) {
            Application.OnNewWorkbook(Wb);
        }

        [DispId(2909)]
        public void ProtectedViewWindowActivate([In] object Pvw) {
            Application.OnProtectedViewWindowActivate(Pvw);
        }

        [DispId(2906)]
        public void ProtectedViewWindowBeforeClose([In] object Pvw, [In] int Reason, [In, Out] ref bool Cancel) {
            Application.OnProtectedViewWindowBeforeClose(Pvw, Reason, ref Cancel);
        }

        [DispId(2905)]
        public void ProtectedViewWindowBeforeEdit([In] object Pvw, [In, Out] ref bool Cancel) {
            Application.OnProtectedViewWindowBeforeEdit(Pvw, ref Cancel);
        }

        [DispId(2910)]
        public void ProtectedViewWindowDeactivate([In] object Pvw) {
            Application.OnProtectedViewWindowDeactivate(Pvw);
        }

        [DispId(2903)]
        public void ProtectedViewWindowOpen([In] object Pvw) {
            Application.OnProtectedViewWindowOpen(Pvw);
        }

        [DispId(2908)]
        public void ProtectedViewWindowResize([In] object Pvw) {
            Application.OnProtectedViewWindowResize(Pvw);
        }

        [DispId(1561)]
        public void SheetActivate([In] object Sh) {
            Application.OnSheetActivate(Sh);
        }

        [DispId(3079)]
        public void SheetBeforeDelete([In] object Sh) {
            Application.OnSheetBeforeDelete(Sh);
        }

        [DispId(1559)]
        public void SheetBeforeDoubleClick([In] object Sh, [In] object Target, [In, Out] ref bool Cancel) {
            Application.OnSheetBeforeDoubleClick(Sh, Target, ref Cancel);
        }

        [DispId(1560)]
        public void SheetBeforeRightClick([In] object Sh, [In] object Target, [In, Out] ref bool Cancel) {
            Application.OnSheetBeforeRightClick(Sh, Target, ref Cancel);
        }

        [DispId(1563)]
        public void SheetCalculate([In] object Sh) {
            Application.OnSheetCalculate(Sh);
        }

        [DispId(1564)]
        public void SheetChange([In] object Sh, [In] object Target) {
            Application.OnSheetChange(Sh, Target);
        }

        [DispId(1562)]
        public void SheetDeactivate([In] object Sh) {
            Application.OnSheetDeactivate(Sh);
        }

        [DispId(1854)]
        public void SheetFollowHyperlink([In] object Sh, [In] object Target) {
            Application.OnSheetFollowHyperlink(Sh, Target);
        }

        [DispId(3075)]
        public void SheetLensGalleryRenderComplete([In] object Sh) {
            Application.OnSheetLensGalleryRenderComplete(Sh);
        }

        [DispId(2895)]
        public void SheetPivotTableAfterValueChange([In] object Sh, [In] object TargetPivotTable, [In] object TargetRange) {
            Application.OnSheetPivotTableAfterValueChange(Sh, TargetPivotTable, TargetRange);
        }

        [DispId(2896)]
        public void SheetPivotTableBeforeAllocateChanges(
                                    [In] object Sh, [In] object TargetPivotTable,
                                    [In] int ValueChangeStart, [In] int ValueChangeEnd, 
                                    [In, Out] ref bool Cancel) {
            Application.OnSheetPivotTableBeforeAllocateChanges(
                                    Sh, TargetPivotTable, 
                                    ValueChangeStart, ValueChangeEnd, 
                                    ref Cancel);
        }

        [DispId(2897)]
        public void SheetPivotTableBeforeCommitChanges(
                                    [In] object Sh, [In] object TargetPivotTable,
                                    [In] int ValueChangeStart, [In] int ValueChangeEnd,
                                    [In, Out] ref bool Cancel) {
            Application.OnSheetPivotTableBeforeCommitChanges(
                                    Sh, TargetPivotTable,
                                    ValueChangeStart, ValueChangeEnd,
                                    ref Cancel);
        }

        [DispId(2898)]
        public void SheetPivotTableBeforeDiscardChanges(
                                    [In] object Sh, [In] object TargetPivotTable,
                                    [In] int ValueChangeStart, [In] int ValueChangeEnd) {
            Application.OnSheetPivotTableBeforeDiscardChanges(
                                    Sh, TargetPivotTable,
                                    ValueChangeStart, ValueChangeEnd);
        }

        [DispId(2157)]
        public void SheetPivotTableUpdate([In] object Sh, [In] object Target) {
            Application.OnSheetPivotTableUpdate(Sh, Target);
        }

        [DispId(1558)]
        public void SheetSelectionChange([In] object Sh, [In] object Target) {
            Application.OnSheetSelectionChange(Sh, Target);
        }

        [DispId(3076)]
        public void SheetTableUpdate([In] object Sh, [In] object Target) {
            Application.OnSheetTableUpdate(Sh, Target);
        }

        [DispId(1556)]
        public void WindowActivate([In] object Wb, [In] object Wn) {
            Application.OnWindowActivate(Wb, Wn);
        }

        [DispId(1557)]
        public void WindowDeactivate([In] object Wb, [In] object Wn) {
            Application.OnWindowDeactivate(Wb, Wn);
        }

        [DispId(1554)]
        public void WindowResize([In] object Wb, [In] object Wn) {
            Application.OnWindowResize(Wb, Wn);
        }

        [DispId(1568)]
        public void WorkbookActivate([In] object Wb) {
            Application.OnWorkbookActivate(Wb);
        }

        [DispId(1574)]
        public void WorkbookAddinInstall([In] object Wb) {
            Application.OnWorkbookAddinInstall(Wb);
        }

        [DispId(1575)]
        public void WorkbookAddinUninstall([In] object Wb) {
            Application.OnWorkbookAddinUninstall(Wb);
        }

        [DispId(2911)]
        public void WorkbookAfterSave([In] object Wb, [In] bool Success) {
            Application.OnWorkbookAfterSave(Wb, Success);
        }

        [DispId(2293)]
        public void WorkbookAfterXmlExport([In] object Wb, [In] object Map, [In] string Url, [In] int Result) {
            Application.OnWorkbookAfterXmlExport(Wb, Map, Url, Result);
        }

        [DispId(2291)]
        public void WorkbookAfterXmlImport([In] object Wb, [In] object Map, [In] bool IsRefresh, [In] int Result) {
            Application.OnWorkbookAfterXmlImport(Wb, Map, IsRefresh, Result);
        }

        [DispId(1570)]
        public void WorkbookBeforeClose([In] object Wb, [In, Out] ref bool Cancel) {
            Application.OnWorkbookBeforeClose(Wb, ref Cancel);
        }

        [DispId(1572)]
        public void WorkbookBeforePrint([In] object Wb, [In, Out] ref bool Cancel) {
            Application.OnWorkbookBeforePrint(Wb, ref Cancel);
        }

        [DispId(1571)]
        public void WorkbookBeforeSave([In] object Wb, [In] bool SaveAsUI, [In, Out] ref bool Cancel) {
            Application.OnWorkbookBeforeSave(Wb, SaveAsUI, ref Cancel);
        }

        [DispId(2292)]
        public void WorkbookBeforeXmlExport([In] object Wb, [In] object Map, [In] string Url, [In, Out] ref bool Cancel) {
            Application.OnWorkbookBeforeXmlExport(Wb, Map, Url, ref Cancel);
        }

        [DispId(2290)]
        public void WorkbookBeforeXmlImport([In] object Wb, [In] object Map, [In] string Url, [In] bool IsRefresh, [In, Out] ref bool Cancel) {
            Application.OnWorkbookBeforeXmlImport(Wb, Map, Url, IsRefresh,  ref Cancel);
        }

        [DispId(1569)]
        public void WorkbookDeactivate([In] object Wb) {
            Application.OnWorkbookDeactivate(Wb);
        }

        [DispId(3080)]
        public void WorkbookModelChange([In] object Wb, [In] object Changes) {
            Application.OnWorkbookModelChange(Wb, Changes);
        }

        [DispId(2912)]
        public void WorkbookNewChart([In] object Wb, [In] object Ch) {
            Application.OnWorkbookNewChart(Wb, Ch);
        }

        [DispId(1573)]
        public void WorkbookNewSheet([In] object Wb, [In] object Sh) {
            Application.OnWorkbookNewSheet(Wb, Sh);
        }

        [DispId(1567)]
        public void WorkbookOpen([In] object Wb) {
            Application.OnWorkbookOpen(Wb);
        }

        [DispId(2160)]
        public void WorkbookPivotTableCloseConnection([In] object Wb, [In] object Target) {
            Application.OnWorkbookPivotTableCloseConnection(Wb, Target);
        }

        [DispId(2161)]
        public void WorkbookPivotTableOpenConnection([In] object Wb, [In] object Target) {
            Application.OnWorkbookPivotTableOpenConnection(Wb, Target);
        }

        [DispId(2611)]
        public void WorkbookRowsetComplete([In] object Wb, [In] string Description, [In] string Sheet, [In] bool Success) {
            Application.OnWorkbookRowsetComplete(Wb, Description, Sheet, Success);
        }

        [DispId(2289)]
        public void WorkbookSync([In] object Wb, [In] object SyncEventType) {
            Application.OnWorkbookSync(Wb, SyncEventType);
        }
    }
}
