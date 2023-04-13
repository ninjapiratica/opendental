using CodeBase;
using System;
using System.IO;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormWikiFileFolder : FormODBase
    {
        public bool IsFolderMode;
        public string SelectedLink;

        public FormWikiFileFolder()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormWikiFileFolder_Load(object sender, EventArgs e)
        {
            if (IsFolderMode)
            {
                Text = Lan.g(this, "Insert Folder Link");
            }
        }

        private void butBrowse_Click(object sender, EventArgs e)
        {
            if (IsFolderMode)
            {
                using FolderBrowserDialog folderBD = new FolderBrowserDialog();
                if (folderBD.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                textLink.Text = folderBD.SelectedPath;
                return;
            }
            using OpenFileDialog openFileD = new OpenFileDialog();
            if (openFileD.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            textLink.Text = openFileD.FileName;
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (IsFolderMode)
            {
                if (!ODBuild.IsWeb() && !Directory.Exists(textLink.Text))
                {
                    if (!MsgBox.Show(this, MsgBoxButtons.OKCancel, "Folder does not exist. Continue anyway?"))
                    {
                        return;
                    }
                    /*try {
						Directory.CreateDirectory(textLink.Text);
					}
					catch(Exception ex) {
						MessageBox.Show(this,ex.Message);
						return;
					}*/
                }
            }
            else
            {//file mode
                if (!ODBuild.IsWeb() && !File.Exists(textLink.Text))
                {
                    if (!MsgBox.Show(this, MsgBoxButtons.OKCancel, "File does not exist. Continue anyway?"))
                    {
                        return;
                    }
                }
            }
            SelectedLink = textLink.Text;
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}