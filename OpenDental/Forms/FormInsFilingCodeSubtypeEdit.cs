using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace OpenDental
{
    /// <summary></summary>
    public partial class FormInsFilingCodeSubtypeEdit : FormODBase
    {
        public InsFilingCodeSubtype InsFilingCodeSubtypeCur;

        ///<summary></summary>
        public FormInsFilingCodeSubtypeEdit()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormInsFilingCodeEdit_Load(object sender, System.EventArgs e)
        {
            textDescription.Text = InsFilingCodeSubtypeCur.Descript;
        }

        private void butDelete_Click(object sender, System.EventArgs e)
        {
            if (InsFilingCodeSubtypeCur.IsNew)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }
            if (!MsgBox.Show(this, MsgBoxButtons.OKCancel, "Delete?"))
            {
                return;
            }
            try
            {
                InsFilingCodeSubtypes.Delete(InsFilingCodeSubtypeCur.InsFilingCodeSubtypeNum);
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butOK_Click(object sender, System.EventArgs e)
        {
            if (this.textDescription.Text == "")
            {
                MessageBox.Show(Lan.g(this, "Please enter a description."));
                return;
            }
            InsFilingCodeSubtypeCur.Descript = textDescription.Text;
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}





















