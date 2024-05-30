namespace SingleInstance
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    internal static class Program
    {
        private const string TARGET_PROGRAM = "notepad2.exe";

        [STAThread]
        static void Main(string[] args) {
            var mutex = new Mutex(false, NativeMethods.GUID);
            try {
                // Mutex を獲得できるまで終了要求をポストする
                while (!mutex.WaitOne(100)) {
                    IntPtr hwnd = NativeMethods.FindWindow(null, NativeMethods.GUID);
                    if (hwnd != IntPtr.Zero) {
                        NativeMethods.PostMessage(
                            hwnd, NativeMethods.WM_APPLICATION_EXIT,
                            IntPtr.Zero, IntPtr.Zero);
                    }
                }

                // ターゲットを起動
                using (var target = StartProcess(TARGET_PROGRAM, args)) {

                    // ターゲットが終了したら自身を終了する
                    target.EnableRaisingEvents = true;
                    target.Exited += (s, e) => {
                        Application.Exit();
                    };

                    try {
                        // 通知受信用フォームの表示
                        Application.Run(new NotifyForm());

                    } finally {
                        // ターゲットが生きていたら終了する
                        if (!target.HasExited) {
                            target.Kill(); 
                            // または target.MainWindowHandle を閉じる
                            // (閉じても終了するとは限らない)
                        }
                        target.WaitForExit();
                    }
                }

            } finally {
                // 終了前に Mutex を開放
                mutex.ReleaseMutex();
            }
        }

        static Process StartProcess(string fileName, string[] args) {
            var info = new ProcessStartInfo {
                FileName = fileName,
                Arguments = CreateArguments(args),
                UseShellExecute = false,
                CreateNoWindow = false
            };
            return Process.Start(info);
        }

        // 与えられた引数を起動するプログラムに渡すため編集
        static string CreateArguments(string[] args) {
            var sb = new StringBuilder();
            foreach (var str in args) {
                if (sb.Length > 0) {
                    sb.Append(" ");
                }
                // 「"」で引数をくくり、エスケープする
                sb.Append("\"" + str.Replace("\"", "\"\"") + "\"");
            }
            return sb.ToString();
        }
    }

    static class Program2 {
        [STAThread]
        static void Main1() {
            var mutex = new Mutex(false, NativeMethods.GUID);
            try {
                // Mutex を獲得できるまで終了要求をポストする
                while (!mutex.WaitOne(100)) {
                    IntPtr hwnd = NativeMethods.FindWindow(null, NativeMethods.GUID);
                    if (hwnd != IntPtr.Zero) {
                        NativeMethods.PostMessage(
                            hwnd, NativeMethods.WM_APPLICATION_EXIT,
                            IntPtr.Zero, IntPtr.Zero);
                    }
                }

                // ここから通常の処理
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // 通知受信用フォームの作成
                var notifyForm = new NotifyForm();
                notifyForm.Show();

                // メインフォーム表示
                Application.Run(new Form1());

            } finally {
                // 終了前に Mutex を開放
                mutex.ReleaseMutex();
            }
        }
    }
}
