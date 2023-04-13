using CodeBase;
using Microsoft.Web.WebView2.Core;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OpenDental
{

    public partial class FormEServicesSignup : FormODBase
    {
        private WebServiceMainHQProxy.EServiceSetup.SignupOut _signupOut;

        public FormEServicesSignup(WebServiceMainHQProxy.EServiceSetup.SignupOut signupOut = null)
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
            _signupOut = signupOut;
        }

        private async void FormEServicesSignup_Load(object sender, EventArgs e)
        {
            if (ODBuild.IsWeb())
            {
                if (_signupOut == null)
                {
                    _signupOut = FormEServicesSetup.GetSignupOut();
                }
                UIHelper.ForceBringToFront(this);
                Process.Start(_signupOut.SignupPortalUrl);
                DialogResult = DialogResult.Abort;
                return;
            }
            try
            {
                await webViewMain.Init();
            }
            catch (Exception ex)
            {
                ex.DoNothing();
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }
            FillMenu();
            Text = Lan.g(this, "Loading") + "...";
            if (_signupOut == null)
            {
                _signupOut = FormEServicesSetup.GetSignupOut();
            }
            ODException.SwallowAnyException(() =>
            {
                if (ODBuild.IsDebug())
                {
                    _signupOut.SignupPortalUrl = _signupOut.SignupPortalUrl.Replace("https://www.patientviewer.com/SignupPortal/GWT/SignupPortal/SignupPortal.html", "http://127.0.0.1:8888/SignupPortal.html");
                }
                webViewMain.CoreWebView2.Navigate(_signupOut.SignupPortalUrl);
            });
        }

        private void FillMenu()
        {
            MenuItemOD menuItemSettings = new MenuItemOD("Settings");
            menuMain.Add(menuItemSettings);
            menuItemSettings.Add("Clear Browser Cache", clearCacheToolStripMenuItem_Click);
        }

        private void webViewMain_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            SetTitle();
        }

        private void SetTitle()
        {
            Text = Lan.g(this, "eServices");
            if (webViewMain.CoreWebView2 != null && !string.IsNullOrWhiteSpace(webViewMain.CoreWebView2.DocumentTitle))
            {
                Text += " - " + webViewMain.CoreWebView2.DocumentTitle;
            }
        }

        private void clearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webViewMain.ClearCache();
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}