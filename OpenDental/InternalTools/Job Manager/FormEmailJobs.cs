using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormEmailJobs : FormODBase
    {
        public bool IsNew;
        //private List<EmailAddress> _listEmailAddresses;
        public Job JobCur;
        private List<Jobs.JobEmail> _listJobEmails;
        private List<FeatureRequest> _listFeatureReqs;
        private bool _isSent;

        ///<summary></summary>
        public FormEmailJobs()
        {
            InitializeComponent();// Required for Windows Form Designer support
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormEmailJobs_Load(object sender, System.EventArgs e)
        {
            List<long> frNums = JobCur.ListJobLinks.FindAll(x => x.LinkType == JobLinkType.Request).Select(x => x.FKey).ToList();
            _listFeatureReqs = FeatureRequests.GetAll(frNums);
            _listJobEmails = Jobs.GetCustomerEmails(JobCur)
                .OrderByDescending(x => x.IsQuote)
                .ThenByDescending(x => x.IsTask)
                .ThenByDescending(x => x.IsFeatureReq)
                .ThenByDescending(x => x.PledgeAmount)
                .ThenByDescending(x => x.Votes)
                .ThenBy(x => x.Pat.PatNum).ToList();
            FillComboEmail();
            FillGridRecipients();
            textBodyTextTemplate.Text = PrefC.GetString(PrefName.JobManagerDefaultEmail);
            textPledgeTemplate.Text = PrefC.GetString(PrefName.JobManagerDefaultBillingMsg);
            textVersions.Text = JobCur.JobVersion;
            textDescriptions.Text = JobCur.Title;
            _listFeatureReqs.ForEach(x => textDescriptions.Text += "\r\n\r\nFeature Request #" + x.FeatReqNum + "\r\n" + x.Description);
        }

        private void FillComboEmail()
        {
            long curUserNum = Security.CurUser.UserNum;
            List<EmailAddress> listEmailAddresses = EmailAddresses.GetEmailAddressesForComboBoxes(curUserNum);
            textFromAddress.Text = EmailAddresses.GetByClinic(Clinics.ClinicNum).EmailUsername;
            comboEmailFrom.Items.AddList(listEmailAddresses, x => EmailAddresses.GetDisplayStringForComboBox(x, curUserNum));
            comboEmailFrom.SelectedIndex = 0;
            EmailAddress curUserEmail = listEmailAddresses.FirstOrDefault(x => x.UserNum == curUserNum);
            if (curUserEmail != null)
            {
                comboEmailFrom.SelectedIndex = 1;
                textFromAddress.Text = curUserEmail.EmailUsername;
            }
        }

        private void FillGridRecipients()
        {
            gridRecipients.BeginUpdate();
            gridRecipients.Columns.Clear();
            gridRecipients.Columns.Add(new GridColumn("Pat\r\nNum", 50));
            gridRecipients.Columns.Add(new GridColumn("Pat\r\nName", 200));
            gridRecipients.Columns.Add(new GridColumn("Email", 170));
            gridRecipients.Columns.Add(new GridColumn("Phone", 120));
            gridRecipients.Columns.Add(new GridColumn("Quote", 50) { TextAlign = HorizontalAlignment.Center });
            gridRecipients.Columns.Add(new GridColumn("Task", 50) { TextAlign = HorizontalAlignment.Center });
            gridRecipients.Columns.Add(new GridColumn("Feature", 50) { TextAlign = HorizontalAlignment.Center });
            gridRecipients.Columns.Add(new GridColumn("Pledge\r\nAmount", 75) { TextAlign = HorizontalAlignment.Right });
            gridRecipients.Columns.Add(new GridColumn("Votes", 50) { TextAlign = HorizontalAlignment.Right });
            gridRecipients.Columns.Add(new GridColumn("Send", 50) { TextAlign = HorizontalAlignment.Center });
            gridRecipients.Columns.Add(new GridColumn("Status", 50) { TextAlign = HorizontalAlignment.Center });
            gridRecipients.ListGridRows.Clear();
            foreach (Jobs.JobEmail jobEmail in _listJobEmails)
            {
                GridRow row = new GridRow();
                row.Cells.Add(jobEmail.Pat.PatNum.ToString());
                row.Cells.Add(jobEmail.Pat.GetNameFL());
                row.Cells.Add(jobEmail.EmailAddress);
                row.Cells.Add(jobEmail.PhoneNums);
                row.Cells.Add(jobEmail.IsQuote ? "X" : "");
                row.Cells.Add(jobEmail.IsTask ? "X" : "");
                row.Cells.Add(jobEmail.IsFeatureReq ? "X" : "");
                row.Cells.Add(jobEmail.PledgeAmount > 0 ? jobEmail.PledgeAmount.ToString("c") : "");
                row.Cells.Add(jobEmail.Votes.ToString());
                if (jobEmail.IsSend)
                {
                    row.Cells.Add(new GridCell("X") { ColorBackG = Color.FromArgb(236, 255, 236) });//light green;
                }
                else
                {
                    row.Cells.Add(new GridCell("") { ColorBackG = Color.FromArgb(222, 222, 222) });//light gray;
                }
                if (string.IsNullOrEmpty(jobEmail.StatusMsg))
                {
                    //do nothing
                }
                else if (jobEmail.StatusMsg == "Sent")
                {
                    row.ColorBackG = Color.FromArgb(236, 255, 236);//light green
                }
                else
                {
                    row.ColorBackG = Color.FromArgb(254, 235, 233);//light red;
                }
                row.Cells.Add(jobEmail.StatusMsg);
                gridRecipients.ListGridRows.Add(row);
            }
            gridRecipients.EndUpdate();
        }

        private void gridRecipients_CellClick(object sender, ODGridClickEventArgs e)
        {
            if (gridRecipients.Columns[e.Col].Heading == "Send")
            {
                _listJobEmails[e.Row].IsSend = !_listJobEmails[e.Row].IsSend;
                FillGridRecipients();
            }
        }

        ///<summary></summary>
        private void butSend_Click(object sender, System.EventArgs e)
        {
            EmailAddress emailAddress = comboEmailFrom.GetSelected<EmailAddress>();
            if (emailAddress == null)
            {
                MessageBox.Show("Unable to find email address.");
                return;
            }
            if (emailAddress.EmailAddressNum == 0)
            { //clinic/practice default
                emailAddress = EmailAddresses.GetByClinic(Clinics.ClinicNum);//practice or clinic.
            }
            if (string.IsNullOrWhiteSpace(textFromAddress.Text))
            {
                MsgBox.Show(this, "Please enter a sender address.");
                return;
            }
            if (_listJobEmails.All(x => !x.IsSend))
            {
                MsgBox.Show(this, "Please select at least one recipient.");
                return;
            }
            if (emailAddress.SMTPserver == "")
            {
                MsgBox.Show(this, "The email address in email setup must have an SMTP server.");
                return;
            }
            if (string.IsNullOrEmpty(textVersions.Text))
            {
                if (MessageBox.Show("No version specified, continue?", "", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return;
                }
            }
            if (string.IsNullOrEmpty(textDescriptions.Text))
            {
                if (MessageBox.Show("No description specified, continue?", "", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return;
                }
            }
            if (string.IsNullOrEmpty(textSubject.Text))
            {
                if (MessageBox.Show("No subject specified, continue?", "", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                {
                    return;
                }
            }
            StringBuilder sb = new StringBuilder();
            Cursor = Cursors.WaitCursor;
            string template = PrefC.GetString(PrefName.JobManagerDefaultEmail);
            foreach (Jobs.JobEmail je in _listJobEmails)
            {
                if (!je.IsSend)
                {
                    continue;
                }
                if (ODBuild.IsDebug())
                {
                    je.EmailAddress = "ryan@opendental.com";//send all emails to Ryan for debugging.
                }
                EmailMessage emailMessage = GetJobEmailMessage(je);
                emailMessage.SentOrReceived = EmailSentOrReceived.Sent;
                try
                {
                    EmailMessages.SendEmail(emailMessage, emailAddress);
                    je.StatusMsg = "Sent";
                    je.IsSend = false;
                }
                catch (Exception ex)
                {
                    je.StatusMsg = ex.Message;
                    sb.AppendLine(ex.Message + "\r\n");
                }
            }
            Cursor = Cursors.Default;
            _isSent = true;
            butSend.Text = "Resend";
            butCancel.Text = "Close";
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                using MsgBoxCopyPaste msgBox = new MsgBoxCopyPaste("The following errors occurred while sending emails:\r\n" + sb.ToString());
                msgBox.ShowDialog();
            }
            FillGridRecipients();
        }

        private EmailMessage GetJobEmailMessage(Jobs.JobEmail je)
        {
            EmailMessage retVal = new EmailMessage();
            retVal.PatNum = je.Pat.PatNum;
            retVal.PatNumSubj = je.Pat.PatNum;
            retVal.FromAddress = textFromAddress.Text;
            retVal.RecipientAddress = je.EmailAddress;
            retVal.Subject = textSubject.Text.Replace("[Versions]", textVersions.Text);
            retVal.ToAddress = je.EmailAddress;
            retVal.BodyText = PrefC.GetString(PrefName.JobManagerDefaultEmail);
            retVal.BodyText = retVal.BodyText.Replace("[PledgePara]", je.PledgeAmount == 0 ? "" : PrefC.GetString(PrefName.JobManagerDefaultBillingMsg));
            retVal.BodyText = retVal.BodyText.Replace("[PledgeAmount]", je.PledgeAmount.ToString("C"));
            retVal.BodyText = retVal.BodyText.Replace("[Descriptions]", textDescriptions.Text);
            retVal.BodyText = retVal.BodyText.Replace("[Versions]", textVersions.Text);
            retVal.MsgDateTime = DateTime.Now;
            retVal.MsgType = EmailMessageSource.JobManager;
            return retVal;
        }

        private void editTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using FormEmailJobTemplateEdit FormEJTE = new FormEmailJobTemplateEdit();
            FormEJTE.ShowDialog();
            if (FormEJTE.DialogResult != DialogResult.OK)
            {
                return;
            }
            textBodyTextTemplate.Text = PrefC.GetString(PrefName.JobManagerDefaultEmail);
            textPledgeTemplate.Text = PrefC.GetString(PrefName.JobManagerDefaultBillingMsg);
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }
        private void butExport_Click(object sender, EventArgs e)
        {
            if (_listJobEmails.Count == 0)
            {
                MsgBox.Show(this, "Recipient list is empty.");
                return;
            }
            Cursor = Cursors.WaitCursor;
            SaveFileDialog Dlg = new SaveFileDialog();
            if (Directory.Exists(PrefC.GetString(PrefName.ExportPath)))
            {
                Dlg.InitialDirectory = PrefC.GetString(PrefName.ExportPath);
            }
            else if (Directory.Exists("C:\\"))
            {
                Dlg.InitialDirectory = "C:\\";
            }
            Dlg.FileName = "RecipientEmailList.txt";
            if (Dlg.ShowDialog() != DialogResult.OK)
            {
                Cursor = Cursors.Default;
                return;
            }
            using (StreamWriter sr = File.CreateText(Dlg.FileName))
            {
                sr.Write("Job Description: " + JobCur.Title);
                sr.Write("\r\n\r\n");
                foreach (Jobs.JobEmail jobEmail in _listJobEmails)
                {
                    sr.Write(jobEmail.Pat.PatNum.ToString() + "\t");
                    sr.Write(jobEmail.Pat.GetNameFL() + "\t");
                    sr.Write(jobEmail.EmailAddress);
                    sr.Write("\r\n");
                }
                sr.Write("\r\n");
                sr.Write("FeatureReqNum\tDescription");
                sr.Write("\r\n\r\n");
                foreach (FeatureRequest req in _listFeatureReqs)
                {
                    sr.Write(req.FeatReqNum + "\t");
                    sr.Write(req.Description);
                }
            }
            try
            {
                System.Diagnostics.Process.Start(Dlg.FileName);
            }
            catch (Exception ex) { ex.DoNothing(); }
            Cursor = Cursors.Default;
            MsgBox.Show(this, "Recipient list exported.");
        }

        private void FormEmailJobs_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isSent)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
        }

    }
}