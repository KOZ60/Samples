namespace RandomFileSample
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;

    public class RandomFile : SafeHandleZeroOrMinusOneIsInvalid
    {
        private int recordSize;

        public RandomFile(string fileName, int nSize)
            : this(fileName, nSize, FileAccess.ReadWrite) { }

        public RandomFile(string fileName, int nSize, FileAccess fileAccess)
            : this(fileName, nSize, fileAccess, FileShare.ReadWrite) { }

        public RandomFile(string fileName, int nSize, FileAccess fileAccess, FileShare fileShare)
            : this(fileName, nSize, fileAccess, fileShare, FileMode.OpenOrCreate) { }

        public RandomFile(string fileName, int nSize, FileAccess fileAccess,
                            FileShare fileShare, FileMode fileMode)
            : this(fileName, nSize, fileAccess, fileShare, fileMode, (FileAttributes)0) { }

        public RandomFile(string fileName, int nSize, FileAccess fileAccess, 
                            FileShare fileShare, FileMode fileMode, FileAttributes fileAttributes)
            : this(fileName, nSize, fileAccess, fileShare, fileMode, fileAttributes, false) { }

        public RandomFile(
                string fileName,
                int nSize,
                FileAccess fileAccess,
                FileShare fileShare,
                FileMode fileMode,
                FileAttributes fileAttributes,
                bool largeFile)
            : base(true)
        {
            // 【注意】
            // FILE_FLAG_RANDOM_ACCESS を指定すると大きなファイルを扱うとパフォーマンスが低下する場合がある
            // 「Performance degrades when you access large files with FILE_FLAG_RANDOM_ACCESS」
            // https://docs.microsoft.com/en-us/troubleshoot/windows-server/application-management/operating-system-performance-degrades
            if (!largeFile) {
                fileAttributes |= (FileAttributes)NativeMethods.FILE_FLAG_RANDOM_ACCESS;
            }

            handle = NativeMethods.CreateFile(
                                        fileName,
                                        fileAccess,
                                        fileShare,
                                        IntPtr.Zero,
                                        fileMode,
                                        fileAttributes,
                                        IntPtr.Zero
                                        );
            if (handle == NativeMethods.INVALID_HANDLE_VALUE) {
                throw new Win32Exception();
            }
            recordSize = nSize;
        }

        protected override bool ReleaseHandle()
        {
            bool result = NativeMethods.CloseHandle(handle);
            handle = NativeMethods.INVALID_HANDLE_VALUE;
            return result;
        }

        private long Pointer {
            set {
                bool result = NativeMethods.SetFilePointerEx(
                                        this, 
                                        value, 
                                        IntPtr.Zero, 
                                        NativeMethods.FILE_BEGIN);
                if (!result) {
                    throw new Win32Exception();
                }
            }
        }

        public long Length {
            get {
                long length;
                bool result = NativeMethods.GetFileSizeEx(this, out length);
                if (!result) {
                    throw new Win32Exception();
                }
                return length;
            }
        }

        private void ParameterCheck(int recordNo, Type recordType)
        {
            if (recordNo <= 0) {
                throw new ArgumentOutOfRangeException("レコード番号は 1 から始まります。");
            }

            if (Marshal.SizeOf(recordType) > recordSize) {
                throw new ArgumentOutOfRangeException("構造体の大きさがレコード長を超えています。");
            }
        }

        public void Get<T>(int recordNo, ref T record) where T : struct
        {
            ParameterCheck(recordNo, typeof(T));

            IntPtr bufferPtr = Read(recordNo, 1);
            try {
                record = (T)Marshal.PtrToStructure(bufferPtr, typeof(T));
            } finally {
                Marshal.FreeCoTaskMem(bufferPtr);
            }
        }

        public void Get<T>(int recordNo, T[] records) where T : struct
        {
            ParameterCheck(recordNo, typeof(T));

            IntPtr bufferPtr = Read(recordNo, records.Length);
            IntPtr pointer = bufferPtr;
            try {
                for (int i = 0; i < records.Length; i++) {
                    records[i] = (T)Marshal.PtrToStructure(pointer, typeof(T));
                    pointer += recordSize;
                }
            } finally {
                Marshal.FreeCoTaskMem(bufferPtr);
            }
        }

        public void Put<T>(int recordNo, ref T record) where T : struct
        {
            ParameterCheck(recordNo, typeof(T));

            IntPtr bufferPtr = Marshal.AllocCoTaskMem(recordSize);
            try {
                Marshal.StructureToPtr(record, bufferPtr, true);
                Write(recordNo, bufferPtr, recordSize);
            } finally {
                Marshal.FreeCoTaskMem(bufferPtr);
            }
        }

        public void Put<T>(int recordNo, T[] records) where T : struct
        {
            ParameterCheck(recordNo, typeof(T));

            int nSize = recordSize * records.Length;
            IntPtr bufferPtr = Marshal.AllocCoTaskMem(nSize);
            IntPtr pointer = bufferPtr;
            try {
                for (int i = 0; i < records.Length; i++) {
                    Marshal.StructureToPtr(records[i], pointer, true);
                    pointer += recordSize;
                }
                Write(recordNo, bufferPtr, nSize);
            } finally {
                Marshal.FreeCoTaskMem(bufferPtr);
            }
        }

        private IntPtr Read(int recordNo, int recordCount)
        {
            Pointer = (recordNo - 1) * recordSize;
            int nSize = recordSize * recordCount;
            IntPtr bufferPtr = Marshal.AllocCoTaskMem(recordSize * recordCount);
            int readed;
            bool result = NativeMethods.ReadFile(this, bufferPtr, nSize, out readed, IntPtr.Zero);
            if (!result) {
                throw new Win32Exception();
            }
            return bufferPtr;
        }

        private void Write(int recordNo, IntPtr bufferPtr, int nSize)
        {
            byte[] bytes = new byte[nSize];
            Marshal.Copy(bufferPtr, bytes, 0, nSize);
            Pointer = (recordNo - 1) * recordSize;
            int wrote;
            bool result = NativeMethods.WriteFile(this, bufferPtr, nSize, out wrote, IntPtr.Zero);
            if (!result) {
                throw new Win32Exception();
            }
        }

        internal static class NativeMethods
        {
            private const string Kernel32 = "kernel32.dll";

            public static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

            public const int FILE_FLAG_RANDOM_ACCESS = 0x10000000;

            [DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr CreateFile(
                string lpFileName,
                FileAccess dwDesiredAccess,
                FileShare dwShareMode,
                IntPtr securityAttrs,
                FileMode dwCreationDisposition,
                FileAttributes dwFlagsAndAttributes,
                IntPtr hTemplateFile);

            [DllImport(Kernel32, SetLastError = true)]
            public static extern bool WriteFile(SafeHandle handle, IntPtr bytes, int numBytesToWrite, out int numBytesWritten, IntPtr mustBeZero);

            [DllImport(Kernel32, SetLastError = true)]
            public static extern bool ReadFile(SafeHandle handle, IntPtr bytes, int numBytesToRead, out int numBytesRead, IntPtr mustBeZero);

            [DllImport(Kernel32, SetLastError = true)]
            public static extern bool CloseHandle(IntPtr handle);

            [DllImport(Kernel32, SetLastError = true)]
            public static extern bool GetFileSizeEx(SafeHandle hFile, out long lpFileSize);

            public const int FILE_BEGIN = 0;
            public const int FILE_CURRENT = 1;
            public const int FILE_END = 2;

            [DllImport(Kernel32, SetLastError = true)]
            public static extern bool SetFilePointerEx(SafeHandle handle, long liDistanceToMove, IntPtr lpNewFilePointer, int dwMoveMethod);

        }
    }
}
