using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SomeTimers
{
    public partial class Form1 : Form
    {
        private readonly Stopwatch sw = new Stopwatch();
        private readonly System.Timers.Timer timersTimer
                                = new System.Timers.Timer() { Interval = 100 };
        private readonly WaitableTimer waitableTimer
                                = new WaitableTimer() { Interval = 100 };
        private readonly PerformanceTimer performanceTimer
                                = new PerformanceTimer() { Interval = 100 };

        List<TimeSpan>[] lists;

        public Form1() {
            InitializeComponent();
        }

        private void SetButton(bool value) {
            btnStart.Enabled = value;
            btnStop.Enabled = !value;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            timersTimer.Elapsed += TimersTimer_Elapsed;
            waitableTimer.Elapsed += WaitableTimer_Elapsed;
            performanceTimer.Elapsed += PerformanceTimer_Elapsed;
            SetButton(true);
        }

        protected override void OnFormClosed(FormClosedEventArgs e) {
            sw.Stop();
            timersTimer.Dispose();
            waitableTimer.Dispose();
            base.OnFormClosed(e);
        }

        private void TimersTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lists[0].Add(sw.Elapsed);
        }

        private void WaitableTimer_Elapsed(object sender, EventArgs e) {
            lists[1].Add(sw.Elapsed);
        }

        private void PerformanceTimer_Elapsed(object sender, EventArgs e) {
            lists[2].Add(sw.Elapsed);
        }

        private async void BtnStart_Click(object sender, EventArgs e) {
            SetButton(false);
            lists = new List<TimeSpan>[] { new List<TimeSpan>(6000),
                                           new List<TimeSpan>(6000),
                                           new List<TimeSpan>(6000) };
            if (checkBox1.Checked) timersTimer.Start();
            if (checkBox2.Checked) waitableTimer.Start();
            if (checkBox3.Checked) performanceTimer.Start();
            sw.Start();
            await Task.Delay(5000);
            btnStop.PerformClick();
        }

        private void BtnStop_Click(object sender, EventArgs e) {
            if (checkBox1.Checked) timersTimer.Stop();
            if (checkBox2.Checked) waitableTimer.Stop();
            if (checkBox3.Checked) performanceTimer.Stop();
            SetButton(true);
            sw.Stop();
            if (checkBox1.Checked) SetList(listBox1, label1, lists[0]);
            if (checkBox2.Checked) SetList(listBox2, label2, lists[1]);
            if (checkBox3.Checked) SetList(listBox3, label3, lists[2]);
        }

        private static void SetList(ListBox list, Label label, List<TimeSpan> values) {
            list.Items.Clear();
            double totalValue = 0;
            double maxValue = double.MinValue;
            double minValue = double.MaxValue;
            for (int i = 1; i < values.Count; i++) {
                var value = (values[i] - values[i - 1]).TotalMilliseconds;
                list.Items.Add($"{value:000.0000} ms");
                totalValue += value;
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }
            double avgValue = totalValue / (values.Count - 1);
            label.Text = string.Format("MAX:{0:000.0000}", maxValue) + "\r\n"
                       + string.Format("MIN:{0:000.0000}", minValue) + "\r\n"
                       + string.Format("AVG:{0:000.0000}", avgValue);
        }
    }
}
