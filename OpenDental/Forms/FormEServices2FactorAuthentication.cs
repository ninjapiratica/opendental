using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormEServices2FactorAuthentication : FormODBase
    {
        public WebServiceMainHQProxy.MobileSettingsAuth MobileSettingsAuth;

        public FormEServices2FactorAuthentication()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (text2FactorAuthCode.Text != MobileSettingsAuth.AuthCodeEmail && text2FactorAuthCode.Text != MobileSettingsAuth.AuthCodePhone)
            {
                MessageBox.Show("The given code did not match. Enter a valid code, or hit Cancel to send a new code.");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}