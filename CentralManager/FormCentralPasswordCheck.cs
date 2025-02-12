using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace CentralManager
{
    public partial class FormCentralPasswordCheck : Form
    {

        public FormCentralPasswordCheck()
        {
            InitializeComponent();
        }

        private void FormCentralPasswordCheck_Load(object sender, EventArgs e)
        {

        }

        private void butOK_Click(object sender, EventArgs e)
        {
            string hashCur = PrefC.GetString(PrefName.CentralManagerPassHash);
            string saltCur = PrefC.GetString(PrefName.CentralManagerPassSalt);
            bool result = Authentication.CheckPassword(textPassword.Text, saltCur, hashCur);
            if (!result)
            {
                MessageBox.Show("Bad password.");
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