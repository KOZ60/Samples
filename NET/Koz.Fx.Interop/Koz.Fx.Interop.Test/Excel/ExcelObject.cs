using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Koz.Fx.Interop.Test.Excel
{
    public class ExcelObject : ComWrapper
    {
        private static readonly Dictionary<string, ConstructorInfo> Constructors;

        static ExcelObject() {
            Constructors = new Dictionary<string, ConstructorInfo>();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            ParameterModifier[] modifiers = new ParameterModifier[0];
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type type in asm.GetTypes()) {
                if (type.IsSubclassOf(typeof(ExcelObject)) && !type.IsAbstract && !type.IsGenericType ) {
                    ConstructorInfo ci = type.GetConstructor(flags, null, new Type[] { typeof(object) }, modifiers);
                    if (ci != null) {
                        Debug.Print(type.Name);
                        Constructors.Add(type.Name, ci);
                    }
                }
            }
        }

        public ExcelObject(object comObject) : base(comObject) { }

        protected override sealed ComWrapper CreateComWrapper(object comObject, string comTypeName) {
            if (Constructors.TryGetValue(comTypeName, out ConstructorInfo ci)) {
                return (ComWrapper)ci.Invoke( new object[] { comObject });
            }
            Debug.Print("CreateComWrapper {0}", comTypeName);

            switch (comTypeName) {
                case "Range": return new Range(comObject);
                case "Workbooks": return new Workbooks(comObject);
                case "Workbook": return new Workbook(comObject);
                case "Chart": return new Chart(comObject);
                case "Hyperlink": return new Hyperlink(comObject);
                case "PivotTable": return new PivotTable(comObject);
                case "TableObject": return new TableObject(comObject);
                case "Window": return new Window(comObject);
                case "ModelChanges": return new ModelChanges(comObject);
                case "ProtectedViewWindow": return new ProtectedViewWindow(comObject);
                case "MsoSyncEventType": return new MsoSyncEventType(comObject);
            }
            return new ExcelObject(comObject);
        }

        Application _Application;
        ExcelObject _Parent;

        public Application Application {
            get {
                if (_Application == null) {
                    _Application = GetWrapperProperty<Application>("Application");
                }
                return _Application;
            }
        }

        public ExcelObject Parent {
            get {
                if (_Parent == null) {
                    _Parent = GetWrapperProperty<ExcelObject>("Parent");
                }
                return _Parent;
            }
        }

        public int Creator {
            get {
                return GetIntProperty("Creator");
            }
        }

        public void Copy() {
            Invoke("Copy");
        }

        protected ProtectedViewWindow GetProtectedViewWindow(object o) {
            return (ProtectedViewWindow)GetComWrapper(o);
        }

        protected Sheet GetSheet(object o) {
            return (Sheet)GetComWrapper(o);
        }

        protected Range GetRange(object o) {
            return (Range)GetComWrapper(o);
        }

        protected Window GetWindow(object o) {
            return (Window)GetComWrapper(o);
        }

        protected Workbook GetWorkbook(object o) {
            return (Workbook)GetComWrapper(o);
        }

        protected PivotTable GetPivotTable(object o) {
            return (PivotTable)GetComWrapper(o);
        }

        protected MsoSyncEventType GetMsoSyncEventType(object o) {
            return (MsoSyncEventType)GetComWrapper(o);
        }

        protected Hyperlink GetHyperlink(object o) {
            return (Hyperlink)GetComWrapper(o);
        }

        protected XmlMap GetXmlMap(object o) {
            return (XmlMap)GetComWrapper(o);
        }

        protected ModelChanges GetModelChanges(object o) {
            return (ModelChanges)GetComWrapper(o);
        }

        protected Chart GetChart(object o) {
            return (Chart)GetComWrapper(o);
        }

        protected TableObject GetTableObject(object o) {
            return (TableObject)GetComWrapper(o);
        }

    }
}
