namespace DiskMultiThread
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        const int FILE_FLAG_NO_BUFFERING = 0x20000000;
        const int MAX_THREADS = 10;
        const int BUFFER_SIZE = 1024 * 1024 * 10; // 512 の整数倍でないとコケる

        static void Main(string[] args) {
            var targetDir = Environment.ExpandEnvironmentVariables(@"%WINDIR%\System32");
            var files = Directory.GetFiles(targetDir);
            Watch($"** Multi({MAX_THREADS}) **", () => { Multi(files, MAX_THREADS); });
            Watch("** Single **", () => { Single(files); });
            Console.ReadKey();
        }

        static void Watch(string message, Action action) {
            var sw = new Stopwatch();
            sw.Start();
            action.Invoke();
            sw.Stop();
            Console.WriteLine($"{message} {sw.ElapsedMilliseconds} ms");
        }

        static void Multi(string[] files, int threads) {
            ThreadPool.SetMinThreads(threads, threads);
            int allCount = files.Length;
            int splitCount = allCount / threads;
            int remainder = allCount % threads;
            int[] counts = new int[threads];
            for (int i = 0; i < threads; i++) {
                if (i < remainder) {
                    counts[i] = splitCount + 1;
                } else {
                    counts[i] = splitCount;
                }
            }
            Task[] tasks = new Task[threads];
            int position = 0;
            for (int i = 0; i < threads; i++) {
                int start = position;
                int count = counts[i];
                tasks[i] = Task.Run(() => { ReadFiles(files, start, count); });
                position += count;
            }
            Task.WaitAll(tasks);
        }

        static void Single(string[] files) {
            ReadFiles(files, 0, files.Length);
        }

        static void ReadFiles(string[] files, int start, int count) {
            Console.WriteLine($"start={start} count={count}");
            byte[] buffer = new byte[BUFFER_SIZE];
            for (int i = 0; i < count; i++) {
                ReadFile(files[start + i], buffer);
            }
        }

        static void ReadFile(string file, byte[] buffer) {
            FileOptions options = (FileOptions)FILE_FLAG_NO_BUFFERING;
            using (var stream = new FileStream(file, FileMode.Open,
                                        FileAccess.Read, FileShare.Read,
                                        BUFFER_SIZE, options)) {
                while (stream.Read(buffer, 0, BUFFER_SIZE) == BUFFER_SIZE) { }
            }
        }
    }
}
