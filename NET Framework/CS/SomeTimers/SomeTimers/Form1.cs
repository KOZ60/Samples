using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace SomeTimers
{
    public partial class Form1 : Form
    {
        private Stopwatch sw = new Stopwatch();
        List<TimeSpan>[] lists;


        public Form1() {
            InitializeComponent();
        }

        private void SetButton(bool value) {
            btnStart.Enabled = value;
            btnStop.Enabled = !value;
        }

        private void btnStart_Click(object sender, EventArgs e) {
            SetButton(false);
            lists = new List<TimeSpan>[] { new List<TimeSpan>(), new List<TimeSpan>() };
            sw.Start();
            waitableTimer1.IntervalMillisecond = 100;
            performanceTimer1.IntervalMillisecond = 100;
            performanceTimer1.Start();
            waitableTimer1.Start();
        }

        private void btnStop_Click(object sender, EventArgs e) {
            SetButton(true);
            performanceTimer1.Stop();
            waitableTimer1.Stop();
            sw.Stop();
            SetList(listBox1, lists[0]);
            SetList(listBox2, lists[1]);
        }

        private static void SetList(ListBox list, List<TimeSpan> velues) {
            list.Items.Clear();
            for (int i = 1; i < velues.Count; i++) {
                var memo = velues[i] - velues[i - 1];
                list.Items.Add(memo);
            }
        }


        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            SetButton(true);
        }

        protected override void OnFormClosed(FormClosedEventArgs e) {
            sw.Stop();
            base.OnFormClosed(e);
        }

        private void performanceTimer1_Elapsed(object sender, EventArgs e) {
            lists[0].Add(sw.Elapsed);
        }

        private void waitableTimer1_Elapsed(object sender, EventArgs e) {
            lists[1].Add(sw.Elapsed);
        }
    }
}
