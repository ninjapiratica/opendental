using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormJobTimeLog : FormODBase
    {
        private Job _jobCur;

        public FormJobTimeLog(Job jobCur)
        {
            _jobCur = jobCur;
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormJobTimeLog_Load(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void FillGrid()
        {
            List<JobReview> listTime = new List<JobReview>();
            listTime.AddRange(_jobCur.ListJobTimeLogs);
            listTime.AddRange(_jobCur.ListJobReviews);
            listTime = listTime.OrderByDescending(x => x.DateTStamp).ToList();
            List<Userod> listUsers = Userods.GetAll();
            gridJobs.Columns.Add(new GridColumn("Date", 75) { TextAlign = HorizontalAlignment.Center });
            gridJobs.Columns.Add(new GridColumn("User", 75));
            gridJobs.Columns.Add(new GridColumn("Type", 125) { TextAlign = HorizontalAlignment.Center });
            gridJobs.Columns.Add(new GridColumn("Time", 75) { TextAlign = HorizontalAlignment.Center });
            gridJobs.ListGridRows.Clear();
            foreach (JobReview review in listTime)
            {
                GridRow row = new GridRow() { Tag = review };
                row.Cells.Add(review.DateTStamp.ToShortDateString());
                row.Cells.Add(listUsers.FirstOrDefault(x => x.UserNum == review.ReviewerNum).UserName);
                row.Cells.Add(review.ReviewStatus.ToString());
                row.Cells.Add(Math.Round(review.Hours, 2).ToString());
                gridJobs.ListGridRows.Add(row);
            }
            gridJobs.EndUpdate();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
