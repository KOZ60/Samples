using System;
using System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop.Test.Excel
{
    public class Application : ExcelObject
    {
        public Application() : base(InteropUtils.CreateObject("Excel.Application")) {
            AddSink(new AppEvents());
        }

        private Workbooks workbooks;

        public Workbooks Workbooks {
            get {
                if (workbooks == null) {
                    workbooks = GetWrapperProperty<Workbooks>("Workbooks");
                }
                return workbooks;
            }
        }

        public bool Visible {
            get { return GetProperty<bool>("Visible"); }
            set { SetProperty("Visible", value); }
        }

        public bool DisplayAlerts {
            get { return GetProperty<bool>("DisplayAlerts"); }
            set { SetProperty("DisplayAlerts", value); }
        }

        public void Quit() {
            Dispose();
        }

        protected override void Dispose(bool disposing) {
            DisplayAlerts = false;
            Invoke("Quit");
            base.Dispose(disposing);
        }

        public delegate void AfterCalculateDelegate();
        public delegate void NewWorkbookDelegate(Workbook Wb);
        public delegate void ProtectedViewWindowActivateDelegate(ProtectedViewWindow Pvw);
        public delegate void ProtectedViewWindowBeforeCloseDelegate(ProtectedViewWindow Pvw, XlProtectedViewCloseReason Reason, ref bool Cancel);
        public delegate void ProtectedViewWindowBeforeEditDelegate(ProtectedViewWindow Pvw, ref bool Cancel);
        public delegate void ProtectedViewWindowDeactivateDelegate(ProtectedViewWindow Pvw);
        public delegate void ProtectedViewWindowOpenDelegate(ProtectedViewWindow Pvw);
        public delegate void ProtectedViewWindowResizeDelegate(ProtectedViewWindow Pvw);
        public delegate void SheetActivateDelegate(Sheet Sh);
        public delegate void SheetBeforeDeleteDelegate(Sheet Sh);
        public delegate void SheetBeforeDoubleClickDelegate(Sheet Sh, Range Target, ref bool Cancel);
        public delegate void SheetBeforeRightClickDelegate(Sheet Sh, Range Target, ref bool Cancel);
        public delegate void SheetCalculateDelegate(Sheet Sh);
        public delegate void SheetChangeDelegate(Sheet Sh, Range Target);
        public delegate void SheetDeactivateDelegate(Sheet Sh);
        public delegate void SheetFollowHyperlinkDelegate(Sheet Sh, Hyperlink Target);
        public delegate void SheetLensGalleryRenderCompleteDelegate(Sheet Sh);
        public delegate void SheetPivotTableAfterValueChangeDelegate(Sheet Sh, PivotTable TargetPivotTable, Range TargetRange);
        public delegate void SheetPivotTableBeforeAllocateChangesDelegate(Sheet Sh, PivotTable TargetPivotTable, int ValueChangeStart, int ValueChangeEnd, ref bool Cancel);
        public delegate void SheetPivotTableBeforeCommitChangesDelegate(Sheet Sh, PivotTable TargetPivotTable, int ValueChangeStart, int ValueChangeEnd, ref bool Cancel);
        public delegate void SheetPivotTableBeforeDiscardChangesDelegate(Sheet Sh, PivotTable TargetPivotTable, int ValueChangeStart, int ValueChangeEnd);
        public delegate void SheetPivotTableUpdateDelegate(Sheet Sh, PivotTable Target);
        public delegate void SheetSelectionChangeDelegate(Sheet Sh, Range Target);
        public delegate void SheetTableUpdateDelegate(Sheet Sh, TableObject Target);
        public delegate void WindowActivateDelegate(Workbook Wb, Window Wn);
        public delegate void WindowDeactivateDelegate(Workbook Wb, Window Wn);
        public delegate void WindowResizeDelegate(Workbook Wb, Window Wn);
        public delegate void WorkbookActivateDelegate(Workbook Wb);
        public delegate void WorkbookAddinInstallDelegate(Workbook Wb);
        public delegate void WorkbookAddinUninstallDelegate(Workbook Wb);
        public delegate void WorkbookAfterSaveDelegate(Workbook Wb, bool Success);
        public delegate void WorkbookAfterXmlExportDelegate(Workbook Wb, XmlMap Map, string Url, XlXmlExportResult Result);
        public delegate void WorkbookAfterXmlImportDelegate(Workbook Wb, XmlMap Map, bool IsRefresh, XlXmlImportResult Result);
        public delegate void WorkbookBeforeCloseDelegate(Workbook Wb, ref bool Cancel);
        public delegate void WorkbookBeforePrintDelegate(Workbook Wb, ref bool Cancel);
        public delegate void WorkbookBeforeSaveDelegate(Workbook Wb, bool SaveAsUI, ref bool Cancel);
        public delegate void WorkbookBeforeXmlExportDelegate(Workbook Wb, XmlMap Map, string Url, ref bool Cancel);
        public delegate void WorkbookBeforeXmlImportDelegate(Workbook Wb, XmlMap Map, string Url, bool IsRefresh, ref bool Cancel);
        public delegate void WorkbookDeactivateDelegate(Workbook Wb);
        public delegate void WorkbookModelChangeDelegate(Workbook Wb, ModelChanges Changes);
        public delegate void WorkbookNewChartDelegate(Workbook Wb, Chart Ch);
        public delegate void WorkbookNewSheetDelegate(Workbook Wb, Sheet Sh);
        public delegate void WorkbookOpenDelegate(Workbook Wb);
        public delegate void WorkbookPivotTableCloseConnectionDelegate(Workbook Wb, PivotTable Target);
        public delegate void WorkbookPivotTableOpenConnectionDelegate(Workbook Wb, PivotTable Target);
        public delegate void WorkbookRowsetCompleteDelegate(Workbook Wb, string Description, string Sheet, bool Success);
        public delegate void WorkbookSyncDelegate(Workbook Wb, MsoSyncEventType SyncEventType);

        public event AfterCalculateDelegate AfterCalculate;
        public event NewWorkbookDelegate NewWorkbook;
        public event ProtectedViewWindowActivateDelegate ProtectedViewWindowActivate;
        public event ProtectedViewWindowBeforeCloseDelegate ProtectedViewWindowBeforeClose;
        public event ProtectedViewWindowBeforeEditDelegate ProtectedViewWindowBeforeEdit;
        public event ProtectedViewWindowDeactivateDelegate ProtectedViewWindowDeactivate;
        public event ProtectedViewWindowOpenDelegate ProtectedViewWindowOpen;
        public event ProtectedViewWindowResizeDelegate ProtectedViewWindowResize;
        public event SheetActivateDelegate SheetActivate;
        public event SheetBeforeDeleteDelegate SheetBeforeDelete;
        public event SheetBeforeDoubleClickDelegate SheetBeforeDoubleClick;
        public event SheetBeforeRightClickDelegate SheetBeforeRightClick;
        public event SheetCalculateDelegate SheetCalculate;
        public event SheetChangeDelegate SheetChange;
        public event SheetDeactivateDelegate SheetDeactivate;
        public event SheetFollowHyperlinkDelegate SheetFollowHyperlink;
        public event SheetLensGalleryRenderCompleteDelegate SheetLensGalleryRenderComplete;
        public event SheetPivotTableAfterValueChangeDelegate SheetPivotTableAfterValueChange;
        public event SheetPivotTableBeforeAllocateChangesDelegate SheetPivotTableBeforeAllocateChanges;
        public event SheetPivotTableBeforeCommitChangesDelegate SheetPivotTableBeforeCommitChanges;
        public event SheetPivotTableBeforeDiscardChangesDelegate SheetPivotTableBeforeDiscardChanges;
        public event SheetPivotTableUpdateDelegate SheetPivotTableUpdate;
        public event SheetSelectionChangeDelegate SheetSelectionChange;
        public event SheetTableUpdateDelegate SheetTableUpdate;
        public event WindowActivateDelegate WindowActivate;
        public event WindowDeactivateDelegate WindowDeactivate;
        public event WindowResizeDelegate WindowResize;
        public event WorkbookActivateDelegate WorkbookActivate;
        public event WorkbookAddinInstallDelegate WorkbookAddinInstall;
        public event WorkbookAddinUninstallDelegate WorkbookAddinUninstall;
        public event WorkbookAfterSaveDelegate WorkbookAfterSave;
        public event WorkbookAfterXmlExportDelegate WorkbookAfterXmlExport;
        public event WorkbookAfterXmlImportDelegate WorkbookAfterXmlImport;
        public event WorkbookBeforeCloseDelegate WorkbookBeforeClose;
        public event WorkbookBeforePrintDelegate WorkbookBeforePrint;
        public event WorkbookBeforeSaveDelegate WorkbookBeforeSave;
        public event WorkbookBeforeXmlExportDelegate WorkbookBeforeXmlExport;
        public event WorkbookBeforeXmlImportDelegate WorkbookBeforeXmlImport;
        public event WorkbookDeactivateDelegate WorkbookDeactivate;
        public event WorkbookModelChangeDelegate WorkbookModelChange;
        public event WorkbookNewChartDelegate WorkbookNewChart;
        public event WorkbookNewSheetDelegate WorkbookNewSheet;
        public event WorkbookOpenDelegate WorkbookOpen;
        public event WorkbookPivotTableCloseConnectionDelegate WorkbookPivotTableCloseConnection;
        public event WorkbookPivotTableOpenConnectionDelegate WorkbookPivotTableOpenConnection;
        public event WorkbookRowsetCompleteDelegate WorkbookRowsetComplete;
        public event WorkbookSyncDelegate WorkbookSync;

        internal void OnAfterCalculate() {
            AfterCalculate?.Invoke();
        }

        internal void OnNewWorkbook(object wb) {
            NewWorkbook?.Invoke(GetWorkbook(wb));
        }

        internal void OnProtectedViewWindowActivate(object Pvw) {
            ProtectedViewWindowActivate?.Invoke(GetProtectedViewWindow(Pvw));
        }

        internal void OnProtectedViewWindowBeforeClose(object Pvw, int Reason, ref bool Cancel) {
            ProtectedViewWindowBeforeClose?.Invoke(
                                    GetProtectedViewWindow(Pvw),
                                    (XlProtectedViewCloseReason)Reason, ref Cancel);
        }

        internal void OnProtectedViewWindowBeforeEdit(object Pvw, ref bool Cancel) {
            ProtectedViewWindowBeforeEdit?.Invoke(GetProtectedViewWindow(Pvw), ref Cancel);
        }

        internal void OnProtectedViewWindowDeactivate(object Pvw) {
            ProtectedViewWindowDeactivate?.Invoke(GetProtectedViewWindow(Pvw));
        }

        internal void OnProtectedViewWindowOpen(object Pvw) {
            ProtectedViewWindowOpen?.Invoke(GetProtectedViewWindow(Pvw));
        }

        internal void OnProtectedViewWindowResize(object Pvw) {
            ProtectedViewWindowResize?.Invoke(GetProtectedViewWindow(Pvw));
        }

        internal void OnSheetActivate(object Sh) {
            SheetActivate?.Invoke(GetSheet(Sh));
        }

        internal void OnSheetBeforeDelete(object Sh) {
            SheetBeforeDelete?.Invoke(GetSheet(Sh));
        }

        internal void OnSheetBeforeDoubleClick(object Sh, object Target, ref bool Cancel) {
            SheetBeforeDoubleClick?.Invoke(GetSheet(Sh), GetRange(Target), ref Cancel);
        }

        internal void OnSheetBeforeRightClick(object Sh, object Target, ref bool Cancel) {
            SheetBeforeRightClick?.Invoke(GetSheet(Sh), GetRange(Target), ref Cancel);
        }

        internal void OnSheetCalculate(object Sh) {
            SheetCalculate?.Invoke(GetSheet(Sh));
        }

        internal void OnSheetChange(object Sh, object Target) {
            SheetChange?.Invoke(GetSheet(Sh), GetRange(Target));
        }

        internal void OnSheetDeactivate(object Sh) {
            SheetDeactivate?.Invoke(GetSheet(Sh));
        }

        internal void OnSheetFollowHyperlink(object Sh, object Target) {
            SheetFollowHyperlink?.Invoke(GetSheet(Sh), GetHyperlink(Target));
        }

        internal void OnSheetLensGalleryRenderComplete(object Sh) {
            SheetLensGalleryRenderComplete?.Invoke(GetSheet(Sh));
        }

        internal void OnSheetPivotTableAfterValueChange(object Sh, object TargetPivotTable, object TargetRange) {
            SheetPivotTableAfterValueChange?.Invoke(GetSheet(Sh),
                        GetPivotTable(TargetPivotTable), GetRange(TargetRange));
        }

        internal void OnSheetPivotTableBeforeAllocateChanges(
                                object Sh, object TargetPivotTable,
                                int ValueChangeStart, int ValueChangeEnd, ref bool Cancel) {
            SheetPivotTableBeforeAllocateChanges?.Invoke(
                                GetSheet(Sh), GetPivotTable(TargetPivotTable),
                                ValueChangeStart, ValueChangeEnd, ref Cancel);
        }

        internal void OnSheetPivotTableBeforeCommitChanges(
                                object Sh, object TargetPivotTable, 
                                int ValueChangeStart, int ValueChangeEnd, ref bool Cancel) {
            SheetPivotTableBeforeCommitChanges?.Invoke(
                                GetSheet(Sh), GetPivotTable(TargetPivotTable),
                                ValueChangeStart, ValueChangeEnd, ref Cancel);
        }

        internal void OnSheetPivotTableBeforeDiscardChanges(
                                object Sh, object TargetPivotTable, 
                                int ValueChangeStart, int ValueChangeEnd) {
            SheetPivotTableBeforeDiscardChanges?.Invoke(
                                GetSheet(Sh), GetPivotTable(TargetPivotTable),
                                ValueChangeStart, ValueChangeEnd);
        }

        internal void OnSheetPivotTableUpdate(object Sh, object Target) {
            SheetPivotTableUpdate?.Invoke(GetSheet(Sh), GetPivotTable(Target));
        }

        internal void OnSheetSelectionChange(object Sh, object Target) {
            SheetSelectionChange?.Invoke(GetSheet(Sh), GetRange(Target));
        }

        internal void OnSheetTableUpdate(object Sh, object Target) {
            SheetTableUpdate?.Invoke(GetSheet(Sh), GetTableObject(Target));
        }

        internal void OnWindowActivate(object Wb, object Wn) {
            WindowActivate?.Invoke(GetWorkbook(Wb), GetWindow(Wn));
        }

        internal void OnWindowDeactivate(object Wb, object Wn) {
            WindowDeactivate?.Invoke(GetWorkbook(Wb), GetWindow(Wn));
        }

        internal void OnWindowResize(object Wb, object Wn) {
            WindowResize?.Invoke(GetWorkbook(Wb), GetWindow(Wn));
        }

        internal void OnWorkbookActivate(object Wb) {
            WorkbookActivate?.Invoke(GetWorkbook(Wb));
        }

        internal void OnWorkbookAddinInstall(object Wb) {
            WorkbookAddinInstall?.Invoke(GetWorkbook(Wb));
        }

        internal void OnWorkbookAddinUninstall(object Wb) {
            WorkbookAddinUninstall?.Invoke(GetWorkbook(Wb));
        }

        internal void OnWorkbookAfterSave(object Wb, bool Success) {
            WorkbookAfterSave?.Invoke(GetWorkbook(Wb) , Success);
        }

        internal void OnWorkbookAfterXmlExport(object Wb, object Map, string Url, int Result) {
            WorkbookAfterXmlExport?.Invoke(GetWorkbook(Wb), GetXmlMap(Map), Url, (XlXmlExportResult)Result);
        }

        internal void OnWorkbookAfterXmlImport(object Wb, object Map, bool IsRefresh, int Result) {
            WorkbookAfterXmlImport?.Invoke(GetWorkbook(Wb), GetXmlMap(Map), IsRefresh, (XlXmlImportResult)Result);
        }

        internal void OnWorkbookBeforeClose(object Wb, ref bool Cancel) {
            WorkbookBeforeClose?.Invoke(GetWorkbook(Wb), ref Cancel);
        }

        internal void OnWorkbookBeforePrint(object Wb, ref bool Cancel) {
            WorkbookBeforePrint?.Invoke(GetWorkbook(Wb), ref Cancel);
        }

        internal void OnWorkbookBeforeSave(object Wb, bool SaveAsUI, ref bool Cancel) {
            WorkbookBeforeSave?.Invoke(GetWorkbook(Wb), SaveAsUI, ref Cancel);
        }

        internal void OnWorkbookBeforeXmlExport(object Wb, object Map, string Url, ref bool Cancel) {
            WorkbookBeforeXmlExport?.Invoke(GetWorkbook(Wb), GetXmlMap(Map), Url,  ref Cancel);
        }

        internal void OnWorkbookBeforeXmlImport(object Wb, object Map, string Url, bool IsRefresh, ref bool Cancel) {
            WorkbookBeforeXmlImport?.Invoke(GetWorkbook(Wb), GetXmlMap(Map), Url, IsRefresh, ref Cancel);
        }

        internal void OnWorkbookDeactivate(object Wb) {
            WorkbookDeactivate?.Invoke(GetWorkbook(Wb));
        }

        internal void OnWorkbookModelChange(object Wb, object Changes) {
            WorkbookModelChange?.Invoke(GetWorkbook(Wb), GetModelChanges(Changes));
        }

        internal void OnWorkbookNewChart(object Wb, object Ch) {
            WorkbookNewChart?.Invoke(GetWorkbook(Wb), GetChart(Ch));
        }

        internal void OnWorkbookNewSheet(object Wb, object Sh) {
            WorkbookNewSheet?.Invoke(GetWorkbook(Wb), GetSheet(Sh));
        }

        internal void OnWorkbookOpen(object Wb) {
            WorkbookOpen?.Invoke(GetWorkbook(Wb));
        }

        internal void OnWorkbookPivotTableCloseConnection(object Wb, object Target) {
            WorkbookPivotTableCloseConnection?.Invoke(GetWorkbook(Wb), GetPivotTable(Target));
        }

        internal void OnWorkbookPivotTableOpenConnection(object Wb, object Target) {
            WorkbookPivotTableOpenConnection?.Invoke(GetWorkbook(Wb), GetPivotTable(Target));
        }

        internal void OnWorkbookRowsetComplete(object Wb, string Description, string Sheet, bool Success) {
            WorkbookRowsetComplete?.Invoke(GetWorkbook(Wb), Description, Sheet, Success);
        }

        internal void OnWorkbookSync(object Wb, object SyncEventType) {
            WorkbookSync?.Invoke(GetWorkbook(Wb), GetMsoSyncEventType(SyncEventType));
        }

    }
}
