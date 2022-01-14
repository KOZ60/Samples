using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Koz.Fx.Interop
{
    /// <summary>
    /// Interop ユーティリティ
    /// </summary>
    public static class InteropUtils
    {
        private readonly static string[] NoArgumentNames = new string[0];

        private const BindingFlags
            GetMemberBindingFlags =
                                    BindingFlags.Instance |
                                    BindingFlags.Public |
                                    BindingFlags.NonPublic |
                                    BindingFlags.IgnoreCase |
                                    BindingFlags.FlattenHierarchy;

        private const BindingFlags
            InvokeGetPropertyBindingFlags =
                                    GetMemberBindingFlags |
                                    BindingFlags.InvokeMethod |
                                    BindingFlags.OptionalParamBinding |
                                    BindingFlags.GetProperty;

        private const BindingFlags
            InvokeSetPropertyBindingFlags =
                                    GetMemberBindingFlags |
                                    BindingFlags.SetProperty;

        private const BindingFlags
            InvokeMemberBindingFlags =
                                    GetMemberBindingFlags |
                                    BindingFlags.InvokeMethod |
                                    BindingFlags.OptionalParamBinding;

        internal static T GetProperty<T>(SafeComObject o, string name, params object[] indices) {
            IReflect reflectType = o.Target.GetType();
            ArrayList lst = UnwrapArrayList(indices);
            object obj = reflectType.InvokeMember(
                                name,
                                InvokeGetPropertyBindingFlags,
                                null,
                                o.Target,
                                lst.ToArray(),
                                null,
                                null,
                                NoArgumentNames);
            return (T)obj;
        }

        internal static void SetProperty(SafeComObject o, string name, object value, params object[] indices) {

            // InvokeMember に渡す配列は パラメタ + プロパティの設定値
            ArrayList lst = UnwrapArrayList(indices);
            lst.Add(Unwrap(value));

            IReflect reflectType = o.Target.GetType();
            reflectType.InvokeMember(
                        name,
                        InvokeSetPropertyBindingFlags,
                        null,
                        o.Target,
                        lst.ToArray(),
                        null,
                        null,
                        NoArgumentNames);
        }

        internal static object Invoke(SafeComObject o, string name, params object[] args) {
            IReflect reflectType = o.Target.GetType();
            ArrayList lst = UnwrapArrayList(args);
            object result = reflectType.InvokeMember(
                        name,
                        InvokeMemberBindingFlags,
                        null,
                        o.Target,
                        lst.ToArray(),
                        null,
                        null,
                        NoArgumentNames);
            return result;
        }

        private static ArrayList UnwrapArrayList(object[] args) {
            ArrayList lst = new ArrayList(args.Length + 1);
            for (int i = 0; i < args.Length; i++) {
                lst.Add(Unwrap(args[i]));
            }
            return lst;
        }

        /// <summary>
        /// COM オブジェクトのインスタンスを作成します。
        /// </summary>
        /// <param name="ProgId"> 作成するオブジェクトのアプリケーション名とクラス名。</param>
        /// <returns>作成された COM オブジェクトのインスタンス。</returns>
        public static object CreateObject(string ProgId) {
            return Activator.CreateInstance(Type.GetTypeFromProgID(ProgId));
        }

        /// <summary>
        /// ComWrapper のラッピングを解除し、COM オブジェクトを取り出します。
        /// </summary>
        /// <param name="o">ラッピングを解除するオブジェクト。</param>
        /// <returns>ラッピング解除後の COM オブジェクト。</returns>
        public static object Unwrap(object o) {
            ComWrapper wrapper = o as ComWrapper;
            if (wrapper != null) {
                return Unwrap(wrapper);
            }
            SafeComObject comObject = o as SafeComObject;
            if (comObject != null) {
                return Unwrap(comObject);
            }
            return o;
        }

        /// <summary>
        /// ComWrapper のラッピングを解除し、COM オブジェクトを取り出します。
        /// </summary>
        /// <param name="wrapper">ラッピングを解除するオブジェクト。</param>
        /// <returns>ラッピング解除後の COM オブジェクト。</returns>
        public static object Unwrap(ComWrapper wrapper) {
            return wrapper.ComObject;
        }

        internal static object Unwrap(SafeComObject comObject) {
            return comObject.Target;
        }

        /// <summary>
        /// COM オブジェクトのタイプ名を取得します。
        /// </summary>
        /// <param name="o">タイプ名を取得する COM オブジェクト。</param>
        /// <returns>COM オブジェクトのタイプ名。</returns>
        public static string GetComTypeName(object o) {
            IDispatch disp = o as IDispatch;
            if (disp == null) {
                return "Type Unknown";
            }

            disp.GetTypeInfoCount(out uint uCount);
            if (uCount < 1) {
                return "No Type";
            }

            disp.GetTypeInfo(0, 0, out IntPtr typeInfoPtr);
            if (typeInfoPtr == IntPtr.Zero) {
                return "No Type*";
            }

            try {
                ComTypes.ITypeInfo typeInfo = (ComTypes.ITypeInfo)Marshal.GetTypedObjectForIUnknown(
                                                    typeInfoPtr, typeof(ComTypes.ITypeInfo));
                string typeName = Marshal.GetTypeInfoName(typeInfo);
                return typeName.TrimStart('_');

            } catch {
                return "Invalid ITypeInfo";

            } finally {
                Marshal.Release(typeInfoPtr);
            }
        }

        /// <summary>
        /// 省略可能な引数から COM オブジェクトに渡す値を取得します。
        /// </summary>
        public static object ToOptional<T>(this T value) {
            if (value == null) {
                return Type.Missing;
            }
            object o = Unwrap(value);
            return o ?? Type.Missing;
        }
    }
}
