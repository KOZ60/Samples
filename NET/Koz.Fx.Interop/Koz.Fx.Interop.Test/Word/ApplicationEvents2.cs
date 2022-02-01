using System;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop.Test.Word
{
    [ComVisible(true)]
    public class ApplicationEvents2 : SinkObject, IApplicationEvents2
    {
        public const string EventsId = "000209FE-0000-0000-C000-000000000046";
        public static Guid Guid = new Guid(EventsId);

        internal ApplicationEvents2() : base(Guid) { }

        internal Application Application {
            get { return (Application)Owner; }
        }

        public void Startup() {
            Application.OnStartup();
        }

        public void Quit() {
            Application.OnQuit();
        }

        public void DocumentChange() {
            Application.OnDocumentChange();
        }

        public void DocumentOpen([In, MarshalAs(UnmanagedType.Interface)] object Doc) {
            Application.OnDocumentOpen(Doc);
        }

        public void DocumentBeforeClose([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In] ref bool Cancel) {
            Application.OnDocumentBeforeClose(Doc, ref Cancel);
        }

        public void DocumentBeforePrint([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In] ref bool Cancel) {
            Application.OnDocumentBeforePrint(Doc, ref Cancel);
        }

        public void DocumentBeforeSave([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In] ref bool SaveAsUI, [In] ref bool Cancel) {
            Application.OnDocumentBeforeSave(Doc, ref SaveAsUI, ref Cancel);
        }

        public void NewDocument([In, MarshalAs(UnmanagedType.Interface)] object Doc) {
            Application.OnNewDocument(Doc);
        }

        public void WindowActivate([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In, MarshalAs(UnmanagedType.Interface)] object Wn) {
            Application.OnWindowActivate(Doc, Wn);
        }

        public void WindowDeactivate([In, MarshalAs(UnmanagedType.Interface)] object Doc, [In, MarshalAs(UnmanagedType.Interface)] object Wn) {
            Application.OnWindowDeactivate(Doc, Wn);
        }

        public void WindowSelectionChange([In, MarshalAs(UnmanagedType.Interface)] object Sel) {
            Application.OnWindowSelectionChange(Sel);
        }

        public void WindowBeforeRightClick([In, MarshalAs(UnmanagedType.Interface)] object Sel, [In] ref bool Cancel) {
            Application.OnWindowBeforeRightClick(Sel, ref Cancel);
        }

        public void WindowBeforeDoubleClick([In, MarshalAs(UnmanagedType.Interface)] object Sel, [In] ref bool Cancel) {
            Application.OnWindowBeforeDoubleClick(Sel, ref Cancel);
        }
    }
}
