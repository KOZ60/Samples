using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatchProxyTest
{
    internal class ControlIntercepter : IInterceptor
    {
        public void Intercept(IInvocation invocation) {
            invocation.Proceed();
        }
    }
}
