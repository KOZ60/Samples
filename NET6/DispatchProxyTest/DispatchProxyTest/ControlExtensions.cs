using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace DispatchProxyTest
{
    internal static class ControlExtensions
    {
        public static T CreateProxy<T>(this T control) where T : Control {
            object proxy = DispatchProxy.Create<T, ControlProxy<T>>();
            ((ControlProxy<T>)proxy).SetInstance(control);
            return (T)proxy;
        }

        private class ControlProxy<T> : DispatchProxy where T : Control
        {
            private T? _Instance = null;

            public ControlProxy() { }

            public void SetInstance(T instance) {
                _Instance = instance;
            }

            protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) {
                try {
                    if (_Instance != null) {
                        if (_Instance.InvokeRequired) {
                            return _Instance.Invoke(() => { return targetMethod?.Invoke(_Instance, args); });
                        } else {
                            return targetMethod?.Invoke(_Instance, args);
                        }
                    } else {
                        return targetMethod?.Invoke(null, args);
                    }
                } catch (Exception ex) {
                    if (ex.InnerException == null) {
                        throw;
                    } else {
                        throw ex.InnerException;
                    }
                }
            }
        }



    }
}
