using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormWikiAllPages : FormODBase
    {
        ///<summary>Need a reference to the form where this was launched from so that we can tell it to refresh later.</summary>
        public FormWikiEdit OwnerForm;

        public FormWikiAllPages()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormWikiAllPages_Load(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            FillGrid();
        }

        private void LoadWikiPage(string wikiPageTitle)
        {
            bool includeArchived = checkIncludeArchived.Checked;
            WikiPage wikiPage = WikiPages.GetByTitle(wikiPageTitle, isDeleted: includeArchived);
            try
            {
                webBrowserWiki.DocumentText = MarkupEdit.TranslateToXhtml(wikiPage.PageContent, isPreviewOnly: true); ;
            }
            catch (Exception ex)
            {
                webBrowserWiki.DocumentText = "";
                MessageBox.Show(this, Lan.g(this, "This page is broken and cannot be viewed.  Error message:") + " " + ex.Message);
            }
        }

        /// <summary></summary>
        private void FillGrid()
        {
            bool includeArchived = checkIncludeArchived.Checked;
            gridMain.BeginUpdate();
            gridMain.Columns.Clear();
            GridColumn col = new GridColumn(Lan.g(this, "Title"), 70);
            gridMain.Columns.Add(col);
            gridMain.ListGridRows.Clear();
            List<string> listWikiPageTitles = WikiPages.GetForSearch(textSearch.Text, true, isDeleted: includeArchived);
            for (int i = 0; i < listWikiPageTitles.Count; i++)
            {
                GridRow row = new GridRow();
                string wikiPageTitle = PIn.String(listWikiPageTitles[i]);
                row.Tag = wikiPageTitle;
                row.Cells.Add(wikiPageTitle);
                gridMain.ListGridRows.Add(row);
            }
            gridMain.EndUpdate();
        }

        private void gridMain_CellClick(object sender, ODGridClickEventArgs e)
        {
            webBrowserWiki.AllowNavigation = true;
            LoadWikiPage(gridMain.SelectedTag<string>());
            gridMain.Focus();
        }

        private void gridMain_CellDoubleClick(object sender, UI.ODGridClickEventArgs e)
        {
            if (OwnerForm != null && !OwnerForm.IsDisposed)
            {
                WikiPage wikiPageSelected = WikiPages.GetByTitle(gridMain.SelectedTag<string>());
                OwnerForm.RefreshPage(wikiPageSelected);
            }
            Close();
        }

        private void webBrowserWiki_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            webBrowserWiki.AllowNavigation = false;//to disable links in pages.
        }

        ///<summary>Adds a new wikipage.</summary>
        private void butAdd_Click(object sender, EventArgs e)
        {
            using FormWikiRename FormWR = new FormWikiRename();
            FormWR.ShowDialog();
            if (FormWR.DialogResult != DialogResult.OK)
            {
                return;
            }
            Action<string> onWikiSaved = new Action<string>((pageTitleNew) =>
            {
                //return the new wikipage added to FormWikiEdit
                WikiPage wp = WikiPages.GetByTitle(pageTitleNew);
                if (wp != null && OwnerForm != null && !OwnerForm.IsDisposed)
                {
                    OwnerForm.RefreshPage(wp);
                }
            });
            FormWikiEdit FormWE = new FormWikiEdit(onWikiSaved);
            FormWE.WikiPageCur = new WikiPage();
            FormWE.WikiPageCur.IsNew = true;
            FormWE.WikiPageCur.PageTitle = FormWR.PageTitle;
            FormWE.WikiPageCur.PageContent = "[[" + OwnerForm.WikiPageCur.PageTitle + "]]\r\n"//link back
                + "<h1>" + FormWR.PageTitle + "</h1>\r\n";//page title
            FormWE.Show();
            Close();
        }

        /// <summary></summary>
        private void butBrackets_Click(object sender, EventArgs e)
        {
            if (OwnerForm != null && !OwnerForm.IsDisposed)
            {
                OwnerForm.RefreshPage(null);
            }
            Close();
        }

        private void checkIncludeArchived_CheckedChanged(object sender, EventArgs e)
        {
            FillGrid();
        }

        /// <summary></summary>
        private void butOK_Click(object sender, EventArgs e)
        {
            if (gridMain.GetSelectedIndex() == -1)
            {
                MsgBox.Show(this, "Please select a page first.");
                return;
            }
            if (OwnerForm != null && !OwnerForm.IsDisposed)
            {
                WikiPage wikiPageSelected = WikiPages.GetByTitle(gridMain.SelectedTag<string>());
                OwnerForm.RefreshPage(wikiPageSelected);
            }
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            Close();
        }








    }
}