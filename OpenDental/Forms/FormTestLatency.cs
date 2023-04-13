using OpenDentBusiness;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormTestLatency : FormODBase
    {
        public FormTestLatency()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormTestLatency_Load(object sender, EventArgs e)
        {

        }

        private void butLatency_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            Cursor = Cursors.WaitCursor;
            stopwatch.Start();
            MiscData.GetMySqlVersion();//a nice short query and small dataset.
            stopwatch.Stop();
            textLatency.Text = stopwatch.ElapsedMilliseconds.ToString();
            Cursor = Cursors.Default;
        }

        private void butSpeed_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            Cursor = Cursors.WaitCursor;
            stopwatch.Start();
            MiscData.GetMySqlVersion();//a nice short query and small dataset.
            stopwatch.Stop();
            long latency = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            Prefs.RefreshCache();
            stopwatch.Stop();
            long speed = stopwatch.ElapsedMilliseconds - latency;
            textSpeed.Text = speed.ToString();
            Cursor = Cursors.Default;
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            Close();
        }


    }
}