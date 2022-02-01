Koz.Fx.Interop の使い方

COM オブジェクトをラップし、Marshal.ReleaseComObject を実行することなく、安全に解放を行うラッパークラスを作成できます。
Excel を例に、ラッパークラスの作成方法を説明します。

1.  基底クラスの作成

(1) ComWrapper を継承し、基底クラス(ExcelObject)を作成します。

    class ExcelObject : ComWrapper
    {
        public ExcelObject(object comObject) : base(comObject) { }

    どの Excel オブジェクトも以下のプロパティ、メソッドを持っているようです。
        ・Application プロパティ
        ・Parent プロパティ
        ・Creator プロパティ
        ・Copy メソッド

(2) 基底クラスは CreateComWrapper をオーバーライドし、各オブジェクトのインスタンスを作成します。
    他のクラスが書き換えないようシールし、オブジェクトを一元管理します。
    comTypeName に名前が渡されるので、それに応じたオブジェクトを作成してください。 

    protected override sealed ComWrapper CreateComWrapper(object comObject, string comTypeName) {
        switch (comTypeName) {
            case "Workbooks": return new Workbooks(comObject);
            case "Workbook": return new Workbook(comObject);
        }
        return new ExcelObject(comObject);
    }

2.  ルートクラスの作成

    Application クラスを作成します。
    InteropUtils.CreateObject を使用して、COM オブジェクトのインスタンスを作成してください。

    class Application : ExcelObject
    {
        public Application() : base(InteropUtils.CreateObject("Excel.Application")) { }


3. メソッドの実装

    メソッド実装用に以下のメソッドが用意されています。

        protected object Invoke(string name, params object[] args) 

        (使用例)
        public Workbook Add() {
            return (Workbook)Invoke("Add");
        }

4. プロパティの取得および設定

(1) プロパティ取得用に以下のメソッドが用意されています。

        protected object GetProperty(string name, params object[] indices)
        protected int GetIntProperty(string name, params object[] indices) 
        protected string GetStringProperty(string name, params object[] indices) 
        protected T GetProperty<T>(string name, params object[] indices) where T : struct 
        protected T GetWrapperProperty<T>(string propertyName, params object[] indices) where T : ComWrapper

        GetWrapperProperty は COM オブジェクトを取得し、ラッパークラスを返します。

(2) プロパティ設定用に以下のメソッドが用意されています。

        protected void SetProperty(string name, object value, params object[] indices)
        
        (使用例)
        public bool Visible {
            get { return GetProperty<bool>("Visible"); }
            set { SetProperty("Visible", value); }
        }


5. イベントの捕捉

(1) COM からのイベントを受け取るために、シンクオブジェクトのクラスを作成します。
    インターフェイスを定義し、シンクオブジェクトに実装します。

    ① インターフェイス

    [ComImport]
    [Guid(AppEvents.EventsId)]   // シンクオブジェクトの定数
    public interface IAppEvents
    {
        [DispId(2612)]
        [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AfterCalculate();

    定義については OLE/COM Object Viewer などで確認してください。

    ② シンクオブジェクト

    [ComVisible(true)]
    public class AppEvents : SinkObject, IAppEvents
    {
        public const string EventsId = "00024413-0000-0000-C000-000000000046";
        public static Guid Guid = new Guid(EventsId);

        internal AppEvents() : base(Guid) { }

        internal Application Application {
            get { return (Application)Owner; }
        }


    このクラスは COM からの通知を受け取るため、[ComVisible(true)] 属性を付け、public である必要があります。
    コンストラクタの引数にはイベント発生元のラッパークラスと ComTypes.IConnectionPoint を与えます。


(2) イベントを補足するためのメソッドを作成します。

        [DispId(2612)]
        public void AfterCalculate() {
            Application.OnAfterCalculate();
        }

        [DispId(1565)]
        public void NewWorkbook([In] object Wb) {
            Application.OnNewWorkbook(Wb);
        }

    Excel の場合、インターフェイスは必要ありません。ただし、シンクオブジェクトのメソッドに DispId を与える必要があるようです。
    Word の場合、インターフェイスが必要で、シンクオブジェクトのメソッドに DispId は不要です。


(3) イベント発生元のクラスでは AddSink メソッドを使ってシンクオブジェクトを追加します。

    public class Application : ExcelObject
    {
        public Application() : base(InteropUtils.CreateObject("Excel.Application")) {
            AddSink(new AppEvents());
        }

(4) イベント発生元のクラスにシンクオブジェクトからのコールバック関数とイベントを定義します。 

    public delegate void NewWorkbookDelegate(Workbook wb);
    public event NewWorkbookDelegate NewWorkbook;

    internal void OnNewWorkbook(object wb) {
        NewWorkbook?.Invoke((Workbook)GetComWrapper(wb));
    }

    COM オブジェクトが渡された場合は、GetComWrapper メソッドを実行し、ComWrapper オブジェクトをアプリケーションに渡すようにしてください。

99. その他注意事項

(1) Application.Quit メソッド

    Application.Quit メソッドは Dispose を呼ぶようにすると良いでしょう。
    Dispose(bool) メソッドの中で Invoke します。

    public void Quit() {
        Dispose();
    }

    protected override void Dispose(bool disposing) {
        DisplayAlerts = false;
        Invoke("Quit");
        base.Dispose(disposing);
    }

(2) コレクションオブジェクトについて
    コレクションオブジェクトについては一度作成したらキャッシュしておきます。

    public Workbooks Workbooks {
        get {
            if (workbooks == null) {
                workbooks = GetWrapperProperty<Workbooks>("Workbooks");
            }
            return workbooks;
        }
    }

(3) Word Quit イベント

    Word の場合、アプリケーションを閉じると Quit イベントが発生し、exe が終了してしまいます。
    コールバック関数に Dispose() を入れておきます。

    internal void OnQuit() {
        QuitEvent?.Invoke();
        Dispose();
    }

    Excel の場合は、アプリケーションを閉じても exe は終了しないので必要ありません。

