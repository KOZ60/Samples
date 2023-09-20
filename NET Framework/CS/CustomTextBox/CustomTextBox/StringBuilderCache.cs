using System;
using System.Text;

namespace CustomTextBox
{
    public class StringBuilderCache
    {
        private const int MAX_BUILDER_SIZE = 360;
        private const int DefaultCapacity = 16;

        [ThreadStatic]
        private static StringBuilder CachedInstance;

        public static StringBuilder Acquire(int capacity = DefaultCapacity) {
            if (capacity <= MAX_BUILDER_SIZE) {
                StringBuilder sb = CachedInstance;
                if (sb != null) {
                    if (capacity <= sb.Capacity) {
                        CachedInstance = null;
                        sb.Clear();
                        return sb;
                    }
                }
            }
            return new StringBuilder(capacity);
        }

        public static void Release(StringBuilder sb) {
            if (sb.Capacity <= MAX_BUILDER_SIZE) {
                CachedInstance = sb;
            }
        }

        public static string GetStringAndRelease(StringBuilder sb) {
            string result = sb.ToString();
            Release(sb);
            return result;
        }
    }
}
