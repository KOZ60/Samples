using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koz.Fx.Interop.Test.Word
{
    public class Window : WordObject
    {
        internal Window(object comObject) : base(comObject) { }
    }
}
