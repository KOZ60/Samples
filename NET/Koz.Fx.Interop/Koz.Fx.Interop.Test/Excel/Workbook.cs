namespace Koz.Fx.Interop.Test.Excel
{
    public class Workbook : ExcelObject
    {
        internal Workbook(object comObject) : base(comObject) { }

        public string Name {
            get {
                return GetStringProperty("Name");
            }
        }
    }
}
