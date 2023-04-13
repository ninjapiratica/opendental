using System;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormExternalLink : FormODBase
    {
        public string URL;
        public string DisplayText;

        ///<summary>Pass in values that will override the title of the form or the label text next to the corresponding text boxes.</summary>
        public FormExternalLink(string title = "", string url = "", string displayText = "")
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
            if (!string.IsNullOrEmpty(title))
            {
                this.Text = title;
            }
            if (!string.IsNullOrEmpty(url))
            {
                labelUrl.Text = url;
            }
            if (!string.IsNullOrEmpty(displayText))
            {
                labelDisplayText.Text = displayText;
            }
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            URL = textURL.Text;
            DisplayText = textDisplay.Text;
            DialogResult = DialogResult.OK;
        }

        private void butEmptyLink_Click(object sender, EventArgs e)
        {
            URL = "";
            DisplayText = "";
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}