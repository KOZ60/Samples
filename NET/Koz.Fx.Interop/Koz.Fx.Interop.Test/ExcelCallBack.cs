using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Koz.Fx.Interop.Test.Excel;

namespace Koz.Fx.Interop.Test
{
    class ExcelCallBack
    {
        public ExcelCallBack(Excel.Application app) {
            app.AfterCalculate += App_AfterCalculate1;
            app.NewWorkbook += App_NewWorkbook1;
            app.ProtectedViewWindowActivate += App_ProtectedViewWindowActivate;
            app.ProtectedViewWindowBeforeClose += App_ProtectedViewWindowBeforeClose;
            app.ProtectedViewWindowBeforeEdit += App_ProtectedViewWindowBeforeEdit;
            app.ProtectedViewWindowDeactivate += App_ProtectedViewWindowDeactivate;
            app.ProtectedViewWindowOpen += App_ProtectedViewWindowOpen;
            app.ProtectedViewWindowResize += App_ProtectedViewWindowResize;
            app.SheetActivate += App_SheetActivate;
            app.SheetBeforeDelete += App_SheetBeforeDelete;
            app.SheetBeforeDoubleClick += App_SheetBeforeDoubleClick;
            app.SheetBeforeRightClick += App_SheetBeforeRightClick;
            app.SheetCalculate += App_SheetCalculate;
            app.SheetChange += App_SheetChange;
            app.SheetDeactivate += App_SheetDeactivate;
            app.SheetFollowHyperlink += App_SheetFollowHyperlink;
            app.SheetLensGalleryRenderComplete += App_SheetLensGalleryRenderComplete;
            app.SheetPivotTableAfterValueChange += App_SheetPivotTableAfterValueChange;
            app.SheetPivotTableBeforeAllocateChanges += App_SheetPivotTableBeforeAllocateChanges;
            app.SheetPivotTableBeforeCommitChanges += App_SheetPivotTableBeforeCommitChanges;
            app.SheetPivotTableBeforeDiscardChanges += App_SheetPivotTableBeforeDiscardChanges;
            app.SheetPivotTableUpdate += App_SheetPivotTableUpdate;
            app.SheetSelectionChange += App_SheetSelectionChange;
            app.SheetTableUpdate += App_SheetTableUpdate;
            app.WindowActivate += App_WindowActivate;
            app.WindowDeactivate += App_WindowDeactivate;
            app.WindowResize += App_WindowResize;
            app.WorkbookActivate += App_WorkbookActivate;
            app.WorkbookAddinInstall += App_WorkbookAddinInstall;
            app.WorkbookAddinUninstall += App_WorkbookAddinUninstall;
            app.WorkbookAfterSave += App_WorkbookAfterSave;
            app.WorkbookAfterXmlExport += App_WorkbookAfterXmlExport;
            app.WorkbookAfterXmlImport += App_WorkbookAfterXmlImport;
            app.WorkbookBeforeClose += App_WorkbookBeforeClose;
            app.WorkbookBeforePrint += App_WorkbookBeforePrint;
            app.WorkbookBeforeSave += App_WorkbookBeforeSave;
            app.WorkbookBeforeXmlExport += App_WorkbookBeforeXmlExport;
            app.WorkbookBeforeXmlImport += App_WorkbookBeforeXmlImport;
            app.WorkbookDeactivate += App_WorkbookDeactivate;
            app.WorkbookModelChange += App_WorkbookModelChange;
            app.WorkbookNewChart += App_WorkbookNewChart;
            app.WorkbookNewSheet += App_WorkbookNewSheet;
            app.WorkbookOpen += App_WorkbookOpen;
            app.WorkbookPivotTableCloseConnection += App_WorkbookPivotTableCloseConnection;
            app.WorkbookPivotTableOpenConnection += App_WorkbookPivotTableOpenConnection;
            app.WorkbookRowsetComplete += App_WorkbookRowsetComplete;
            app.WorkbookSync += App_WorkbookSync;
        }

        private void App_WorkbookSync(Excel.Workbook Wb, Excel.MsoSyncEventType SyncEventType) {
            throw new NotImplementedException();
        }

        private void App_WorkbookRowsetComplete(Excel.Workbook Wb, string Description, string Sheet, bool Success) {
            throw new NotImplementedException();
        }

        private void App_WorkbookPivotTableOpenConnection(Excel.Workbook Wb, Excel.PivotTable Target) {
            throw new NotImplementedException();
        }

        private void App_WorkbookPivotTableCloseConnection(Excel.Workbook Wb, Excel.PivotTable Target) {
            throw new NotImplementedException();
        }

        private void App_WorkbookOpen(Excel.Workbook Wb) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookNewSheet(Excel.Workbook Wb, Excel.Sheet Sh) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookNewChart(Excel.Workbook Wb, Excel.Chart Ch) {
            throw new NotImplementedException();
        }

        private void App_WorkbookModelChange(Excel.Workbook Wb, Excel.ModelChanges Changes) {
            throw new NotImplementedException();
        }

