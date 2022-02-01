using System;
using System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop.Test.Word
{
    public class Application : WordObject
    {
        public Application() : base(InteropUtils.CreateObject("Word.Application")) {
            AddSink(new ApplicationEvents2());
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

        Documents documents;

        public Documents Documents {
            get {
                if (documents == null) {
                    documents = GetWrapperProperty<Documents>("Documents");
                }
                return documents;
            }
        }

        public delegate void StartupDelegate();
        public delegate void DocumentChangeDelegate();
        public delegate void QuitDelegate();
        public delegate void DocumentOpenDelegate(Document Doc);
        public delegate void DocumentBeforeCloseDelegate(Document Doc, ref bool Cancel);
        public delegate void DocumentBeforePrintDelegate(Document Doc, ref bool Cancel);
        public delegate void DocumentBeforeSaveDelegate(Document Doc, ref bool SaveAsUI, ref bool Cancel);
        public delegate void NewDocumentDelegate(Document Doc);
        public delegate void WindowActivateDelegate(Document Doc, Window Wn);
        public delegate void WindowDeactivateDelegate(Document Doc, Window Wn);
        public delegate void WindowSelectionChangeDelegate(Selection Sel);
        public delegate void WindowBeforeRightClickDelegate(Selection Sel, ref bool Cancel);
        public delegate void WindowBeforeDoubleClickDelegate(Selection Sel, ref bool Cancel);

        public event StartupDelegate Startup;
        public event DocumentChangeDelegate DocumentChange;
        public event QuitDelegate QuitEvent;
        public event DocumentOpenDelegate DocumentOpen;
        public event DocumentBeforeCloseDelegate DocumentBeforeClose;
        public event DocumentBeforePrintDelegate DocumentBeforePrint;
        public event DocumentBeforeSaveDelegate DocumentBeforeSave;
        public event NewDocumentDelegate NewDocument;
        public event WindowActivateDelegate WindowActivate;
        public event WindowDeactivateDelegate WindowDeactivate;
        public event WindowSelectionChangeDelegate WindowSelectionChange;
        public event WindowBeforeRightClickDelegate WindowBeforeRightClick;
        public event WindowBeforeDoubleClickDelegate WindowBeforeDoubleClick;

        internal void OnStartup() {
            Startup?.Invoke();
        }

        internal void OnQuit() {
            QuitEvent?.Invoke();
            Dispose();
        }

        internal void OnDocumentChange() {
            DocumentChange?.Invoke();
        }

        internal void OnDocumentOpen(object Doc) {
            DocumentOpen?.Invoke(GetDocument(Doc));
        }

        internal void OnDocumentBeforeClose(object Doc, ref bool Cancel) {
            DocumentBeforeClose?.Invoke(GetDocument(Doc), ref Cancel);
        }

        internal void OnDocumentBeforePrint(object Doc, ref bool Cancel) {
            DocumentBeforePrint?.Invoke(GetDocument(Doc), ref Cancel);
        }

        internal void OnDocumentBeforeSave(object Doc, ref bool SaveAsUI, ref bool Cancel) {
            DocumentBeforeSave?.Invoke(GetDocument(Doc), ref SaveAsUI, ref Cancel);
        }

        internal void OnNewDocument(object Doc) {
            NewDocument?.Invoke(GetDocument(Doc));
        }

        internal void OnWindowActivate(object Doc, object Wn) {
            WindowActivate?.Invoke(GetDocument(Doc), GetWindow(Wn));
        }

        internal void OnWindowDeactivate(object Doc, object Wn) {
            WindowDeactivate?.Invoke(GetDocument(Doc), GetWindow(Wn));
        }

        internal void OnWindowSelectionChange(object Sel) {
            WindowSelectionChange?.Invoke(GetSelection(Sel));
        }

        internal void OnWindowBeforeRightClick(object Sel, ref bool Cancel) {
            WindowBeforeRightClick?.Invoke(GetSelection(Sel), ref Cancel);
        }

        internal void OnWindowBeforeDoubleClick(object Sel, ref bool Cancel) {
            WindowBeforeDoubleClick?.Invoke(GetSelection(Sel), ref Cancel);
        }

    }
}
