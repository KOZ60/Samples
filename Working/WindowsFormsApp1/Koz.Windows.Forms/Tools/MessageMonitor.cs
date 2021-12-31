using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Koz.Windows.Forms.Tools
{
    /// <summary>
    /// デバッグ用
    /// </summary>
    public static class MessageMonitor
    {
        // 【使い方】
        //protected override void WndProc(ref Message m) {
        //    try {
        //        MessageMonitor.Enter<EditMessage>(ref m);
        //        base.WndProc(ref m);
        //    } finally {
        //        MessageMonitor.Exit<EditMessage>(ref m);
        //    }
        //}

        private const int
            WM_USER = 0x0400,
            WM_REFLECT = WM_USER + 0x1C00;


        public static string MsgToString<TEnum>(int msg) where TEnum : struct {
            if ((msg & WM_REFLECT) == WM_REFLECT) {
                return "WM_REFLECT + " + MsgToString<TEnum>(msg & ~WM_REFLECT);
            }
            if (TryParse<WindowMessage>(msg, out WindowMessage wm)) {
                return wm.ToString();
            }
            if (TryParse<TEnum>(msg, out TEnum contolType)) {
                return contolType.ToString();
            }
            return string.Empty;
        }

        public static bool TryParse<TEnum>(int value, out TEnum result) where TEnum : struct {
            if (Enum.IsDefined(typeof(TEnum), value)) {
                result = (TEnum)Enum.ToObject(typeof(TEnum), value);
                return true;
            }
            result = default(TEnum);
            return false;
        }

        private static readonly Dictionary<IntPtr, int> countDictionary = new Dictionary<IntPtr, int>();

        private static int GetCount(IntPtr hwnd) {
            lock (countDictionary) {
                countDictionary.TryGetValue(hwnd, out int count);
                return count;
            }
        }

        private static void SaveCount(IntPtr hwnd, int count) {
            lock (countDictionary) {
                if (count == 0) {
                    if (countDictionary.ContainsKey(hwnd)) {
                        countDictionary.Remove(hwnd);
                    }
                } else {
                    countDictionary[hwnd] = count;
                }
            }
        }

        public static void Enter<TEnum>(ref Message m) where TEnum : struct {
            int count = GetCount(m.HWnd);
            var space = new string(' ', count);
            Debug.Print("{0} {1}+ msg=0x{2:X4} {3} hwnd=0x{4:X4} wparam=0x{5:X8} lparam=0x{6:X8}",
                            DateTime.Now, space, (uint)m.Msg, MsgToString<TEnum>(m.Msg), (ulong)m.HWnd, (ulong)m.WParam, (ulong)m.LParam);
            count++;
            SaveCount(m.HWnd, count);
        }

        public static void Exit<TEnum>(ref Message m) where TEnum : struct {
            int count = GetCount(m.HWnd);
            count--;
            var space = new string(' ', count);
            Debug.Print("{0} {1}- msg=0x{2:X4} {3} hwnd=0x{4:X4} result={5}",
                            DateTime.Now, space, (uint)m.Msg, MsgToString<TEnum>(m.Msg), (ulong)m.HWnd, (ulong)m.Result);
            SaveCount(m.HWnd, count);
        }
    }
}
