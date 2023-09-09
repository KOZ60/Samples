using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Windows.Forms;

/// <summary>
/// ひな形となるクラス。定型的です。
/// </summary>
public class BaseProxy : RealProxy
{
    private readonly object instance;

    public BaseProxy(object target, Type type) : base(type) {
        instance = target;
    }

    public sealed override IMessage Invoke(IMessage msg) {
        try {
            IMethodMessage mm = msg as IMethodMessage;
            object[] args = mm.Args;
            MethodInfo method = (MethodInfo)mm.MethodBase;
            object ret = MethodInvoke(instance, method, args);
            return new ReturnMessage(
                ret, args, args.Length, mm.LogicalCallContext, (IMethodCallMessage)msg);

        } catch (Exception ex) {
            if (ex.InnerException != null)
                return new ReturnMessage(ex.InnerException, (IMethodCallMessage)msg);
            return new ReturnMessage(ex, (IMethodCallMessage)msg);
        }
    }

    protected virtual object MethodInvoke(object instance, MethodInfo mi, object[] args) {
        return mi.Invoke(instance, args);
    }
}

public class ControlProxy : BaseProxy
{
    private ControlProxy(Control target) : base(target, target.GetType()) { }

    /// <summary>
    /// MethodInvoke メソッドを override してカスタマイズします。
    /// (1) 他のスレッドからの呼び出しは Control.Invoke を介して行います。
    /// (2) ByRef で Control な引数は透過プロキシに置き換えます。
    /// </summary>
    protected override object MethodInvoke(object instance, MethodInfo mi, object[] args) {
        var control = (Control)instance;
        if (control.InvokeRequired) {
            Func<object, MethodInfo, object[], object> d = MethodInvoke;
            return control.Invoke(d, control, mi, args);
        }
        var result = base.MethodInvoke(control, mi, args);
        var parameters = mi.GetParameters();
        for (int i = 0; i < parameters.Length; i++) {
            var parameter = parameters[i];
            if (parameter.ParameterType.IsByRef && args[i] is Control ctl) {
                args[i] = GetTransparentProxy(ctl);
            }
        }
        return result;
    }

    public static T GetTransparentProxy<T>(T target) where T : Control {
        var proxy = new ControlProxy(target);
        return (T)proxy.GetTransparentProxy();
    }
}