        private void App_WorkbookDeactivate(Excel.Workbook Wb) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookBeforeXmlImport(Excel.Workbook Wb, Excel.XmlMap Map, string Url, bool IsRefresh, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookBeforeXmlExport(Excel.Workbook Wb, Excel.XmlMap Map, string Url, ref bool Cancel) {
            throw new NotImplementedException();
        }

        private void App_WorkbookBeforeSave(Excel.Workbook Wb, bool SaveAsUI, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookBeforePrint(Excel.Workbook Wb, ref bool Cancel) {
            throw new NotImplementedException();
        }

        private void App_WorkbookBeforeClose(Excel.Workbook Wb, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookAfterXmlImport(Excel.Workbook Wb, Excel.XmlMap Map, bool IsRefresh, Excel.XlXmlImportResult Result) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookAfterXmlExport(Excel.Workbook Wb, Excel.XmlMap Map, string Url, Excel.XlXmlExportResult Result) {
            throw new NotImplementedException();
        }

        private void App_WorkbookAfterSave(Excel.Workbook Wb, bool Success) {
            throw new NotImplementedException();
        }

        private void App_WorkbookAddinUninstall(Excel.Workbook Wb) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookAddinInstall(Excel.Workbook Wb) {
            //throw new NotImplementedException();
        }

        private void App_WorkbookActivate(Excel.Workbook Wb) {
            // throw new NotImplementedException();
        }

        private void App_WindowResize(Excel.Workbook Wb, Excel.Window Wn) {
            //throw new NotImplementedException();
        }

        private void App_WindowDeactivate(Excel.Workbook Wb, Excel.Window Wn) {
            //throw new NotImplementedException();
        }

        private void App_WindowActivate(Excel.Workbook Wb, Excel.Window Wn) {
            //throw new NotImplementedException();
        }

        private void App_SheetTableUpdate(Excel.Sheet Sh, Excel.TableObject Target) {
            throw new NotImplementedException();
        }

        private void App_SheetSelectionChange(Excel.Sheet Sh, Excel.Range Target) {
            //throw new NotImplementedException();
        }

        private void App_SheetPivotTableUpdate(Excel.Sheet Sh, Excel.PivotTable Target) {
            //throw new NotImplementedException();
        }

        private void App_SheetPivotTableBeforeDiscardChanges(Excel.Sheet Sh, Excel.PivotTable TargetPivotTable, int ValueChangeStart, int ValueChangeEnd) {
            throw new NotImplementedException();
        }

        private void App_SheetPivotTableBeforeCommitChanges(Excel.Sheet Sh, Excel.PivotTable TargetPivotTable, int ValueChangeStart, int ValueChangeEnd, ref bool Cancel) {
            throw new NotImplementedException();
        }

        private void App_SheetPivotTableBeforeAllocateChanges(Excel.Sheet Sh, Excel.PivotTable TargetPivotTable, int ValueChangeStart, int ValueChangeEnd, ref bool Cancel) {
            throw new NotImplementedException();
        }

        private void App_SheetPivotTableAfterValueChange(Excel.Sheet Sh, Excel.PivotTable TargetPivotTable, Excel.Range TargetRange) {
            throw new NotImplementedException();
        }

        private void App_SheetLensGalleryRenderComplete(Excel.Sheet Sh) {
            throw new NotImplementedException();
        }

        private void App_SheetFollowHyperlink(Excel.Sheet Sh, Excel.Hyperlink Target) {
            //throw new NotImplementedException();
        }

        private void App_SheetDeactivate(Excel.Sheet Sh) {
            //throw new NotImplementedException();
        }

        private void App_SheetChange(Excel.Sheet Sh, Excel.Range Target) {
            //throw new NotImplementedException();
        }

        private void App_SheetCalculate(Excel.Sheet Sh) {
            //throw new NotImplementedException();
        }

        private void App_SheetBeforeRightClick(Excel.Sheet Sh, Excel.Range Target, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_SheetBeforeDoubleClick(Excel.Sheet Sh, Excel.Range Target, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_SheetBeforeDelete(Excel.Sheet Sh) {
            throw new NotImplementedException();
        }

        private void App_SheetActivate(Excel.Sheet Sh) {
            //throw new NotImplementedException();
        }

        private void App_ProtectedViewWindowResize(Excel.ProtectedViewWindow Pvw) {
            throw new NotImplementedException();
        }

        private void App_ProtectedViewWindowOpen(Excel.ProtectedViewWindow Pvw) {
            throw new NotImplementedException();
        }

        private void App_ProtectedViewWindowDeactivate(Excel.ProtectedViewWindow Pvw) {
            throw new NotImplementedException();
        }

        private void App_ProtectedViewWindowBeforeEdit(Excel.ProtectedViewWindow Pvw, ref bool Cancel) {
            throw new NotImplementedException();
        }

        private void App_ProtectedViewWindowBeforeClose(Excel.ProtectedViewWindow Pvw, Excel.XlProtectedViewCloseReason Reason, ref bool Cancel) {
            throw new NotImplementedException();
        }

        private void App_ProtectedViewWindowActivate(Excel.ProtectedViewWindow Pvw) {
            throw new NotImplementedException();
        }

        private void App_NewWorkbook1(Excel.Workbook Wb) {
            Debug.Print("App_NewWorkbook1");
        }

        private void App_AfterCalculate1() {
            throw new NotImplementedException();
        }
    }

}
