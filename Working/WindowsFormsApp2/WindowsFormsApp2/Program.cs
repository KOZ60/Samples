using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {

            TEST result;
            TryParse<TEST>(1, out result);
            TryParse<TEST>(2, out result);
            TryParse<TEST>(3, out result);
            TryParse<TEST>(4, out result);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private enum TEST
        {
            ONE = 1,
            TWO,
            THREE
        }

        public static bool TryParse<T>(int value, out T result) where T : struct {
            if (Enum.IsDefined(typeof(T), value)) {
                result = (T)Enum.ToObject(typeof(T), value);
                return true;
            }
            result = default(T);
            return false;
        }


    }
}
