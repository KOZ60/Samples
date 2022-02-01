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

        public MLangInifile(string fileName) : this(fileName, CharacterSet.Default) { }

        public MLangInifile(string fileName, CharacterSet forceCaracterSet)
        {
            inifileName = fileName;
            targetFile = fileName;
            caracterSet = CharacterSet.Default;
            encoding = Encoding.Default;
            useTempFile = false;

            byte[] bytes = null;

            if (File.Exists(inifileName)) {

                // ファイルの内容をバイト配列に格納

                bytes = File.ReadAllBytes(inifileName);

                // Default が指定されたらファイルの内容で自動判定
                if (forceCaracterSet == CharacterSet.Default) {
                    caracterSet = UTL.DetectCharacterSet(bytes);
                } else {
                    caracterSet = forceCaracterSet;
                }
            } else {
                caracterSet = forceCaracterSet;
            }

            // キャラクタセットから Encoding を取得
            encoding = UTL.Encodings[caracterSet];

            // ini ファイルの API は SJIS および UTF16 のファイルを読み書きできるので
            // それ以外は UTF16 にデコードしてテンポラリに書き込み操作する

            switch (caracterSet) {
                case CharacterSet.Default:
                case CharacterSet.SJIS:
                    break;

                case CharacterSet.UTF16:
                    // ファイルが存在しなければテンポラリを対象
                    if (bytes == null) useTempFile = true;
                    break;

                default:
                    useTempFile = true;
                    break;
            }
            if (useTempFile) {
                targetFile = Path.GetTempFileName();
                if (bytes != null) {
                    // BOM を除いてデコード
                    int start = encoding.GetPreamble().Length;
                    int count = bytes.Length - start;
                    string buffer = encoding.GetString(bytes, start, count);
                    WriteAllText(targetFile, buffer, Encoding.Unicode);
                } else {
                    WriteAllText(targetFile, string.Empty, Encoding.Unicode);
                }
            }
        }

        private void WriteAllText(string fileName, string buffer, Encoding encoding)
        {
            MakeSureDirectoryPathExists(fileName);
            File.WriteAllText(fileName, buffer, encoding);
        }

        private void MakeSureDirectoryPathExists(string targetFile)
        {
            var dir = Path.GetDirectoryName(targetFile);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
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
                WriteAllText(inifileName, buffer, encoding);
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
            MakeSureDirectoryPathExists(targetFile);
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
            MakeSureDirectoryPathExists(targetFile);
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
