using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;

namespace OpenDental
{
    public partial class FormUpdateHistory : FormODBase
    {

        public FormUpdateHistory()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormUpdateHistory_Load(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void FillGrid()
        {
            gridMain.BeginUpdate();
            gridMain.Columns.Clear();
            GridColumn col = new GridColumn(Lan.g(this, "Version"), 117);
            gridMain.Columns.Add(col);
            col = new GridColumn(Lan.g(this, "Date"), 117);
            gridMain.Columns.Add(col);
            gridMain.ListGridRows.Clear();
            GridRow row = null;
            List<UpdateHistory> listUpdateHistories = UpdateHistories.GetAll();
            foreach (UpdateHistory updateHistory in listUpdateHistories)
            {
                row = new GridRow();
                row.Cells.Add(updateHistory.ProgramVersion);
                row.Cells.Add(updateHistory.DateTimeUpdated.ToString());
                gridMain.ListGridRows.Add(row);
            }
            gridMain.EndUpdate();
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}