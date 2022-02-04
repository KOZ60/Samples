namespace Koz.Fx.Interop.Test.Word
{
    public class Document : WordObject
    {
        internal Document(object comObject) : base(comObject) { }

        public string Name {
            get {
                return GetStringProperty("Name");
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}
