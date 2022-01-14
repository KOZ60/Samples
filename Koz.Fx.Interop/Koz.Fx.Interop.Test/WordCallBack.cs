using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koz.Fx.Interop.Test
{
    class WordCallBack
    {
        public WordCallBack(Word.Application app) {
            app.Startup += App_Startup;
            app.DocumentChange += App_DocumentChange;
            app.QuitEvent += App_QuitEvent;
            app.DocumentOpen += App_DocumentOpen;
            app.DocumentBeforeClose += App_DocumentBeforeClose;
            app.DocumentBeforePrint += App_DocumentBeforePrint;
            app.DocumentBeforeSave += App_DocumentBeforeSave;
            app.NewDocument += App_NewDocument;
            app.WindowActivate += App_WindowActivate;
            app.WindowDeactivate += App_WindowDeactivate;
            app.WindowSelectionChange += App_WindowSelectionChange;
            app.WindowBeforeRightClick += App_WindowBeforeRightClick;
            app.WindowBeforeDoubleClick += App_WindowBeforeDoubleClick;
        }

        private void App_Startup() {
            throw new NotImplementedException();
        }

        private void App_DocumentChange() {
            //throw new NotImplementedException();
        }

        private void App_QuitEvent() {
            //throw new NotImplementedException();
        }

        private void App_DocumentOpen(Word.Document Doc) {
            //throw new NotImplementedException();
        }

        private void App_DocumentBeforeClose(Word.Document Doc, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_DocumentBeforePrint(Word.Document Doc, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_DocumentBeforeSave(Word.Document Doc, ref bool SaveAsUI, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_NewDocument(Word.Document Doc) {
            //throw new NotImplementedException();
        }

        private void App_WindowActivate(Word.Document Doc, Word.Window Wn) {
            //throw new NotImplementedException();
        }

        private void App_WindowDeactivate(Word.Document Doc, Word.Window Wn) {
            //throw new NotImplementedException();
        }

        private void App_WindowSelectionChange(Word.Selection Sel) {
            //throw new NotImplementedException();
        }

        private void App_WindowBeforeRightClick(Word.Selection Sel, ref bool Cancel) {
            //throw new NotImplementedException();
        }

        private void App_WindowBeforeDoubleClick(Word.Selection Sel, ref bool Cancel) {
            //throw new NotImplementedException();
        }
    }
}
