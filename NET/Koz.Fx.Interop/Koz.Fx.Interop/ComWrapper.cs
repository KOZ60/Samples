using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Koz.Fx.Interop
{
    /// <summary>
    /// COM オブジェクトのラッパークラスです。
    /// </summary>
    public abstract class ComWrapper : DisposableObject
    {
        private ComWrapper root;
        private ComWrapperCollection wrappers;
        private List<SinkObject> sinkObjects;

        /// <summary>
        /// COM のラッパークラスを作成します。
        /// </summary>
        /// <param name="comObject">ラップする COM オブジェクト</param>
        protected ComWrapper(object comObject) {
            ComObject = new SafeComObject(comObject);
        }

        /// <summary>
        /// イベントをキャッチするためのシンクオブジェクトを追加します。
        /// </summary>
        /// <param name="sink">SinkObject クラスを継承して作成したイベントをキャッチするためのシンクオブジェクト。</param>
        public void AddSink(SinkObject sink) {
            sink.Advise(this);
            SinkObjects.Add(sink);
        }

        /// <summary>
        /// AddSink で追加したシンクオブジェクトを解除します。
        /// </summary>
        /// <param name="sink">SinkObject クラスを継承して作成したイベントをキャッチするためのシンクオブジェクト。</param>
        public void RemoveSink(SinkObject sink) {
            SinkObjects.Remove(sink);
            sink.Unadvise();
        }

        private bool HasSinkObjects {
            get {
                return (sinkObjects != null) && (sinkObjects.Count > 0);
            }
        }

        private List<SinkObject> SinkObjects {
            get {
                if (sinkObjects == null) {
                    sinkObjects = new List<SinkObject>();
                }
                return sinkObjects;
            }
        }

        /// <summary>
        /// ラッパークラスのインスタンスを作成するメソッドです。
        /// 基底クラスは CreateComWrapper をオーバーライドし、各オブジェクトのインスタンスを作成します。
        /// </summary>
        /// <param name="comObject">ラップする COM オブジェクト。</param>
        /// <param name="comTypeName">COM オブジェクトのタイプ名。</param>
        /// <returns>作成したラッパークラスのインスタンス。</returns>
        protected abstract ComWrapper CreateComWrapper(object comObject, string comTypeName);

        internal SafeComObject ComObject { get; }

        internal ComWrapper Root { 
            get {
                if (root == null) {
                    root = this;
                    wrappers = new ComWrapperCollection();
                    wrappers.Add(this);
                }
                return root;
            }
            set {
                root = value;
                root.wrappers.Add(this);
            }
        }

        /// <summary>
        /// ComWrapper クラスが管理しているラッパークラスのコレクションからラッパークラスを取り出します。
        /// コレクションに無い場合は、CreateComWrapper メソッドを呼び出し、インスタンスを作成します。
        /// </summary>
        /// <param name="comObject">ラップする COM オブジェクト。</param>
        /// <returns>comObject に対応したラッパークラスのインスタンス。</returns>
        protected ComWrapper GetComWrapper(object comObject) {
            using (SafeComObject key = new SafeComObject(comObject)) {
                if (Wrappers.TryGetValue(key, out ComWrapper comWrapper)) {
                    if (!comWrapper.IsDisposed) {
                        return comWrapper;
                    } else {
                        Wrappers.Remove(comWrapper);
                    }
                }
                ComWrapper wrapper = CreateComWrapper(key.Target, key.TypeName);
                wrapper.Root = Root;
                key.OwnsHandle = false; // comObject を解放しない
                return wrapper;
            }
        }

        private ComWrapperCollection Wrappers {
            get {
                return Root.wrappers;
            }
        }

        /// <summary>
        /// プログラムから参照する様々な情報を格納します。
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 使用されているアンマネージ リソースを解放し、オプションでマネージ リソースも解放します。
        /// </summary>
        /// <param name="disposing">マネージ リソースとアンマネージ リソースの両方を解放する場合は true。アンマネージ リソースだけを解放する場合は false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (HasSinkObjects) {
                    foreach (var so in SinkObjects) {
                        so.Dispose();
                    }
                }
                Wrappers.Remove(this);
                if (wrappers != null) {
                    // 新しいものから Dispose
                    var array = wrappers.ToList();
                    for (int i = array.Count - 1; i >= 0; i--) {
                        array[i].Dispose();
                    }
                }
            }
            ComObject.Dispose();
        }

        /// <summary>
        /// 現在のオブジェクトを表す文字列を返します。
        /// </summary>
        /// <returns>現在のオブジェクトを表す文字列。</returns>
        public override string ToString() {
            if (Tag == null) {
                return GetType().Name;
            }
            return string.Format("{0} -{1}-", GetType().Name, Tag);
        }

        /// <summary>
        /// 名前と引数を指定してプロパティを取得します。
        /// </summary>
        /// <param name="name">取得するプロパティの名前。</param>
        /// <param name="indices">取得する際に必要な引数。</param>
        protected object GetProperty(string name, params object[] indices) {
            return InteropUtils.GetProperty<object>(ComObject, name, indices);
        }

        /// <summary>
        /// 名前と引数を指定してプロパティを取得し指定した型へキャストします。
        /// </summary>
        /// <typeparam name="T">プロパティの値をキャストする型</typeparam>
        /// <param name="name">取得するプロパティの名前。</param>
        /// <param name="indices">プロパティを取得する際に必要な引数。</param>
        protected T GetProperty<T>(string name, params object[] indices) where T : struct {
            return InteropUtils.GetProperty<T>(ComObject, name, indices);
        }

        /// <summary>
        /// 名前と引数を指定してプロパティを取得し int 型に変換します。
        /// </summary>
        /// <param name="name">取得するプロパティの名前。</param>
        /// <param name="indices">プロパティを取得する際に必要な引数。</param>
        protected int GetIntProperty(string name, params object[] indices) {
            object value = InteropUtils.GetProperty<object>(ComObject, name, indices);
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// 名前と引数を指定して String 型のプロパティを取得します。
        /// </summary>
        /// <param name="name">取得するプロパティの名前。</param>
        /// <param name="indices">プロパティを取得する際に必要な引数。</param>
        protected string GetStringProperty(string name, params object[] indices) {
            return InteropUtils.GetProperty<string>(ComObject, name, indices);
        }

        /// <summary>
        /// 名前と引数を指定してプロパティに対するラッパークラスのインスタンスを取得します。
        /// </summary>
        /// <typeparam name="T">取得するプロパティの型</typeparam>
        /// <param name="propertyName">取得するプロパティの名前。</param>
        /// <param name="indices">プロパティを取得する際に必要な引数。</param>
        /// <returns>ラッパークラスのインスタンス。</returns>
        protected T GetWrapperProperty<T>(string propertyName, params object[] indices) where T : ComWrapper {
            object o = GetProperty(propertyName, indices);
            if (Marshal.IsComObject(o)) {
                return (T)GetComWrapper(o);
            }
            return o as T;
        }

        /// <summary>
        /// 名前と引数を指定してプロパティを設定します。
        /// </summary>
        /// <param name="name">設定するプロパティの名前。</param>
        /// <param name="value">設定するプロパティの値。</param>
        /// <param name="indices">設定する際に必要な引数。</param>
        protected void SetProperty(string name, object value, params object[] indices) {
            InteropUtils.SetProperty(ComObject, name, value, indices);
        }

        /// <summary>
        /// 名前を指定してメソッドを実行します。
        /// </summary>
        /// <param name="name">実行するメソッドの名前。</param>
        /// <param name="args">メソッドに渡す引数。</param>
        /// <returns>メソッドの戻り値。</returns>
        protected object Invoke(string name, params object[] args) {
            return TryWrapper(InteropUtils.Invoke(ComObject, name, args));
        }

        internal object TryWrapper(object o) {
            if (o != null && Marshal.IsComObject(o)) {
                return GetComWrapper(o);
            } else {
                return o;
            }
        }
    }
}
