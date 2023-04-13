using System;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormMedicationUpdate : FormODBase
    {
        public FormMedicationUpdate()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
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