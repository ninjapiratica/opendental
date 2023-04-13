using CodeBase;
using System;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormEhrEduBrowser : FormODBase
    {
        public string ResourceURL;
        public bool DidPrint;

        public FormEhrEduBrowser(string resourceURL)
        {
            ResourceURL = resourceURL;
            InitializeComponent();
            InitializeLayoutManager();
        }

        private void FormEduBrowser_Load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                webBrowser1.Url = new Uri(ResourceURL);
            }
            catch (UriFormatException ex)
            {
                ex.DoNothing();
                MessageBox.Show("The specified URL is in an incorrect format.  Did you include the http:// ?");
                DialogResult = DialogResult.Cancel;
            }
            Cursor = Cursors.Default;
        }

        private void butPrint_Click(object sender, EventArgs e)
        {
            //use the modeless version, which also allows user to choose printer
            webBrowser1.ShowPrintDialog();
            DidPrint = true;
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}
