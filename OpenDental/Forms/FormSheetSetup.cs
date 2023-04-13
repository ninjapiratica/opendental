using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormSheetSetup : FormODBase
    {
        private bool _isChanged;

        public FormSheetSetup()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormReportSetup_Load(object sender, EventArgs e)
        {
            checkPatientFormsShowConsent.Checked = PrefC.GetBool(PrefName.PatientFormsShowConsent);
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (Prefs.UpdateBool(PrefName.PatientFormsShowConsent, checkPatientFormsShowConsent.Checked))
            {
                _isChanged = true;
            }
            if (_isChanged)
            {
                DataValid.SetInvalid(InvalidType.Prefs);
            }
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


    }
}