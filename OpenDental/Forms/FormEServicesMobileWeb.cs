using OpenDentBusiness;
using System;
using System.Linq;

namespace OpenDental
{

    public partial class FormEServicesMobileWeb : FormODBase
    {
        private WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;

        public FormEServicesMobileWeb(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut = null)
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
            _signupOut = signupOut;
        }

        private void FormEServicesMobileWeb_Load(object sender, EventArgs e)
        {
            if (_signupOut == null)
            {
                _signupOut = FormEServicesSetup.GetSignupOut();
            }
            WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService signupOutEService =
                WebServiceMainHQProxy.GetSignups<WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService>(_signupOut, eServiceCode.MobileWeb).FirstOrDefault();
            if (signupOutEService == null)
            {
                signupOutEService = new WebServiceMainHQProxy.EServiceSetup.SignupOut.SignupOutEService();
                signupOutEService.HostedUrl = "";
            }
            string urlFromHQ = signupOutEService.HostedUrl;
            textHostedUrlMobileWeb.Text = urlFromHQ;
        }

        private void butSetupMobileWebUsers_Click(object sender, EventArgs e)
        {
            FormOpenDental.S_MenuItemSecurity_Click(sender, e);
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}