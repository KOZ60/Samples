namespace IniFileForNotepad
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class NativeMethods
    {
        const string Kernel32 = "kernel32.dll";
        const string ICU = "icu.dll";

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileInt(
            string lpAppName,
            string lpKeyName,
            int nDefault,
            string lpFileName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileSection(
            string lpAppName,
            StringBuilder lpszReturnBuffer,
            int nSize,
            string lpFileName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpDefault,
            StringBuilder lpReturnedString,
            int nSize,
            string lpFileName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WritePrivateProfileSection(
            string lpAppName,
            string lpString,
            string lpFileName);

        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);

        [DllImport(Kernel32, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(Kernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        public static bool DetectDll(string dllName)
        {
            IntPtr hModule = LoadLibrary(dllName);
            if (hModule != IntPtr.Zero) {
                FreeLibrary(hModule);
                return true;
            }
            return false;
        }

        public static bool DetectICU()
        {
            return DetectDll(ICU);
        }

        // icu.dll

        public enum UErrorCode
        {
            U_USING_FALLBACK_WARNING = -128, U_ERROR_WARNING_START = -128, U_USING_DEFAULT_WARNING = -127, U_SAFECLONE_ALLOCATED_WARNING = -126,
            U_STATE_OLD_WARNING = -125, U_STRING_NOT_TERMINATED_WARNING = -124, U_SORT_KEY_TOO_SHORT_WARNING = -123, U_AMBIGUOUS_ALIAS_WARNING = -122,
            U_DIFFERENT_UCA_VERSION = -121, U_PLUGIN_CHANGED_LEVEL_WARNING = -120, U_ERROR_WARNING_LIMIT, U_ZERO_ERROR = 0,
            U_ILLEGAL_ARGUMENT_ERROR = 1, U_MISSING_RESOURCE_ERROR = 2, U_INVALID_FORMAT_ERROR = 3, U_FILE_ACCESS_ERROR = 4,
            U_INTERNAL_PROGRAM_ERROR = 5, U_MESSAGE_PARSE_ERROR = 6, U_MEMORY_ALLOCATION_ERROR = 7, U_INDEX_OUTOFBOUNDS_ERROR = 8,
            U_PARSE_ERROR = 9, U_INVALID_CHAR_FOUND = 10, U_TRUNCATED_CHAR_FOUND = 11, U_ILLEGAL_CHAR_FOUND = 12,
            U_INVALID_TABLE_FORMAT = 13, U_INVALID_TABLE_FILE = 14, U_BUFFER_OVERFLOW_ERROR = 15, U_UNSUPPORTED_ERROR = 16,
            U_RESOURCE_TYPE_MISMATCH = 17, U_ILLEGAL_ESCAPE_SEQUENCE = 18, U_UNSUPPORTED_ESCAPE_SEQUENCE = 19, U_NO_SPACE_AVAILABLE = 20,
            U_CE_NOT_FOUND_ERROR = 21, U_PRIMARY_TOO_LONG_ERROR = 22, U_STATE_TOO_OLD_ERROR = 23, U_TOO_MANY_ALIASES_ERROR = 24,
            U_ENUM_OUT_OF_SYNC_ERROR = 25, U_INVARIANT_CONVERSION_ERROR = 26, U_INVALID_STATE_ERROR = 27, U_COLLATOR_VERSION_MISMATCH = 28,
            U_USELESS_COLLATOR_ERROR = 29, U_NO_WRITE_PERMISSION = 30, U_INPUT_TOO_LONG_ERROR = 31, U_STANDARD_ERROR_LIMIT = 32,
            U_BAD_VARIABLE_DEFINITION = 0x10000, U_PARSE_ERROR_START = 0x10000, U_MALFORMED_RULE, U_MALFORMED_SET,
            U_MALFORMED_SYMBOL_REFERENCE, U_MALFORMED_UNICODE_ESCAPE, U_MALFORMED_VARIABLE_DEFINITION, U_MALFORMED_VARIABLE_REFERENCE,
            U_MISMATCHED_SEGMENT_DELIMITERS, U_MISPLACED_ANCHOR_START, U_MISPLACED_CURSOR_OFFSET, U_MISPLACED_QUANTIFIER,
            U_MISSING_OPERATOR, U_MISSING_SEGMENT_CLOSE, U_MULTIPLE_ANTE_CONTEXTS, U_MULTIPLE_CURSORS,
            U_MULTIPLE_POST_CONTEXTS, U_TRAILING_BACKSLASH, U_UNDEFINED_SEGMENT_REFERENCE, U_UNDEFINED_VARIABLE,
            U_UNQUOTED_SPECIAL, U_UNTERMINATED_QUOTE, U_RULE_MASK_ERROR, U_MISPLACED_COMPOUND_FILTER,
            U_MULTIPLE_COMPOUND_FILTERS, U_INVALID_RBT_SYNTAX, U_INVALID_PROPERTY_PATTERN, U_MALFORMED_PRAGMA,
            U_UNCLOSED_SEGMENT, U_ILLEGAL_CHAR_IN_SEGMENT, U_VARIABLE_RANGE_EXHAUSTED, U_VARIABLE_RANGE_OVERLAP,
            U_ILLEGAL_CHARACTER, U_INTERNAL_TRANSLITERATOR_ERROR, U_INVALID_ID, U_INVALID_FUNCTION,
            U_PARSE_ERROR_LIMIT, U_UNEXPECTED_TOKEN = 0x10100, U_FMT_PARSE_ERROR_START = 0x10100, U_MULTIPLE_DECIMAL_SEPARATORS,
            U_MULTIPLE_DECIMAL_SEPERATORS = U_MULTIPLE_DECIMAL_SEPARATORS, U_MULTIPLE_EXPONENTIAL_SYMBOLS, U_MALFORMED_EXPONENTIAL_PATTERN, U_MULTIPLE_PERCENT_SYMBOLS,
            U_MULTIPLE_PERMILL_SYMBOLS, U_MULTIPLE_PAD_SPECIFIERS, U_PATTERN_SYNTAX_ERROR, U_ILLEGAL_PAD_POSITION,
            U_UNMATCHED_BRACES, U_UNSUPPORTED_PROPERTY, U_UNSUPPORTED_ATTRIBUTE, U_ARGUMENT_TYPE_MISMATCH,
            U_DUPLICATE_KEYWORD, U_UNDEFINED_KEYWORD, U_DEFAULT_KEYWORD_MISSING, U_DECIMAL_NUMBER_SYNTAX_ERROR,
            U_FORMAT_INEXACT_ERROR, U_NUMBER_ARG_OUTOFBOUNDS_ERROR, U_NUMBER_SKELETON_SYNTAX_ERROR, U_FMT_PARSE_ERROR_LIMIT = 0x10114,
            U_BRK_INTERNAL_ERROR = 0x10200, U_BRK_ERROR_START = 0x10200, U_BRK_HEX_DIGITS_EXPECTED, U_BRK_SEMICOLON_EXPECTED,
            U_BRK_RULE_SYNTAX, U_BRK_UNCLOSED_SET, U_BRK_ASSIGN_ERROR, U_BRK_VARIABLE_REDFINITION,
            U_BRK_MISMATCHED_PAREN, U_BRK_NEW_LINE_IN_QUOTED_STRING, U_BRK_UNDEFINED_VARIABLE, U_BRK_INIT_ERROR,
            U_BRK_RULE_EMPTY_SET, U_BRK_UNRECOGNIZED_OPTION, U_BRK_MALFORMED_RULE_TAG, U_BRK_ERROR_LIMIT,
            U_REGEX_INTERNAL_ERROR = 0x10300, U_REGEX_ERROR_START = 0x10300, U_REGEX_RULE_SYNTAX, U_REGEX_INVALID_STATE,
            U_REGEX_BAD_ESCAPE_SEQUENCE, U_REGEX_PROPERTY_SYNTAX, U_REGEX_UNIMPLEMENTED, U_REGEX_MISMATCHED_PAREN,
            U_REGEX_NUMBER_TOO_BIG, U_REGEX_BAD_INTERVAL, U_REGEX_MAX_LT_MIN, U_REGEX_INVALID_BACK_REF,
            U_REGEX_INVALID_FLAG, U_REGEX_LOOK_BEHIND_LIMIT, U_REGEX_SET_CONTAINS_STRING, U_REGEX_OCTAL_TOO_BIG,
            U_REGEX_MISSING_CLOSE_BRACKET = U_REGEX_SET_CONTAINS_STRING + 2, U_REGEX_INVALID_RANGE, U_REGEX_STACK_OVERFLOW, U_REGEX_TIME_OUT,
            U_REGEX_STOPPED_BY_CALLER, U_REGEX_PATTERN_TOO_BIG, U_REGEX_INVALID_CAPTURE_GROUP_NAME, U_REGEX_ERROR_LIMIT = U_REGEX_STOPPED_BY_CALLER + 3,
            U_IDNA_PROHIBITED_ERROR = 0x10400, U_IDNA_ERROR_START = 0x10400, U_IDNA_UNASSIGNED_ERROR, U_IDNA_CHECK_BIDI_ERROR,
            U_IDNA_STD3_ASCII_RULES_ERROR, U_IDNA_ACE_PREFIX_ERROR, U_IDNA_VERIFICATION_ERROR, U_IDNA_LABEL_TOO_LONG_ERROR,
            U_IDNA_ZERO_LENGTH_LABEL_ERROR, U_IDNA_DOMAIN_NAME_TOO_LONG_ERROR, U_IDNA_ERROR_LIMIT, U_STRINGPREP_PROHIBITED_ERROR = U_IDNA_PROHIBITED_ERROR,
            U_STRINGPREP_UNASSIGNED_ERROR = U_IDNA_UNASSIGNED_ERROR, U_STRINGPREP_CHECK_BIDI_ERROR = U_IDNA_CHECK_BIDI_ERROR, U_PLUGIN_ERROR_START = 0x10500, U_PLUGIN_TOO_HIGH = 0x10500,
            U_PLUGIN_DIDNT_SET_LEVEL, U_PLUGIN_ERROR_LIMIT, U_ERROR_LIMIT = U_PLUGIN_ERROR_LIMIT
        }

        [DllImport(ICU, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ucsdet_open(ref UErrorCode status);

        [DllImport(ICU, CallingConvention = CallingConvention.Cdecl)]
        private static extern void ucsdet_close(IntPtr ucsd);

        [DllImport(ICU, CallingConvention = CallingConvention.Cdecl)]
        private static extern void ucsdet_setText(IntPtr ucsd, ref byte textIn, int len, ref UErrorCode status);

        [DllImport(ICU, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ucsdet_detect(IntPtr ucsd, ref UErrorCode status);

        [DllImport(ICU, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ucsdet_getName(IntPtr ucsd, ref UErrorCode status);

        public static string DetectCharacterSetICU(byte[] buffer)
        {
            UErrorCode error = UErrorCode.U_ZERO_ERROR;

            IntPtr detector = ucsdet_open(ref error);
            if (error != UErrorCode.U_ZERO_ERROR) {
                throw new Win32Exception(error.ToString());
            }

            try {
                ucsdet_setText(detector, ref buffer[0], buffer.Length, ref error);
                if (error != UErrorCode.U_ZERO_ERROR) {
                    throw new Win32Exception(error.ToString());
                }

                IntPtr match = ucsdet_detect(detector, ref error);
                if (error != UErrorCode.U_ZERO_ERROR) {
                    throw new Win32Exception(error.ToString());
                }

                IntPtr match_encoding = ucsdet_getName(match, ref error);
                if (error != UErrorCode.U_ZERO_ERROR) {
                    throw new Win32Exception(error.ToString());
                }

                return Marshal.PtrToStringAnsi(match_encoding);

            } finally {
                ucsdet_close(detector);
            }
        }
    }
}
