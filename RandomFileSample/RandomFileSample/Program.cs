using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace RandomFileSample
{
    static class Program
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct Record
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public char[] Item1;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public char[] Item2;
        }

        static void Main(string[] args)
        {
            string fileName = @"TEST.bin";
            int recordSize = Marshal.SizeOf(typeof(Record));

            var record = new Record();
            record.Item1 = "ABCDE".ToArray();
            record.Item2 = "FGH".ToArray();

            using (var randomFile = new RandomFile(fileName, recordSize)) {
                randomFile.Put(1, ref record);
            }

            record.Item1 = string.Empty.ToArray();
            using (var randomFile = new RandomFile(fileName, recordSize)) {
                randomFile.Get(1, ref record);
            }

            System.Diagnostics.Debug.Print(new string(record.Item1));
            System.Diagnostics.Debug.Print(new string(record.Item2));

            Record[] records = new Record[10];
            using (var randomFile = new RandomFile(fileName, recordSize)) {
                randomFile.Get(1, records);
            }

            for (int i = 0; i < records.Length; i++) {
                records[i].Item1 = "ABCDE".ToArray();
                records[i].Item2 = "FGH".ToArray();
            }

            using (var randomFile = new RandomFile(fileName, recordSize)) {
                randomFile.Put(1, records);
            }
        }
    }
}
