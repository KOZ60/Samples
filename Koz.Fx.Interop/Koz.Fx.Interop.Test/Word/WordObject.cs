using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Koz.Fx.Interop.Test.Word
{
    public class WordObject : ComWrapper
    {
        private static readonly Dictionary<string, ConstructorInfo> Constructors;

        static WordObject() {
            Constructors = new Dictionary<string, ConstructorInfo>();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            ParameterModifier[] modifiers = new ParameterModifier[0];
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (Type type in asm.GetTypes()) {
                if (type.IsSubclassOf(typeof(WordObject)) && !type.IsAbstract && !type.IsGenericType) {
                    ConstructorInfo ci = type.GetConstructor(flags, null, new Type[] { typeof(object) }, modifiers);
                    if (ci != null) {
                        Debug.Print(type.Name);
                        Constructors.Add(type.Name, ci);
                    }
                }
            }
        }

        public WordObject(object comObject) : base(comObject) { }

        protected override sealed ComWrapper CreateComWrapper(object comObject, string comTypeName) {
            if (Constructors.TryGetValue(comTypeName, out ConstructorInfo ci)) {
                return (ComWrapper)ci.Invoke(new object[] { comObject });
            }
            switch (comTypeName) {
                case "Documents": return new Documents(comObject);
                case "Document": return new Document(comObject);
                case "Selection": return new Selection(comObject);
                case "Window": return new Window(comObject);
            }
            return new WordObject(comObject);
        }

        Application _Application;
        WordObject _Parent;

        public Application Application {
            get {
                if (_Application == null) {
                    _Application = GetWrapperProperty<Application>("Application");
                }
                return _Application;
            }
        }

        public WordObject Parent {
            get {
                if (_Parent == null) {
                    _Parent = GetWrapperProperty<WordObject>("Parent");
                }
                return _Parent;
            }
        }

        public int Creator {
            get {
                return GetIntProperty("Creator");
            }
        }

        protected Document GetDocument(object o) {
            return (Document)GetComWrapper(o);
        }

        protected Selection GetSelection(object o) {
            return (Selection)GetComWrapper(o);
        }

        protected Window GetWindow(object o) {
            return (Window)GetComWrapper(o);
        }

    }
}
