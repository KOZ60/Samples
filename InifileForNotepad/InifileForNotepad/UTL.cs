namespace IniFileForNotepad
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class UTL
    {
        static Dictionary<CharacterSet, Encoding> _Encodings;
        public static Dictionary<CharacterSet, Encoding> Encodings {
            get {
                if (_Encodings == null) {
                    _Encodings = new Dictionary<CharacterSet, Encoding>();
                    _Encodings.Add(CharacterSet.SJIS, Encoding.GetEncoding("Shift_JIS"));
                    _Encodings.Add(CharacterSet.UTF8, new UTF8Encoding(true, false));
                    _Encodings.Add(CharacterSet.UTF8N, new UTF8Encoding(false, false));
                    _Encodings.Add(CharacterSet.UTF16, new UnicodeEncoding(false, true));
                    _Encodings.Add(CharacterSet.UTF16BE, new UnicodeEncoding(true, true));
                    _Encodings.Add(CharacterSet.UTF32, new UTF32Encoding(false, true));
                    _Encodings.Add(CharacterSet.UTF32BE, new UTF32Encoding(true, true));
                    _Encodings.Add(CharacterSet.Default, Encoding.Default);
                }
                return _Encodings;
            }
        }

        class ByteOrderMarkEntry
        {
            public ByteOrderMarkEntry(CharacterSet character, byte[] byteOrderMark)
            {
                Character = character;
                ByteOrderMark = byteOrderMark;
            }

            public CharacterSet Character { get; }
            public byte[] ByteOrderMark { get; }
        }

        static ByteOrderMarkEntry[] _ByteOrderMark;
        static ByteOrderMarkEntry[] ByteOrderMark {
            get {
                if (_ByteOrderMark == null) {
                    var lst = new List<ByteOrderMarkEntry>();
                    foreach (var kp in Encodings) {
                        var encoding = kp.Value;
                        var byteOrderMark = encoding.GetPreamble();
                        if (byteOrderMark.Length > 0) {
                            var entry = new ByteOrderMarkEntry(kp.Key, byteOrderMark);
                            lst.Add(entry);
                        }
                    }
                    // BOM の長さの降順に並び替える
                    // (UTF16 と UTF32 の先頭が等しいので BOM の長さの大きいものから比較)
                    _ByteOrderMark = lst.OrderByDescending(i => i.ByteOrderMark.Length).ToArray();
                }
                return _ByteOrderMark;
            }
        }

        public static CharacterSet DetectCharacterSet(byte[] bytes)
        {
            // 短い場合は判別不能
            if (bytes.Length < 2) {
                return CharacterSet.Default;
            }

            // BOM が一致するものを探す
            foreach (var entry in ByteOrderMark) {
                if (bytes.HasBOM(entry.ByteOrderMark)) {
                    return entry.Character;
                }
            }

            // BOM 無しで文字判定
            return DetectCharacterSetPureText(bytes);
        }

        private static bool HasBOM(this byte[] src, byte[] item)
        {
            if (src.Length < item.Length) return false;
            for (int i = 0; i < item.Length; i++) {
                if (src[i] != item[i]) return false;
            }
            return true;
        }

        private static CharacterSet DetectCharacterSetPureText(byte[] bytes)
        {
            // icu.dll が存在していれば使用する
            if(NativeMethods.DetectICU()) {
                var name = NativeMethods.DetectCharacterSetICU(bytes);
                if (name == "UTF-8") {
                    return CharacterSet.UTF8N;
                }
                return CharacterSet.SJIS;
            }           
            
            // DOBON.NET より拝借(euc の判定を除く)
            // https://dobon.net/vb/dotnet/string/detectcode.html#jcode

            int len = bytes.Length;
            byte b1, b2, b3;
            int sjis = 0;
            int utf8 = 0;

            // SJIS として妥当な文字のバイト数を調べる
            for (int i = 0; i < len - 1; i++) {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                    ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC))) {
                    sjis += 2;
                    i++;
                }
            }

            // UTF8 として妥当な文字のバイト数を調べる
            for (int i = 0; i < len - 1; i++) {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF)) {
                    utf8 += 2;
                    i++;
                } else if (i < len - 2) {
                    b3 = bytes[i + 2];
                    if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
                        (0x80 <= b3 && b3 <= 0xBF)) {
                        utf8 += 3;
                        i += 2;
                    }
                }
            }

            if (utf8 > sjis) {
                return CharacterSet.UTF8N;
            }

            return CharacterSet.SJIS;
        }
    }
}
