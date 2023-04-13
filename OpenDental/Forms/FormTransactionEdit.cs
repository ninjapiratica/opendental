using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace OpenDental
{
    /// <summary>
    /// Summary description for FormBasicTemplate.
    /// </summary>
    public partial class FormTransactionEdit : FormODBase
    {
        private Transaction _transaction;
        private List<JournalEntry> _listJournalEntries;
        private List<JournalEntry> _listJournalEntriesOld;
        ///<summary>The account where the edit originated from.  This affects how the simple version gets displayed, and the signs on debit and credit.</summary>
        private Account AccountOfOrigin;
        ///<summary>When in simple mode, this is the 'other' account, not the one of origin.  It can be null.</summary>
        private Account AccountPicked;

        ///<summary>Just used for security.</summary>
        public bool IsNew;

        ///<summary></summary>
        public FormTransactionEdit(long transNum, long accountNum)
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
            _transaction = Transactions.GetTrans(transNum);
            AccountOfOrigin = Accounts.GetAccount(accountNum);
            //AccountNumOrigin=accountNumOrigin;
        }

        private void FormTransactionEdit_Load(object sender, EventArgs e)
        {
            _listJournalEntries = JournalEntries.GetForTrans(_transaction.TransactionNum);//Count might be 0
            if (IsNew)
            {
                if (!Security.IsAuthorized(Permissions.AccountingCreate, DateTime.Today))
                {
                    //we will check the date again when saving
                    DialogResult = DialogResult.Cancel;
                    return;
                }
            }
            else
            {
                //for an existing transaction, there will always be at least 2 entries.
                //We enforce security here based on date displayed, not date entered	
                if (!Security.IsAuthorized(Permissions.AccountingEdit, ((JournalEntry)_listJournalEntries[0]).DateDisplayed))
                {
                    butOK.Enabled = false;
                    butDelete.Enabled = false;
                }
            }
            _listJournalEntriesOld = new List<JournalEntry>();
            for (int i = 0; i < _listJournalEntries.Count; i++)
            {
                _listJournalEntriesOld.Add(_listJournalEntries[i].Copy());
            }
            textDateTimeEntered.Text = _transaction.DateTimeEntry.ToString();
            textDateTimeEdited.Text = _transaction.SecDateTEdit.ToString();
            textUserEntered.Text = Userods.GetName(_transaction.UserNum);
            textUserEdited.Text = Userods.GetName(_transaction.SecUserNumEdit);
            if (_listJournalEntries.Count > 0)
            {
                textDate.Text = _listJournalEntries[0].DateDisplayed.ToShortDateString();
            }
            else
            {
                textDate.Text = DateTime.Today.ToShortDateString();
            }
            if (AccountOfOrigin == null)
            {//if accessed from within a payment screen instead of through accounting
                checkSimple.Checked = false;
                checkSimple.Visible = false;//don't allow user to switch back to simple view
                FillCompound();
            }
            else if (JournalEntries.AttachedToReconcile(_listJournalEntries))
            {
                labelReconcileDate.Visible = true;
                textReconcileDate.Visible = true;
                textReconcileDate.Text = JournalEntries.GetReconcileDate(_listJournalEntries).ToShortDateString();
                checkSimple.Checked = false;
                checkSimple.Visible = false;//don't allow user to switch back to simple view
                FillCompound();
            }
            else if (_listJournalEntries.Count > 2)
            {//compound
                checkSimple.Checked = false;
                FillCompound();
            }
            //so count must be 0 or 2
            else if (_listJournalEntries.Count == 2 && ((JournalEntry)_listJournalEntries[0]).Memo != ((JournalEntry)_listJournalEntries[1]).Memo)
            {
                //would be simple view, except memo's are different
                checkSimple.Checked = false;
                FillCompound();
            }
            else
            {//simple
                checkSimple.Checked = true;
                FillSimple();
            }
            if (_transaction.DepositNum == 0)
            {
                butAttachDep.Text = Lan.g(this, "Attach");
            }
            else
            {
                Deposit dep = Deposits.GetOne(_transaction.DepositNum);
                textSourceDeposit.Text = dep.DateDeposit.ToShortDateString()
                    + "  " + dep.Amount.ToString("c");
                butAttachDep.Text = Lan.g(this, "Detach");
            }
            if (_transaction.PayNum == 0)
            {
                //no way to attach yet.  This can be added later.
                butAttachPay.Visible = false;
            }
            else
            {
                Payment pay = Payments.GetPayment(_transaction.PayNum);
                textSourcePay.Text = Patients.GetPat(pay.PatNum).GetNameFL() + " "
                    + pay.PayDate.ToShortDateString() + " " + pay.PayAmt.ToString("c");
                butAttachPay.Text = Lan.g(this, "Detach");
            }
            if (_transaction.TransactionInvoiceNum == 0)
            {
                butOpenInvoice.Enabled = false;
            }
            else
            {
                butAttachInvoice.Text = Lan.g(this, "Detach");
                textSourceInvoice.Text = TransactionInvoices.GetName(_transaction.TransactionInvoiceNum);
                butOpenInvoice.Enabled = true;
            }
        }

        ///<summary>Used when compound view (3 or more journal entries).  Account nums allowed to be 0.</summary>
        private void FillCompound()
        {
            panelSimple.Visible = false;
            panelCompound.Visible = true;
            bool memoSame = true;
            string memo = "";
            double debits = 0;
            double credits = 0;
            gridMain.BeginUpdate();
            gridMain.Columns.Clear();
            GridColumn col = new GridColumn(Lan.g("TableTransSplits", "Account"), 150);
            gridMain.Columns.Add(col);
            string str = Lan.g("TableTransSplits", "Debit");
            if (AccountOfOrigin != null)
            {
                if (Accounts.DebitIsPos(AccountOfOrigin.AcctType))
                {
                    str += Lan.g(this, "(+)");
                }
                else
                {
                    str += Lan.g(this, "(-)");
                }
            }
            col = new GridColumn(str, 70, HorizontalAlignment.Right);
            gridMain.Columns.Add(col);
            str = Lan.g("TableTransSplits", "Credit");
            if (AccountOfOrigin != null)
            {
                if (Accounts.DebitIsPos(AccountOfOrigin.AcctType))
                {
                    str += Lan.g(this, "(-)");
                }
                else
                {
                    str += Lan.g(this, "(+)");
                }
            }
            col = new GridColumn(str, 70, HorizontalAlignment.Right);
            gridMain.Columns.Add(col);
            col = new GridColumn(Lan.g("TableTransSplits", "Memo"), 200);
            gridMain.Columns.Add(col);
            gridMain.ListGridRows.Clear();
            GridRow row;
            for (int i = 0; i < _listJournalEntries.Count; i++)
            {
                row = new GridRow();
                row.Cells.Add(Accounts.GetDescript(((JournalEntry)_listJournalEntries[i]).AccountNum));
                if (((JournalEntry)_listJournalEntries[i]).DebitAmt == 0)
                {
                    row.Cells.Add("");
                }
                else
                {
                    row.Cells.Add(((JournalEntry)_listJournalEntries[i]).DebitAmt.ToString("n"));
                }
                if (((JournalEntry)_listJournalEntries[i]).CreditAmt == 0)
                {
                    row.Cells.Add("");
                }
                else
                {
                    row.Cells.Add(((JournalEntry)_listJournalEntries[i]).CreditAmt.ToString("n"));
                }
                row.Cells.Add(((JournalEntry)_listJournalEntries[i]).Memo);
                gridMain.ListGridRows.Add(row);
                if (i == 0)
                {
                    memo = ((JournalEntry)_listJournalEntries[i]).Memo;
                }
                else
                {
                    if (memo != ((JournalEntry)_listJournalEntries[i]).Memo)
                    {
                        memoSame = false;
                    }
                }
                credits += ((JournalEntry)_listJournalEntries[i]).CreditAmt;
                debits += ((JournalEntry)_listJournalEntries[i]).DebitAmt;
            }
            gridMain.EndUpdate();
            checkMemoSame.Checked = memoSame;
            textCredit.Text = credits.ToString("n");
            textDebit.Text = debits.ToString("n");
        }

        ///<summary>Used when switching simple view (0, 1, or 2 journal entries with identical notes).  This function fills in the correct fields in the simple view, and then deletes any journal entries.  2 journal entries will be recreated upon leaving for compound view by CreateTwoEntries.  This is only called once upon going to simple view.  It's not called repeatedly as a way of refreshing the screen.</summary>
        private void FillSimple()
        {
            panelSimple.Visible = true;
            panelCompound.Visible = false;
            if (_listJournalEntries.Count == 0)
            {
                AccountPicked = null;
                textAccount.Text = "";
                butChange.Text = Lan.g(this, "Pick");
                textAmount.Text = "";
                textMemo.Text = "";
            }
            else if (_listJournalEntries.Count == 1)
            {
                double amt = 0;
                //first we assume that the sole entry is for the current account
                if (Accounts.DebitIsPos(AccountOfOrigin.AcctType))
                {//this is used for checking account
                    if (((JournalEntry)_listJournalEntries[0]).DebitAmt > 0)
                    {
                        amt = ((JournalEntry)_listJournalEntries[0]).DebitAmt;
                    }
                    else
                    {//use the credit
                        amt = -((JournalEntry)_listJournalEntries[0]).CreditAmt;
                    }
                }
                else
                {//false for checking acct
                    if (((JournalEntry)_listJournalEntries[0]).DebitAmt > 0)
                    {
                        amt = -((JournalEntry)_listJournalEntries[0]).DebitAmt;
                    }
                    else
                    {
                        amt = ((JournalEntry)_listJournalEntries[0]).CreditAmt;
                    }
                }
                //then, if we assumed wrong, change the sign
                if (((JournalEntry)_listJournalEntries[0]).AccountNum != AccountOfOrigin.AccountNum)
                {
                    amt = -amt;
                }
                textAmount.Text = amt.ToString("n");
                if (((JournalEntry)_listJournalEntries[0]).AccountNum == 0)
                {
                    AccountPicked = null;
                    textAccount.Text = "";
                    butChange.Text = Lan.g(this, "Pick");
                }
                else if (((JournalEntry)_listJournalEntries[0]).AccountNum == AccountOfOrigin.AccountNum)
                {
                    AccountPicked = null;
                    textAccount.Text = "";
                    butChange.Text = Lan.g(this, "Pick");
                }
                else
                {//the sole entry is not for the current account
                    AccountPicked = Accounts.GetAccount(((JournalEntry)_listJournalEntries[0]).AccountNum);
                    textAccount.Text = AccountPicked.Description;
                    butChange.Text = Lan.g(this, "Change");
                }
                textMemo.Text = ((JournalEntry)_listJournalEntries[0]).Memo;
                textCheckNumber.Text = ((JournalEntry)_listJournalEntries[0]).CheckNumber;
            }
            else
            {//count=2
                JournalEntry journalCur;
                JournalEntry journalOther;
                if (((JournalEntry)_listJournalEntries[0]).AccountNum == AccountOfOrigin.AccountNum)
                {
                    //if the first entry is for the account of origin
                    journalCur = (JournalEntry)_listJournalEntries[0];
                    journalOther = (JournalEntry)_listJournalEntries[1];
                }
                else
                {
                    journalCur = (JournalEntry)_listJournalEntries[1];
                    journalOther = (JournalEntry)_listJournalEntries[0];
                }
                if (Accounts.DebitIsPos(AccountOfOrigin.AcctType))
                {//this is used for checking account
                    if (journalCur.DebitAmt > 0)
                    {
                        textAmount.Text = journalCur.DebitAmt.ToString("n");
                    }
                    else
                    {//use the credit
                        textAmount.Text = (-journalCur.CreditAmt).ToString("n");
                    }
                }
                else
                {//false for checking acct
                    if (journalCur.DebitAmt > 0)
                    {
                        textAmount.Text = (-journalCur.DebitAmt).ToString("n");
                    }
                    else
                    {
                        textAmount.Text = journalCur.CreditAmt.ToString("n");
                    }
                }
                if (journalOther.AccountNum == 0)
                {
                    AccountPicked = null;
                    textAccount.Text = "";
                    butChange.Text = Lan.g(this, "Pick");
                }
                else
                {
                    AccountPicked = Accounts.GetAccount(journalOther.AccountNum);
                    textAccount.Text = AccountPicked.Description;
                    butChange.Text = Lan.g(this, "Change");
                }
                textMemo.Text = journalCur.Memo;
                if (journalCur.CheckNumber != "")
                {
                    textCheckNumber.Text = journalCur.CheckNumber;
                }
                if (journalOther.CheckNumber != "")
                {
                    textCheckNumber.Text = journalOther.CheckNumber;
                }
            }
            _listJournalEntries = new List<JournalEntry>();
        }

        private void butChange_Click(object sender, EventArgs e)
        {
            using FormAccountPick FormA = new FormAccountPick();
            FormA.ShowDialog();
            if (FormA.DialogResult != DialogResult.OK)
            {
                return;
            }
            AccountPicked = FormA.SelectedAccount.Clone();
            textAccount.Text = AccountPicked.Description;
            butChange.Text = Lan.g(this, "Change");
        }

        private void checkSimple_Click(object sender, EventArgs e)
        {
            if (checkSimple.Checked)
            {
                if (_listJournalEntries.Count > 2)
                {//do not allow switching to simple if there are more than 2 entries
                    MsgBox.Show(this, "Not allowed to switch to simple view when there are more then two entries.");
                    checkSimple.Checked = false;
                    return;
                }
                if (_listJournalEntries.Count == 2 && ((JournalEntry)_listJournalEntries[0]).Memo != ((JournalEntry)_listJournalEntries[1]).Memo)
                {
                    //warn if notes are different
                    if (!MsgBox.Show(this, MsgBoxButtons.OKCancel, "Note might be lost. Continue?"))
                    {
                        checkSimple.Checked = false;
                        return;
                    }
                }
                FillSimple();
            }
            else
            {//switching from simple to compound view
                CreateTwoEntries();//never fails.  Just does the best it can
                FillCompound();
            }
        }

        private void butAdd_Click(object sender, EventArgs e)
        {
            JournalEntry entry = new JournalEntry();
            entry.TransactionNum = _transaction.TransactionNum;
            if (checkMemoSame.Checked && _listJournalEntries.Count > 0)
            {
                entry.Memo = ((JournalEntry)_listJournalEntries[0]).Memo;
            }
            //date gets set when closing.  Everthing else gets set within following form.
            using FormJournalEntryEdit FormJ = new FormJournalEntryEdit();
            FormJ.IsNew = true;
            FormJ.JournalEntryCur = entry;
            FormJ.ShowDialog();
            if (FormJ.DialogResult == DialogResult.OK)
            {
                _listJournalEntries.Add(FormJ.JournalEntryCur);
                if (checkMemoSame.Checked)
                {
                    for (int i = 0; i < _listJournalEntries.Count; i++)
                    {
                        ((JournalEntry)_listJournalEntries[i]).Memo = FormJ.JournalEntryCur.Memo;
                    }
                }
            }
            FillCompound();
        }

        private void gridMain_CellDoubleClick(object sender, ODGridClickEventArgs e)
        {
            using FormJournalEntryEdit FormJ = new FormJournalEntryEdit();
            FormJ.JournalEntryCur = (JournalEntry)_listJournalEntries[e.Row];
            FormJ.ShowDialog();
            if (FormJ.JournalEntryCur == null)
            {//user deleted
                _listJournalEntries.RemoveAt(e.Row);
            }
            else if (FormJ.DialogResult == DialogResult.OK)
            {
                if (checkMemoSame.Checked)
                {
                    for (int i = 0; i < _listJournalEntries.Count; i++)
                    {
                        ((JournalEntry)_listJournalEntries[i]).Memo = FormJ.JournalEntryCur.Memo;
                    }
                }
            }
            FillCompound();
        }

        private void butExport_Click(object sender, EventArgs e)
        {
            //DateTime reconcileDate=PIn.Date(textReconcileDate.Text);
            //List<Tuple<string,string>> listOtherDetails=new List<Tuple<string,string>>() {
            //	Tuple.Create(labelDateTimeEntered.Text,PIn.DateT(textDateTimeEntered.Text).ToString()),
            //	Tuple.Create(labelUserEntered.Text,PIn.String(textUserEntered.Text)),
            //	Tuple.Create(labelDateTimeEdited.Text,PIn.DateT(textDateTimeEdited.Text).ToString()),
            //	Tuple.Create(labelUserEdited.Text,PIn.String(textUserEdited.Text)),
            //	Tuple.Create(labelDate.Text,PIn.Date(textDate.Text).ToShortDateString()),
            //	Tuple.Create(labelReconcileDate.Text,(reconcileDate==DateTime.MinValue?"":reconcileDate.ToShortDateString()))
            //};
            //GridRow totalsRow=new GridRow("Totals",textDebit.Text,textCredit.Text,"");
            //string msg=
            gridMain.Export(gridMain.Title);//listOtherDetails:listOtherDetails,totalsRow:totalsRow);
                                            //if(!string.IsNullOrEmpty(msg)) {
                                            //	MsgBox.Show(this,msg);
                                            //}
        }

        private void butAttachDep_Click(object sender, EventArgs e)
        {
            if (_transaction.DepositNum == 0)
            {//trying to attach
                using FormDeposits FormD = new FormDeposits();
                FormD.IsSelectionMode = true;
                FormD.ShowDialog();
                if (FormD.DialogResult == DialogResult.Cancel)
                {
                    return;
                }
                _transaction.DepositNum = FormD.DepositSelected.DepositNum;
                textSourceDeposit.Text = FormD.DepositSelected.DateDeposit.ToShortDateString()
                    + "  " + FormD.DepositSelected.Amount.ToString("c");
                butAttachDep.Text = Lan.g(this, "Detach");
            }
            else
            {//trying to detach
                _transaction.DepositNum = 0;
                textSourceDeposit.Text = "";
                butAttachDep.Text = Lan.g(this, "Attach");
            }
        }

        private void butAttachPay_Click(object sender, EventArgs e)
        {
            if (_transaction.PayNum == 0)
            {//trying to attach
             //no way to do this yet.
            }
            else
            {//trying to detach
                _transaction.PayNum = 0;
                textSourcePay.Text = "";
                butAttachPay.Visible = false;
            }
        }

        private void butAttachInvoice_Click(object sender, EventArgs e)
        {
            if (_transaction.TransactionInvoiceNum != 0)
            {
                if (!MsgBox.Show(this, MsgBoxButtons.YesNo, "Detach invoice file?"))
                {
                    return;
                }
                TransactionInvoices.Delete(_transaction.TransactionInvoiceNum);
                _transaction.TransactionInvoiceNum = 0;
                Transactions.UpdateInvoiceNum(_transaction.TransactionNum, _transaction.TransactionInvoiceNum);
                textSourceInvoice.Text = "";
                butAttachInvoice.Text = Lan.g(this, "Attach");
                butOpenInvoice.Enabled = false;
                return;
            }
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string path = openFileDialog.FileName;
            if (!File.Exists(path))
            {
                MsgBox.Show(this, "File does not exist or cannot be read.");
                return;
            }
            TransactionInvoice transactionInvoice = new TransactionInvoice();
            if (PrefC.GetBool(PrefName.AccountingInvoiceAttachmentsSaveInDatabase))
            {
                transactionInvoice.FileName = Path.GetFileName(openFileDialog.FileName);
                byte[] arrayDocumentBytes;
                try
                {
                    arrayDocumentBytes = File.ReadAllBytes(path);
                    transactionInvoice.InvoiceData = Convert.ToBase64String(arrayDocumentBytes);
                    if (transactionInvoice.InvoiceData.Length > 16777215)
                    {
                        MsgBox.Show("Invoice cannot be greater than about 12MB.");
                        return;
                    }
                    TransactionInvoices.Insert(transactionInvoice);
                }
                catch (Exception ex)
                {
                    FriendlyException.Show(ex.Message, ex);
                    return;
                }
            }
            else
            {
                transactionInvoice.FileName = Path.GetFileName(openFileDialog.FileName);
                transactionInvoice.FilePath = path;
                TransactionInvoices.Insert(transactionInvoice);
            }
            _transaction.TransactionInvoiceNum = transactionInvoice.TransactionInvoiceNum;
            Transactions.UpdateInvoiceNum(_transaction.TransactionNum, transactionInvoice.TransactionInvoiceNum);
            butAttachInvoice.Text = Lan.g(this, "Detach");
            textSourceInvoice.Text = transactionInvoice.FileName;
            butOpenInvoice.Enabled = true;
        }

        private void butOpenInvoice_Click(object sender, EventArgs e)
        {
            TransactionInvoice transactionInvoice = TransactionInvoices.GetOne(_transaction.TransactionInvoiceNum);
            if (String.IsNullOrEmpty(transactionInvoice.FilePath))
            {//FilePath is empty when Invoice contains base64 data.
                try
                {
                    string fileExt = Path.GetExtension(transactionInvoice.FileName);
                    string prefix = transactionInvoice.FileName.Substring(0, transactionInvoice.FileName.Length - fileExt.Length);
                    string filePath = ODFileUtils.CreateRandomFile(PrefC.GetTempFolderPath(), fileExt, prefix);
                    byte[] arrayDocumentBytes = Convert.FromBase64String(transactionInvoice.InvoiceData);
                    ODFileUtils.WriteAllBytesThenStart(filePath, arrayDocumentBytes, null);
                }
                catch (Exception ex)
                {
                    FriendlyException.Show(ex.Message, ex);
                    return;
                }
            }
            else
            {
                try
                {
                    ODFileUtils.ProcessStart(transactionInvoice.FilePath);
                }
                catch
                {
                    MsgBox.Show(transactionInvoice.FilePath + " cannot be found.");//Previously attached invoice was likely deleted, moved, or renamed.
                }
            }
        }

        /*//<summary>If the journalList is 0 or 1 in length, then this function creates either 1 or 2 entries so that the simple view can display properly.</summary>
                private void CreateTwoEntries() {
                    JournalEntry entry;
                    if(JournalList.Count>=2) {
                        return;
                    }
                    if(JournalList.Count==0) {
                        //first, for account of origin
                        entry=new JournalEntry();
                        entry.TransactionNum=TransCur.TransactionNum;
                        if(textDate.Text=="" || textDate.errorProvider1.GetError(textDate)!="") {
                            entry.DateDisplayed=DateTime.Today;
                        }
                        else {
                            entry.DateDisplayed=PIn.PDate(textDate.Text);
                        }
                        entry.AccountNum=AccountOfOrigin.AccountNum;
                        JournalList.Add(entry);
                        //then, for the other
                        entry=new JournalEntry();
                        entry.TransactionNum=TransCur.TransactionNum;
                        if(textDate.Text=="" || textDate.errorProvider1.GetError(textDate)!="") {
                            entry.DateDisplayed=DateTime.Today;
                        }
                        else {
                            entry.DateDisplayed=PIn.PDate(textDate.Text);
                        }
                        entry.AccountNum=0;
                        JournalList.Add(entry);
                        return;
                    }
                    //otherwise, count=1
                    entry=new JournalEntry();
                    entry.TransactionNum=TransCur.TransactionNum;
                    entry.DateDisplayed=((JournalEntry)JournalList[0]).DateDisplayed;
                    if(((JournalEntry)JournalList[0]).AccountNum==AccountOfOrigin.AccountNum) {
                        entry.AccountNum=0;
                    }
                    else {
                        entry.AccountNum=AccountOfOrigin.AccountNum;
                    }
                    entry.DebitAmt=((JournalEntry)JournalList[0]).CreditAmt;
                    entry.CreditAmt=((JournalEntry)JournalList[0]).DebitAmt;
                    JournalList.Add(entry);
                }*/


        ///<summary>When leaving simple view, this function takes the info from the screen and creates two journal entries.  Never fails.  One of the journal entries might accountNum=0, and they might both have amounts of 0.  This is all in preparation for either saving or for viewing as compound.</summary>
        private void CreateTwoEntries()
        {
            JournalEntry entry;
            //first, for account of origin
            entry = new JournalEntry();
            entry.TransactionNum = _transaction.TransactionNum;
            if (textDate.Text == "" || !textDate.IsValid())
            {
                entry.DateDisplayed = DateTime.Today;
            }
            else
            {
                entry.DateDisplayed = PIn.Date(textDate.Text);
            }
            entry.AccountNum = AccountOfOrigin.AccountNum;
            double amt = 0;
            if (textAmount.IsValid())
            {//if no error
                amt = PIn.Double(textAmount.Text);
            }
            //if amt==0, then both credit and debit remain 0
            if (amt > 0)
            {
                if (Accounts.DebitIsPos(AccountOfOrigin.AcctType))
                {//used for checking
                    entry.DebitAmt = amt;
                }
                else
                {
                    entry.CreditAmt = amt;
                }
            }
            else if (amt < 0)
            {
                if (Accounts.DebitIsPos(AccountOfOrigin.AcctType))
                {//used for checking
                    entry.CreditAmt = -amt;
                }
                else
                {
                    entry.DebitAmt = -amt;
                }
            }
            if (!IsNew)
            {
                entry.SecUserNumEntry = _listJournalEntriesOld[0].SecUserNumEntry;
                entry.SecDateTEntry = _listJournalEntriesOld[0].SecDateTEntry;
                entry.JournalEntryNum = _listJournalEntriesOld[0].JournalEntryNum;
            }
            entry.Memo = textMemo.Text;
            entry.CheckNumber = textCheckNumber.Text;
            _listJournalEntries.Add(entry);
            //then, for the other
            entry = new JournalEntry();
            entry.TransactionNum = _transaction.TransactionNum;
            entry.DateDisplayed = ((JournalEntry)_listJournalEntries[0]).DateDisplayed;
            entry.DebitAmt = ((JournalEntry)_listJournalEntries[0]).CreditAmt;
            entry.CreditAmt = ((JournalEntry)_listJournalEntries[0]).DebitAmt;
            if (AccountPicked == null)
            {
                entry.AccountNum = 0;
            }
            else
            {
                entry.AccountNum = AccountPicked.AccountNum;
            }
            if (!IsNew && _listJournalEntriesOld.Count > 1)
            {
                entry.SecUserNumEntry = _listJournalEntriesOld[1].SecUserNumEntry;
                entry.SecDateTEntry = _listJournalEntriesOld[1].SecDateTEntry;
                entry.JournalEntryNum = _listJournalEntriesOld[1].JournalEntryNum;
            }
            entry.Memo = textMemo.Text;
            entry.CheckNumber = textCheckNumber.Text;
            _listJournalEntries.Add(entry);
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            if (!MsgBox.Show(this, MsgBoxButtons.OKCancel, "Delete this entire transaction?"))
            {
                return;
            }
            string securityentry = "";
            if (!IsNew)
            {
                //we need this data before it's gone
                _listJournalEntries = JournalEntries.GetForTrans(_transaction.TransactionNum);//because they were cleared when laying out
                securityentry = Lan.g(this, "Deleted: ")
                    + ((JournalEntry)_listJournalEntries[0]).DateDisplayed.ToShortDateString() + " ";
                double tot = 0;
                for (int i = 0; i < _listJournalEntries.Count; i++)
                {
                    tot += ((JournalEntry)_listJournalEntries[i]).DebitAmt;
                    if (i > 0)
                    {
                        securityentry += ", ";
                    }
                    securityentry += Accounts.GetDescript(((JournalEntry)_listJournalEntries[i]).AccountNum);
                }
                securityentry += ". " + tot.ToString("c");
                _listJournalEntries = new List<JournalEntry>();//in case it fails, we don't want to leave this list around.
            }
            try
            {
                Transactions.Delete(_transaction);
            }
            catch (ApplicationException ex)
            {
                _listJournalEntries = JournalEntries.GetForTrans(_transaction.TransactionNum);//Refreshes list so that the journal entries are not deleted by the update later.
                MessageBox.Show(ex.Message);
                return;
            }
            if (!IsNew)
            {
                SecurityLogs.MakeLogEntry(Permissions.AccountingEdit, 0, securityentry);
            }
            DialogResult = DialogResult.OK;
        }

        private void butOK_Click(object sender, System.EventArgs e)
        {
            if (!textDate.IsValid())
            {
                MsgBox.Show(this, "Please fix data entry errors first.");
                return;
            }
            DateTime date = PIn.Date(textDate.Text);
            //Prevent backdating----------------------------------------------------------------------------------------
            if (IsNew)
            {
                if (!Security.IsAuthorized(Permissions.AccountingCreate, date))
                {
                    return;
                }
            }
            else
            {
                //We enforce security here based on date displayed, not date entered
                if (!Security.IsAuthorized(Permissions.AccountingEdit, date))
                {
                    return;
                }
            }
            if (checkSimple.Checked)
            {//simple view
             //even though CreateTwoEntries could handle any errors, it makes more sense to catch errors before calling it
                if (!textAmount.IsValid())
                {
                    MsgBox.Show(this, "Please fix data entry errors first.");
                    return;
                }
                if (AccountPicked == null)
                {
                    MsgBox.Show(this, "Please select an account first.");
                    return;
                }
                CreateTwoEntries();
            }
            else
            {//compound view
                if (textCredit.Text != textDebit.Text)
                {
                    MsgBox.Show(this, "Debits and Credits must be equal.");
                    return;
                }
                for (int i = 0; i < _listJournalEntries.Count; i++)
                {
                    if (((JournalEntry)_listJournalEntries[i]).AccountNum == 0)
                    {
                        MsgBox.Show(this, "Accounts must be selected for each entry first.");
                        return;
                    }
                }
            }
            string splits = "";
            for (int i = 0; i < _listJournalEntries.Count; i++)
            {
                ((JournalEntry)_listJournalEntries[i]).DateDisplayed = date;
                splits = "";
                for (int j = 0; j < _listJournalEntries.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    if (splits != "")
                    {
                        splits += "\r\n";
                    }
                    splits += Accounts.GetDescript(((JournalEntry)_listJournalEntries[j]).AccountNum);
                    if (_listJournalEntries.Count < 3)
                    {
                        continue;//don't show the amount if there is only two splits, because the amount is the same.
                    }
                    if (((JournalEntry)_listJournalEntries[j]).CreditAmt > 0)
                    {
                        splits += "  " + ((JournalEntry)_listJournalEntries[j]).CreditAmt.ToString("n");
                    }
                    else if (((JournalEntry)_listJournalEntries[j]).DebitAmt > 0)
                    {
                        splits += "  " + ((JournalEntry)_listJournalEntries[j]).DebitAmt.ToString("n");
                    }
                }
                ((JournalEntry)_listJournalEntries[i]).Splits = splits;
            }
            //try{
            JournalEntries.UpdateList(_listJournalEntriesOld, _listJournalEntries);
            //}
            //catch{

            //}
            DateTime datePrevious = _transaction.SecDateTEdit;
            Transactions.Update(_transaction);//this catches DepositNum, the only user-editable field.
            double tot = 0;
            for (int i = 0; i < _listJournalEntries.Count; i++)
            {
                tot += ((JournalEntry)_listJournalEntries[i]).DebitAmt;
            }
            if (IsNew)
            {
                SecurityLogs.MakeLogEntry(Permissions.AccountingCreate, 0,
                    date.ToShortDateString() + " " + AccountOfOrigin.Description + " " + tot.ToString("c"), _transaction.TransactionNum, DateTime.MinValue);
            }
            else
            {
                string txt = date.ToShortDateString();
                if (AccountOfOrigin != null)
                {
                    txt += " " + AccountOfOrigin.Description;
                }
                txt += " " + tot.ToString("c");
                SecurityLogs.MakeLogEntry(Permissions.AccountingEdit, 0, txt, _transaction.TransactionNum, datePrevious);
            }
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

















    }
}





















