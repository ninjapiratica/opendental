using System;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormQuickBooksOnlineAuthorization : FormODBase
    {
        ///<summary>The authorization code entered by the user.</summary>
        public string AuthCode = "";
        ///<summary>The realm Id entered by the user.</summary>
        public string RealmId = "";

        public FormQuickBooksOnlineAuthorization()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            AuthCode = textAuthCode.Text;
            RealmId = textRealmId.Text;
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}