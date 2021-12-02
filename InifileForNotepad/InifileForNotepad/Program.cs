using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace IniFileForNotepad
{
    class Program
    {
        /// <summary>
        /// メモ帳が保存するファイルのデフォルトが UTF8N になったので
        /// 保存可能なファイル形式に対応したクラスを作りました。
        /// ファイル形式を判定し、BOM付きUTF16に変換してから IniFile 関連の API を実行します。
        /// </summary>
        static void Main(string[] args) {
            var rootDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var iniFilesDir = Path.Combine(rootDir, "IniFiles");
            var iniFiles = Directory.GetFiles(iniFilesDir);

            Array.Sort(iniFiles);

            Console.Write(@"==============================================");
            Console.Write(@"スタティックメソッドテスト");
            Console.Write(@"==============================================");
            foreach (string fileName in iniFiles) {
                StaticTest(fileName);
            }
            Console.Write(@"何かキーを押してください。");
            Console.ReadKey();

            Console.WriteLine(@"==============================================");
            Console.WriteLine(@"インスタンスメンバテスト");
            Console.WriteLine(@"==============================================");
            foreach (string fileName in iniFiles) {
                InstanceTest(fileName);
            }
            Console.Write(@"何かキーを押してください。");
            Console.ReadKey();
        }

        static void StaticTest(string fileName)
        {
            Console.WriteLine(@"----------------------------------------------");
            Console.WriteLine(Path.GetFileName(fileName));
            Console.WriteLine(@"----------------------------------------------");
            var builder = new StringBuilder(1024);
            MLangInifile.GetPrivateProfileString("Encoding", "Lang", string.Empty, builder, builder.Capacity, fileName);
            int count = MLangInifile.GetPrivateProfileInt("Write", "Count", 0, fileName);
            Console.WriteLine("Lang={0} count={1}", builder, count);
            count++;
            MLangInifile.WritePrivateProfileString("Write", "Count", count.ToString(), fileName);
            string buffer = string.Format("WritePrivateProfileSectionのテスト{0}", count);
            MLangInifile.WritePrivateProfileSection("Section", buffer, fileName);
            MLangInifile.GetPrivateProfileSection("Section", builder, builder.Capacity, fileName);
            Console.WriteLine(builder.ToString());
        }

        static void InstanceTest(string fileName)
        {
            Console.WriteLine(@"----------------------------------------------");
            Console.WriteLine(Path.GetFileName(fileName));
            Console.WriteLine(@"----------------------------------------------");
            using (var inifile = new MLangInifile(fileName)) {
                var builder = new StringBuilder(1024);
                inifile.GetPrivateProfileString("Encoding", "Lang", string.Empty, builder, builder.Capacity);
                int count = inifile.GetPrivateProfileInt("Write", "Count", 0);
                Console.WriteLine("Encodings={0} Lang={1} count={2}", inifile.CharacterSet, builder, count);
                count++;
                inifile.WritePrivateProfileString("Write", "Count", count.ToString());
                string buffer = string.Format("WritePrivateProfileSectionのテスト{0}", count);
                inifile.WritePrivateProfileSection("Section", buffer);
                inifile.GetPrivateProfileSection("Section", builder, builder.Capacity);
                Console.WriteLine(builder.ToString());
            }
        }
    }
}
