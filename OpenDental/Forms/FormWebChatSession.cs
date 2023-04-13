using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormWebChatSession : FormODBase
    {

        private WebChatSession _webChatSession = null;
        private FormWebChatSessionNoteEdit _formWebChatSessionNoteEdit = null;
        private Action _onCloseAction = null;
        private Patient _suggestedPatient = null;
        private int _widthPrevious;
        private long _messageCountPrev;
        private long _messageCountCur;
        ///<summary>This form is only displayed when accessed from the triage list at ODHQ.</summary>
        public FormWebChatSession(WebChatSession webChatSession, Action onCloseAction)
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
            _webChatSession = webChatSession;
            _onCloseAction = onCloseAction;
        }

        private void FormWebChatSession_Load(object sender, EventArgs e)
        {
            if (_webChatSession.DateTend.Year > 1880)
            {//Session has ended?
                DisableAllExcept(textOwner, textWebChatSessionNum, textName, textEmail, textPractice, textPhone, webChatThread, butClose, butAttachSuggestion, butSearchAndAttach, textCustomer
                    , tabControlMain);
                //tabControlMain needs to be enabled to view notes, but these options still need to be disabled.
                butSend.Enabled = false;
                textChatMessage.Enabled = false;
            }
            try
            {
                //auto-suggest patient based on supplied phone number if a patient is not yet associated with the session
                if (_webChatSession.PhoneNumber != "" && _webChatSession.PatNum == 0)
                {
                    List<Patient> listPatients = new List<Patient>();
                    listPatients.AddRange(Patients.GetPatientsByPhone(_webChatSession.PhoneNumber, ""));
                    if (listPatients.Count == 1)
                    { //only one patient matches the supplied phone number
                        _suggestedPatient = listPatients.FirstOrDefault();
                        butAttachSuggestion.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DoNothing(); //Revert to original form behavior if finding a customer fails
            }
            butAddNote.Visible = false;
            FillSession();
            FillGridODNotes();
            FillMessageThread();
            _widthPrevious = ((Control)sender).Size.Width;
        }

        private void FillGridODNotes()
        {
            if (gridODNotes.Columns.Count == 0)
            {
                GridColumn col = new GridColumn("Date", 65);
                gridODNotes.Columns.Add(col);
                col = new GridColumn("Tech", 85);
                gridODNotes.Columns.Add(col);
                col = new GridColumn("Note", 90);
                gridODNotes.Columns.Add(col);
            }
            List<WebChatNote> listWebChatNotes = WebChatNotes.GetAllForSessions(new List<long>() { _webChatSession.WebChatSessionNum });
            GridRow row;
            GridCell cell;
            gridODNotes.BeginUpdate();
            gridODNotes.ListGridRows.Clear();
            for (int i = 0; i < listWebChatNotes.Count; i++)
            {
                row = new GridRow();
                cell = new GridCell(listWebChatNotes[i].DateTimeNote.ToShortDateString());
                row.Cells.Add(cell);
                cell = new GridCell(listWebChatNotes[i].TechName);
                row.Cells.Add(cell);
                cell = new GridCell(listWebChatNotes[i].Note);
                row.Cells.Add(cell);
                row.Tag = listWebChatNotes[i];
                gridODNotes.ListGridRows.Add(row);
            }
            gridODNotes.EndUpdate();
        }

        public override void ProcessSignalODs(List<Signalod> listSignals)
        {
            if (listSignals.Exists(x => x.IType == InvalidType.WebChatSessions && x.FKey == _webChatSession.WebChatSessionNum))
            {
                _webChatSession = WebChatSessions.GetOne(_webChatSession.WebChatSessionNum);//Refresh the session in case the owner changed.
                FillSession();
                FillGridODNotes();
                FillMessageThread();
                tabControlMain.Refresh(); //Redraw tabs to color Messages tab, if necessary
            }
        }

        private void FillSession()
        {
            if (_suggestedPatient != null)
            { //using suggested patient information if present
                textCustomer.Text = _suggestedPatient.GetNameFL();
            }
            else
            {
                textCustomer.Text = (_webChatSession.PatNum == 0) ? "" : Patients.GetNameLF(_webChatSession.PatNum);
            }
            textOwner.Text = _webChatSession.TechName;
            textName.Text = _webChatSession.UserName;
            textPractice.Text = _webChatSession.PracticeName;
            textWebChatSessionNum.Text = POut.Long(_webChatSession.WebChatSessionNum);
            textEmail.Text = _webChatSession.EmailAddress;
            textPhone.Text = _webChatSession.PhoneNumber;
            checkIsCustomer.Checked = _webChatSession.IsCustomer;
        }

        private void FillMessageThread()
        {
            List<WebChatMessage> listWebChatMessages = WebChatMessages.GetAllForSessions(_webChatSession.WebChatSessionNum);
            List<SmsThreadMessage> listThreadMessages = new List<SmsThreadMessage>();
            foreach (WebChatMessage webChatMessage in listWebChatMessages)
            {
                string strMsg = webChatMessage.MessageText;
                if (webChatMessage.MessageType == WebChatMessageType.EndSession)
                {
                    strMsg = MarkupEdit.ConvertToPlainText(strMsg);
                    butEndSession.Enabled = false;
                    butSend.Enabled = false;
                    textChatMessage.Text = "";
                    textChatMessage.Enabled = false;
                }
                SmsThreadMessage msg = new SmsThreadMessage(webChatMessage.WebChatMessageNum.ToString(),
                    webChatMessage.DateT,
                    strMsg,
                    (webChatMessage.MessageType == WebChatMessageType.Customer),
                    false,
                    false,
                    SmsDeliveryStatus.None,
                    webChatMessage.UserName
                );
                listThreadMessages.Add(msg);
            }
            webChatThread.ListSmsThreadMessages = listThreadMessages;
            _messageCountCur = listWebChatMessages.Count;
        }

        private void textChatMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.Handled = true;
                SendMessage();
            }
        }

        private void SendMessage()
        {
            WebChatSessions.SendTechMessage(_webChatSession.WebChatSessionNum, textChatMessage.Text);
            textChatMessage.Text = "";
            FillMessageThread();
            _messageCountPrev = _messageCountCur;
        }

        private void butSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void butSearchAndAttach_Click(object sender, EventArgs e)
        {
            //prefill FormPatientSelect with the customer-provided phone number
            Patient pat = new Patient() { HmPhone = textPhone.Text };
            using FormPatientSelect formPS = new FormPatientSelect(pat);
            if (formPS.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            pat = Patients.GetPat(formPS.PatNumSelected);
            FormOpenDental.S_Contr_PatientSelected(pat, true);
            textCustomer.Text = pat.GetNameLF();
            WebChatSession oldWebChatSession = _webChatSession.Clone();
            _webChatSession.PatNum = pat.PatNum;
            WebChatSessions.Update(_webChatSession, oldWebChatSession);//update here so we can associate pats after chat has ended, or before ownership. Updated immediately for concurrency. 
            butAttachSuggestion.Enabled = false; //Attached searched-for patient instead of suggested one.
            _suggestedPatient = null;
        }

        ///<summary> Attaches a customer to the session if one has been suggested. </summary>
        private void butAttachSuggestion_Click(object sender, EventArgs e)
        {
            WebChatSession webChatSessionOld = _webChatSession.Clone();
            _webChatSession.PatNum = _suggestedPatient.PatNum;
            WebChatSessions.Update(_webChatSession, webChatSessionOld);//Updated immediately for concurrency. 
            butAttachSuggestion.Enabled = false;
            FormOpenDental.S_Contr_PatientSelected(_suggestedPatient, true);
        }

        private void butEndSession_Click(object sender, EventArgs e)
        {
            //Refresh the session in case the session ended in less than the last signal interval.
            _webChatSession = WebChatSessions.GetOne(_webChatSession.WebChatSessionNum);
            if (_webChatSession.DateTend.Year > 1880)
            { //session has already ended. Will not send second End Session message
                FillMessageThread();
                butEndSession.Enabled = false;
                return;
            }
            if (!MsgBox.Show(this, MsgBoxButtons.YesNo, "Permanently end the session for everyone?"))
            {
                return;
            }
            WebChatSessions.EndSession(_webChatSession.WebChatSessionNum);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void butTakeOwnership_Click(object sender, EventArgs e)
        {
            TakeOwnership();
        }

        private void TakeOwnership()
        {
            //Refresh the session in case the owner changed in less than the last signal interval.
            _webChatSession = WebChatSessions.GetOne(_webChatSession.WebChatSessionNum);
            bool isOkToTake = true;
            if (_webChatSession.TechName == Security.CurUser.UserName)
            {
                //Ownership is already claimed.  Do not run a pointless update command.
                isOkToTake = false;
            }
            else if (!String.IsNullOrEmpty(_webChatSession.TechName))
            {
                if (MessageBox.Show(Lan.g(this, "This session is owned by another technician.  Take this session from user") + " " + _webChatSession.TechName + "?",
                    "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    isOkToTake = false;
                }
            }
            if (isOkToTake)
            {
                WebChatSession oldWebChatSession = _webChatSession.Clone();
                _webChatSession.TechName = Security.CurUser.UserName;
                WebChatSessions.Update(_webChatSession, oldWebChatSession);
            }
            textOwner.Text = _webChatSession.TechName;
            if (isOkToTake)
            {//show after previous text has changed
                MessageBox.Show(Lan.g(this, "Session is now owned by") + " " + _webChatSession.TechName);
            }
        }

        ///<summary>Event used to color Messages tab when there are new, unseen messages from the customer while the tech is on the Notes tab. </summary>
        private void tabControlMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            Rectangle tabArea = tabControlMain.GetTabRect(e.Index);
            System.Windows.Forms.TabPage tabPage = tabControlMain.TabPages[e.Index];
            //No indication color for Notes tab
            if (tabPage != tabPageMessages)
            {
                TextRenderer.DrawText(e.Graphics, tabPage.Text, Font, tabArea, tabPage.ForeColor);
                return;
            }
            //There are new unseen messages, color Messages tab
            if (_messageCountCur > _messageCountPrev && tabControlMain.SelectedTab != tabPageMessages)
            { //there are new unseen messages
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(153, 153, 255)), tabArea);
            }
            else
            { //No Messages tab color
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Control), tabArea);
                _messageCountPrev = _messageCountCur;
            }
            TextRenderer.DrawText(e.Graphics, tabPage.Text, Font, tabArea, tabPage.ForeColor);
        }

        ///<summary>Ensures the Messages tab retains its color (if new messages) until clicked on. </summary>
        private void tabPageMessages_Click(object sender, EventArgs e)
        {
            _messageCountPrev = _messageCountCur;
            tabControlMain.Refresh();
        }

        private bool HasAnotherNoteOpen()
        {
            if (_formWebChatSessionNoteEdit != null && !_formWebChatSessionNoteEdit.IsDisposed)
            {
                MsgBox.Show(this, "One or more web chat session note edit windows are open and must be closed.");
                return true;
            }
            return false;
        }

        private void butAddNote_Click(object sender, EventArgs e)
        {
            if (HasAnotherNoteOpen())
            {
                return;
            }
            WebChatNote webChatNote = new WebChatNote();
            webChatNote.WebChatSessionNum = _webChatSession.WebChatSessionNum;
            webChatNote.TechName = _webChatSession.TechName;
            webChatNote.DateTimeNote = DateTime.Now;
            webChatNote.IsNew = true;
            _formWebChatSessionNoteEdit = new FormWebChatSessionNoteEdit(webChatNote);
            _formWebChatSessionNoteEdit.ShowDialog(this);
            _formWebChatSessionNoteEdit.Dispose();
            FillGridODNotes();
        }

        private void gridODNotes_CellDoubleClick(object sender, UI.ODGridClickEventArgs e)
        {
            if (HasAnotherNoteOpen() || gridODNotes.SelectedTag<WebChatNote>() == null)
            {
                return;
            }

            long webChatNoteNumCur = gridODNotes.SelectedTag<WebChatNote>().WebChatNoteNum;//(WebChatSession)gridWebChatSurveys.ListGridRows[e.Row].Tag
            WebChatNote webChatNote = WebChatNotes.GetOne(webChatNoteNumCur);
            if (webChatNote == null)
            {
                MsgBox.Show(this, "This web chat session note was deleted from elsewhere. Unable to display.");
                return;
            }
            _formWebChatSessionNoteEdit = new FormWebChatSessionNoteEdit(webChatNote);
            _formWebChatSessionNoteEdit.ShowDialog(this);
            _formWebChatSessionNoteEdit.Dispose();
            FillGridODNotes();
        }

        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.TabPage tabPageNotes = tabControlMain.TabPages.AsEnumerable<System.Windows.Forms.TabPage>().FirstOrDefault(x => x.Name == "tabPageNotes");
            if (tabPageNotes == null)
            {
                return;//Something went wrong, leave the add button in the state it is in.
            }
            if (tabControlMain.SelectedTab == tabPageNotes)
            {
                butAddNote.Visible = true;
                FillGridODNotes();
                return;
            }
            butAddNote.Visible = false;//No need to add notes from the messaging screen.
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormWebChatSession_FormClosing(object sender, FormClosingEventArgs e)
        {
            WebChatSession webChatSessionOld = _webChatSession.Clone();
            WebChatSessions.Update(_webChatSession, webChatSessionOld);
            _onCloseAction();
        }

        ///<summary> When resizing the form, redraw all messages if the width changed.
        /// A redraw is necessary to update the width and height of each message.
        /// If the width didn't change, no need to redraw. </summary>
        private void FormWebChatSession_ResizeEnd(object sender, EventArgs e)
        {
            int previousWidth = ((Control)sender).Size.Width;
            if (previousWidth != _widthPrevious)
            {
                FillMessageThread();
                FillGridODNotes();
                _widthPrevious = previousWidth;
            }
        }
    }
}