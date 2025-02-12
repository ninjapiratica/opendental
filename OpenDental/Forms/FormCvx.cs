using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormCvxs : FormODBase
    {
        public bool IsSelectionMode;
        public Cvx CvxSelected;
        private List<Cvx> _listCvxs;

        public FormCvxs()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormCvxs_Load(object sender, EventArgs e)
        {
            if (IsSelectionMode)
            {
                butClose.Text = Lan.g(this, "Cancel");
            }
            else
            {
                butOK.Visible = false;
            }
            ActiveControl = textCode;
        }

        private void butSearch_Click(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void FillGrid()
        {
            gridMain.BeginUpdate();
            gridMain.Columns.Clear();
            GridColumn col;
            col = new GridColumn("CVX Code", 100);
            gridMain.Columns.Add(col);
            col = new GridColumn("Description", 500);
            gridMain.Columns.Add(col);
            gridMain.ListGridRows.Clear();
            GridRow row;
            _listCvxs = Cvxs.GetBySearchText(textCode.Text);
            for (int i = 0; i < _listCvxs.Count; i++)
            {
                row = new GridRow();
                row.Cells.Add(_listCvxs[i].CvxCode);
                row.Cells.Add(_listCvxs[i].Description);
                row.Tag = _listCvxs[i];
                gridMain.ListGridRows.Add(row);
            }
            gridMain.EndUpdate();
        }

        private void gridMain_CellDoubleClick(object sender, ODGridClickEventArgs e)
        {
            if (IsSelectionMode)
            {
                CvxSelected = (Cvx)gridMain.ListGridRows[e.Row].Tag;
                DialogResult = DialogResult.OK;
                return;
            }
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            //not even visible unless IsSelectionMode
            if (gridMain.GetSelectedIndex() == -1)
            {
                MsgBox.Show(this, "Please select an item first.");
                return;
            }
            CvxSelected = (Cvx)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}