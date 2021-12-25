using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Koz.Windows.Forms
{
    public static class UTL
    {
        // フォントサイズの平均値を取得
        public static Size GetFontSizeAverage(Font font) {
            HandleRef hdc = new HandleRef(null, NativeMethods.GetDC(new HandleRef(null, IntPtr.Zero)));
            HandleRef hFont = new HandleRef(null, font.ToHfont());
            HandleRef oldFont = new HandleRef(null, NativeMethods.SelectObject(hdc, hFont));
            try {
                return GetFontSizeAverageFromHdc(hdc);
            } finally {
                NativeMethods.SelectObject(hdc, oldFont);
                NativeMethods.DeleteObject(hFont);
                NativeMethods.DeleteDC(hFont);
            }
        }

        // フォントが選択済みのデバイスコンテキストハンドルよりフォントサイズの平均値を取得
        public static Size GetFontSizeAverageFromHdc(HandleRef hdc) {
            NativeMethods.GetTextMetricsW(hdc, out NativeMethods.TEXTMETRICW tmNative);
            return new Size(tmNative.tmAveCharWidth, tmNative.tmHeight);
        }

    }
}
