using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormDispensary : FormODBase
    {
        private List<SchoolClass> _listSchoolClasses;

        public FormDispensary()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormDispensary_Load(object sender, EventArgs e)
        {
            comboClass.Items.Add(Lan.g(this, "All"));
            comboClass.SelectedIndex = 0;
            _listSchoolClasses = SchoolClasses.GetDeepCopy();
            for (int i = 0; i < _listSchoolClasses.Count; i++)
            {
                comboClass.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
            }
            FillStudents();
        }

        private void FillStudents()
        {
            long selectedProvNum = 0;
            long schoolClass = 0;
            if (comboClass.SelectedIndex > 0)
            {
                schoolClass = _listSchoolClasses[comboClass.SelectedIndex - 1].SchoolClassNum;
            }
            long.TryParse(textProvNum.Text, out selectedProvNum);
            DataTable table = Providers.RefreshForDentalSchool(schoolClass, textLName.Text, textFName.Text, textProvNum.Text, selectInstructors: false, selectAll: false);
            gridStudents.BeginUpdate();
            gridStudents.Columns.Clear();
            GridColumn col;
            col = new GridColumn(Lan.g("TableProviderSetup", "ProvNum"), 60);
            gridStudents.Columns.Add(col);
            col = new GridColumn(Lan.g("TableProviderSetup", "Last Name"), 90);
            gridStudents.Columns.Add(col);
            col = new GridColumn(Lan.g("TableProviderSetup", "First Name"), 90);
            gridStudents.Columns.Add(col);
            col = new GridColumn(Lan.g("TableProviderSetup", "Class"), 100);
            gridStudents.Columns.Add(col);
            gridStudents.ListGridRows.Clear();
            GridRow row;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = new GridRow();
                row.Cells.Add(table.Rows[i]["ProvNum"].ToString());
                row.Cells.Add(table.Rows[i]["LName"].ToString());
                row.Cells.Add(table.Rows[i]["FName"].ToString());
                if (table.Rows[i]["GradYear"].ToString() != "")
                {
                    row.Cells.Add(table.Rows[i]["GradYear"].ToString() + "-" + table.Rows[i]["Descript"].ToString());
                }
                else
                {
                    row.Cells.Add("");
                }
                row.Tag = table.Rows[i]["ProvNum"];
                gridStudents.ListGridRows.Add(row);
            }
            gridStudents.EndUpdate();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.Rows[i]["ProvNum"].ToString() == selectedProvNum.ToString())
                {
                    gridStudents.SetSelected(i, true);
                    break;
                }
            }
        }

        private void FillDispSupply()
        {
            DataTable table = DispSupplies.RefreshDispensary(PIn.Long(textProvNum.Text));
            gridDispSupply.BeginUpdate();
            gridDispSupply.Columns.Clear();
            GridColumn col;
            col = new GridColumn(Lan.g("TableProviderSetup", "DateDispensed"), 100);
            gridDispSupply.Columns.Add(col);
            col = new GridColumn(Lan.g("TableProviderSetup", "Description"), 90);
            gridDispSupply.Columns.Add(col);
            col = new GridColumn(Lan.g("TableProviderSetup", "Qty"), 40);
            gridDispSupply.Columns.Add(col);
            col = new GridColumn(Lan.g("TableProviderSetup", "Note"), 100);
            gridDispSupply.Columns.Add(col);
            gridDispSupply.ListGridRows.Clear();
            GridRow row;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                row = new GridRow();
                row.Cells.Add(table.Rows[i]["DateDispensed"].ToString());
                row.Cells.Add(table.Rows[i]["Descript"].ToString());
                row.Cells.Add(table.Rows[i]["DispQuantity"].ToString());
                row.Cells.Add(table.Rows[i]["Note"].ToString());
                gridDispSupply.ListGridRows.Add(row);
            }
            gridDispSupply.EndUpdate();
        }

        private void gridStudents_CellClick(object sender, ODGridClickEventArgs e)
        {
            FillDispSupply();
        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}