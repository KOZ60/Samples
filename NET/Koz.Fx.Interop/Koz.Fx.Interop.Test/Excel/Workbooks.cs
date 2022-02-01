namespace Koz.Fx.Interop.Test.Excel
{
    public class Workbooks : ExcelCollection<Workbook>
    {
        internal Workbooks(object comObject) : base(comObject) { }

        public Workbook Add() {
            return (Workbook)Invoke("Add");
        }
    }
}
