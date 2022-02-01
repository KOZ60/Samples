using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koz.Fx.Interop.Test.Word
{
    public class Documents : WordCollection<Document>
    {
        internal Documents(object comObject) : base(comObject) { }

        public Document Add() {
            return (Document)Invoke("Add");
        }
    }
}
