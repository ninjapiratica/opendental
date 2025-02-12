using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormJobTime : FormODBase
    {

        public FormJobTime()
        {
            InitializeComponent();
            InitializeLayoutManager(isLayoutMS: true);
            Lan.F(this);
        }

        private void FormJobTime_Load(object sender, EventArgs e)
        {
            gridJobs.Title = "Job Hours - " + Security.CurUser.UserName;
            FillGrid();
        }

        private void FillGrid()
        {
            List<Job> listJobsFiltered = JobManagerCore.ListJobsAll.Where(x => !x.PhaseCur.In(JobPhase.Complete, JobPhase.Documentation)).ToList();
            List<JobLog> listJobLogs = JobLogs.GetJobLogsForJobs(listJobsFiltered.Select(x => x.JobNum).ToList());
            listJobsFiltered = listJobsFiltered.Where(x => listJobLogs.Exists(
                y => y.DateTimeEntry >= DateTime.Today
                && y.UserNumChanged == Security.CurUser.UserNum)).ToList();//Give me all the joblogs for today.
                                                                           //Order the list in this order
                                                                           //1: Jobs you own (Engineer or Expert) with no time log for today (Red)
                                                                           //2: Jobs you own (Engineer or Expert) with no time log for today	(Green)
                                                                           //3: Jobs you looked at today, but do not own	(White)
            List<Job> listJobsSorted = listJobsFiltered
                    .OrderByDescending(x => x.UserNumEngineer == Security.CurUser.UserNum || x.UserNumExpert == Security.CurUser.UserNum)
                    .ThenByDescending(x => !x.ListJobTimeLogs.Exists(y => y.ReviewerNum == Security.CurUser.UserNum && y.DateTStamp.Day == DateTime.Today.Day))
                    .ToList();
            List<string> listAvailableIncrements = new List<string>();
            //Fill with range 10 to -10 in .5 increments
            double i = 10;
            while (i > -10.5)
            {
                listAvailableIncrements.Add(i.ToString());
                i = i - (.5);
            }
            gridJobs.BeginUpdate();
            gridJobs.Columns.Clear();
            //The order of these columns is very important.  See butOK_Click() for more details.
            gridJobs.Columns.Add(new GridColumn("Title", 75) { IsWidthDynamic = true });
            gridJobs.Columns.Add(new GridColumn("Estimated", 75) { TextAlign = HorizontalAlignment.Center });
            gridJobs.Columns.Add(new GridColumn("Actual", 75) { TextAlign = HorizontalAlignment.Center });
            gridJobs.Columns.Add(new GridColumn("Today", 75) { TextAlign = HorizontalAlignment.Center });
            gridJobs.Columns.Add(new GridColumn("To Add", 75) { ListDisplayStrings = listAvailableIncrements, TextAlign = HorizontalAlignment.Center });
            gridJobs.Columns.Add(new GridColumn("Note", 200, true));
            gridJobs.ListGridRows.Clear();
            foreach (Job job in listJobsSorted)
            {
                GridRow row = new GridRow() { Tag = job };
                if ((job.UserNumEngineer == Security.CurUser.UserNum || job.UserNumExpert == Security.CurUser.UserNum)
                    && !job.PhaseCur.In(JobPhase.Complete, JobPhase.Documentation))
                {
                    row.ColorBackG = job.ListJobTimeLogs.Exists(y => y.ReviewerNum == Security.CurUser.UserNum && y.DateTStamp.Day == DateTime.Today.Day) ? Color.LightGreen : Color.LightCoral;
                }
                row.Cells.Add(job.ToString());
                row.Cells.Add(job.HoursEstimate.ToString());
                row.Cells.Add(job.HoursActual.ToString());
                row.Cells.Add(job.ListJobTimeLogs.Where(x => x.DateTStamp >= DateTime.Today && x.ReviewerNum == Security.CurUser.UserNum && x.ReviewStatus == JobReviewStatus.TimeLog).Sum(x => x.Hours).ToString());
                row.Cells.Add("0");
                row.Cells.Add("");
                gridJobs.ListGridRows.Add(row);
            }
            gridJobs.EndUpdate();
        }

        private void gridJobs_CellDoubleClick(object sender, ODGridClickEventArgs e)
        {
            FormOpenDental.S_GoToJob(((Job)gridJobs.ListGridRows[e.Row].Tag).JobNum);
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            //Insert new time log entries for each row that has a non-zero additional hours
            foreach (GridRow row in gridJobs.ListGridRows)
            {
                if (row.Cells[4].Text.ToString() != "0")
                {
                    JobReview review = new JobReview();
                    review.Description = POut.String(row.Cells[4].Text.ToString());
                    review.ReviewerNum = Security.CurUser.UserNum;
                    review.TimeReview = TimeSpan.FromHours(PIn.Double(row.Cells[4].Text));
                    review.JobNum = ((Job)row.Tag).JobNum;
                    review.ReviewStatus = JobReviewStatus.TimeLog;
                    JobReviews.Insert(review);
                    Signalods.SetInvalid(InvalidType.Jobs, KeyType.Job, review.JobNum);
                }
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
