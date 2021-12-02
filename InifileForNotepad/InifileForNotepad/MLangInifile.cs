namespace IniFileForNotepad
{
    using System;
    using System.IO;
    using System.Text;

    public class MLangInifile : IDisposable
    {
        readonly string inifileName;
        readonly string targetFile;
        readonly Encoding encoding;
        readonly CharacterSet caracterSet;
        readonly bool useTempFile;
        bool writed = false;

        public MLangInifile(string fileName)
        {
            inifileName = fileName;
            targetFile = fileName;
            caracterSet = CharacterSet.Default;
            encoding = Encoding.Default;
            useTempFile = false;

            if (File.Exists(inifileName)) {

                // ファイルの内容をバイト配列に格納し、キャラクタセットを特定する

                byte[] bytes = File.ReadAllBytes(inifileName);
                caracterSet = UTL.DetectCharacterSet(bytes);
                encoding = UTL.Encodings[caracterSet];

                // ini ファイルの API は SJIS および UTF16 のファイルを読み書きできるので
                // それ以外は UTF16 にデコードしてテンポラリに書き込み操作する

                switch (caracterSet) {
                    case CharacterSet.Default:
                    case CharacterSet.SJIS:
                    case CharacterSet.UTF16:
                        break;

                    default:
                        useTempFile = true;
                        targetFile = Path.GetTempFileName();
                        // BOM を除いてデコード
                        int start = encoding.GetPreamble().Length;
                        int count = bytes.Length - start;
                        string buffer = encoding.GetString(bytes, start, count);
                        File.WriteAllText(targetFile, buffer, Encoding.Unicode);
                        break;
                }
            }
        }

        public CharacterSet CharacterSet {
            get {
                return caracterSet;
            }
        }

        public void Flush()
        {
            if (useTempFile && writed) {
                string buffer = File.ReadAllText(targetFile, Encoding.Unicode);
                File.WriteAllText(inifileName, buffer, encoding);
            }
            writed = false;
        }

        public void Dispose()
        {
            if (useTempFile) {
                Flush();
                if (File.Exists(targetFile)) {
                    File.Delete(targetFile);
                }
            }
        }

        public int GetPrivateProfileInt(
            string appName,
            string keyName,
            int nDefault
           )
        {
            return NativeMethods.GetPrivateProfileInt(
                                        appName,
                                        keyName,
                                        nDefault,
                                        targetFile);
        }

        public int GetPrivateProfileSection(
            string appName,
            StringBuilder builder,
            int nSize)
        {
            return NativeMethods.GetPrivateProfileSection(
                                        appName,
                                        builder,
                                        nSize,
                                        targetFile);
        }

        public int GetPrivateProfileString(
            string appName,
            string keyName,
            string defaultValue,
            StringBuilder builder,
            int nSize)
        {
            return NativeMethods.GetPrivateProfileString(
                                        appName,
                                        keyName,
                                        defaultValue,
                                        builder,
                                        nSize,
                                        targetFile);
        }

        public bool WritePrivateProfileSection(
            string lpAppName,
            string lpString)
        {
            var result = NativeMethods.WritePrivateProfileSection(
                                    lpAppName,
                                    lpString,
                                    targetFile);
            if (result) writed = true;
            return writed;
        }

        public bool WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString)
        {
            var result = NativeMethods.WritePrivateProfileString(
                                                lpAppName,
                                                lpKeyName,
                                                lpString,
                                                targetFile);
            if (result) writed = true;
            return result;
        }

        // -----------------------------------------------------
        // static methods
        // -----------------------------------------------------

        public static int GetPrivateProfileInt(
            string appName,
            string keyName,
            int nDefault,
            string fileName)
        {
            using (var inifile = new MLangInifile(fileName)) {
                return inifile.GetPrivateProfileInt(appName, keyName, nDefault);
            }
        }

        public static int GetPrivateProfileSection(
            string appName,
            StringBuilder builder,
            int nSize,
            string fileName)
        {
            using (var inifile = new MLangInifile(fileName)) {
                return inifile.GetPrivateProfileSection(appName, builder, nSize);
            }
        }

        public static int GetPrivateProfileString(
            string appName,
            string keyName,
            string defaultValue,
            StringBuilder builder,
            int nSize,
            string fileName)
        {
            using (var inifile = new MLangInifile(fileName)) {
                return inifile.GetPrivateProfileString(appName, keyName, defaultValue, builder, nSize);
            }
        }

        public static bool WritePrivateProfileSection(
            string appName,
            string value,
            string fileName)
        {
            using (var inifile = new MLangInifile(fileName)) {
                return inifile.WritePrivateProfileSection(appName, value);
            }
        }

        public static bool WritePrivateProfileString(
            string appName,
            string keyName,
            string value,
            string fileName)
        {
            using (var inifile = new MLangInifile(fileName)) {
                return inifile.WritePrivateProfileString(appName, keyName, value);
            }
        }
    }
}
