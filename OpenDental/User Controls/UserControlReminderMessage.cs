﻿using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class UserControlReminderMessage : UserControl
    {
        public LayoutManagerForms LayoutManager;
        private string _templateEmail;
        private bool _isLoading;

        public UserControlReminderMessage(ApptReminderRule apptReminder, LayoutManagerForms layoutManager)
        {
            InitializeComponent();
            Rule = apptReminder;
            LayoutManager = layoutManager;
            LoadControl();
        }

        public UserControlReminderMessage()
        {//just so the designer can load.
            InitializeComponent();
        }

        public ApptReminderRule Rule { get; }

        private void butEditEmail_Click(object sender, EventArgs e)
        {
            using FormEmailEdit formEE = new FormEmailEdit();
            formEE.MarkupText = _templateEmail;
            formEE.IsRawAllowed = true;
            formEE.IsRaw = Rule.EmailTemplateType == EmailType.RawHtml;
            formEE.DoCheckForDisclaimer = true;
            formEE.ShowDialog();
            if (formEE.DialogResult != DialogResult.OK)
            {
                return;
            }
            Rule.EmailTemplateType = formEE.IsRaw ? EmailType.RawHtml : EmailType.Html;
            _templateEmail = formEE.MarkupText;
            RefreshEmail();
        }

        private void browserEmailBody_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (_isLoading)
            {
                return;
            }
            e.Cancel = true;//Cancel browser navigation (for links clicked within the email message).
            if (e.Url.AbsoluteUri == "about:blank")
            {
                return;
            }
            //if user did not specify a valid url beginning with http:// then the event args would make the url start with "about:" 
            //ex: about:www.google.com and then would ask the user to get a separate app to open the link since it is unrecognized
            string url = e.Url.ToString();
            if (url.StartsWith("about"))
            {
                url = url.Replace("about:", "http://");
            }
            Process.Start(url);//Instead launch the URL into a new default browser window.
        }

        private void browserEmailBody_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            _isLoading = false;
        }

        private void LoadControl()
        {
            textTemplateSms.Text = Rule.TemplateSMS;
            textTemplateSubject.Text = Rule.TemplateEmailSubject;
            _templateEmail = Rule.TemplateEmail;
            if (Rule.TypeCur == ApptReminderType.PatientPortalInvite)
            {
                textTemplateSms.Enabled = false;
            }
            if (Rule.TypeCur == ApptReminderType.Arrival)
            {
                butEditEmail.Visible = false;
                labelEmail.Visible = false;
                textTemplateSubject.Visible = false;
                browserEmailBody.Visible = false;
            }
            RefreshEmail();
        }

        private void RefreshEmail()
        {
            _isLoading = true;
            //There's no easy way to scale the text in the web browser preview to match zoom.  In other places, like wiki, we have altered font size within document.
            if (Rule.EmailTemplateType == EmailType.RawHtml)
            {
                //browserEmailBody.Document.Body.Style = "font-size:"+LayoutManager.ScaleF(12).ToString("f1")+";";
                browserEmailBody.DocumentText = _templateEmail;
                return;//text is already in HTML, it does not need to be translated. 
            }
            try
            {
                string text = MarkupEdit.TranslateToXhtml(_templateEmail, isPreviewOnly: true, hasWikiPageTitles: false, isEmail: true, scale: LayoutManager.ScaleMy());
                //browserEmailBody.Document.Body.Style = "font-size:"+LayoutManager.ScaleF(12).ToString("f1")+";";
                browserEmailBody.DocumentText = text;
            }
            catch (Exception ex)
            {
                ex.DoNothing();
            }
        }

        public List<string> ValidateTemplates()
        {
            List<string> listErrors = new List<string>();
            if (Rule.TypeCur == ApptReminderType.Arrival)
            {
                if (!textTemplateSms.Text.ToLower().Contains(OpenDentBusiness.AutoComm.ArrivalsTagReplacer.ARRIVED_TAG.ToLower()))
                {
                    listErrors.Add(Lan.g(this, $"Arrival texts must contain the \"{OpenDentBusiness.AutoComm.ArrivalsTagReplacer.ARRIVED_TAG}\" tag."));
                }
            }
            if (!(Rule.TypeCur.In(ApptReminderType.PatientPortalInvite, ApptReminderType.Birthday)))
            {
                if (string.IsNullOrWhiteSpace(textTemplateSms.Text))
                {
                    listErrors.Add(Lan.g(this, "Text message cannot be blank."));
                }
            }
            else
            {
                if (Rule.TypeCur != ApptReminderType.PatientPortalInvite)
                {
                    if (string.IsNullOrWhiteSpace(textTemplateSms.Text))
                    {
                        listErrors.Add(Lan.g(this, "Text message cannot be blank."));
                    }
                }
                if (string.IsNullOrWhiteSpace(textTemplateSubject.Text))
                {
                    listErrors.Add(Lan.g(this, "Email subject cannot be blank."));
                }
                if (string.IsNullOrWhiteSpace(_templateEmail))
                {
                    listErrors.Add(Lan.g(this, "Email message cannot be blank."));
                }
                if (PrefC.GetBool(PrefName.EmailDisclaimerIsOn) && !_templateEmail.ToLower().Contains("[emaildisclaimer]"))
                {
                    listErrors.Add(Lan.g(this, "Email must contain the \"[EmailDisclaimer]\" tag."));
                }
            }
            if (Rule.TypeCur == ApptReminderType.ConfirmationFutureDay)
            {
                if (!textTemplateSms.Text.ToLower().Contains("[confirmcode]"))
                {
                    listErrors.Add(Lan.g(this, "Confirmation texts must contain the \"[ConfirmCode]\" tag."));
                }
                if (_templateEmail.ToLower().Contains("[confirmcode]"))
                {
                    listErrors.Add(Lan.g(this, "Confirmation emails should not contain the \"[ConfirmCode]\" tag."));
                }
                if (!_templateEmail.ToLower().Contains("[confirmurl]"))
                {
                    listErrors.Add(Lan.g(this, "Confirmation emails must contain the \"[ConfirmURL]\" tag."));
                }
            }
            listErrors.AddRange(AddCalendarTagErrors());
            //new (non-birthday) rule, user may have not remembered to setup the aggregates
            if (Rule.Language != "" && Rule.ApptReminderRuleNum == 0 && Rule.TypeCur != ApptReminderType.Birthday)
            {
                string err = Lan.g(this, "Aggregated templates have not been set up for all additional languages. Click on the 'Advanced' button to set up " +
                    "aggregated templates.");
                if (Rule.TypeCur == ApptReminderType.Arrival)
                {
                    if (Rule.TemplateSMSAggShared == "" || Rule.TemplateSMSAggPerAppt == "")
                    { //Arrivals don't include email
                        listErrors.Add(err);
                    }
                }
                else if (Rule.TypeCur == ApptReminderType.PatientPortalInvite)
                {
                    if (Rule.TemplateEmailAggShared == "" || Rule.TemplateEmailAggPerAppt == "")
                    { //Patient Portal Invites don't include sms
                        listErrors.Add(err);
                    }
                }
                else
                {
                    if (Rule.TemplateSMSAggShared == "" || Rule.TemplateSMSAggPerAppt == "" || Rule.TemplateEmailAggShared == "" || Rule.TemplateEmailAggPerAppt == "")
                    {
                        listErrors.Add(err);
                    }
                }
            }
            return listErrors;
        }

        ///<summary>Validates the AddToCalendar tag. Adds to the error list if the AddToCalendar tag is present but not signed up for eConfirmations</summary>
        public List<string> AddCalendarTagErrors()
        {
            List<string> listErrors = new List<string>();
            //Only these rule types will have [AddToCalendar] tags.
            //See UserControlReminderAgg.AddCalendarTagErrors() for auto reply validation.
            if (!Rule.TypeCur.In(ApptReminderType.ScheduleThankYou, ApptReminderType.Reminder))
            {
                return listErrors;
            }
            //[AddToCalendar] tags are allowed tag when eConfirmations are enabled so don't bother validating.
            if (PrefC.GetBool(PrefName.ApptConfirmAutoSignedUp))
            {
                return listErrors;
            }
            string addToCalTag = ApptThankYouSents.ADD_TO_CALENDAR.ToLower();
            string errorText(string mode, string reminderType)
            {
                return Lan.g(this, $"Automated {reminderType} {mode} cannot contain ") + ApptThankYouSents.ADD_TO_CALENDAR
                    + Lan.g(this, " when not signed up for eConfirmations.");
            }
            string reminderType = "";
            switch (Rule.TypeCur)
            {
                case ApptReminderType.ScheduleThankYou:
                    reminderType = "Thank-You";
                    break;
                case ApptReminderType.Reminder:
                    reminderType = "Reminder";
                    break;
            }
            if (textTemplateSms.Text.ToLower().Contains(addToCalTag))
            {
                listErrors.Add(errorText("texts", reminderType));
            }
            if (_templateEmail.ToLower().Contains(addToCalTag))
            {
                listErrors.Add(errorText("emails", reminderType));
            }
            return listErrors;
        }

        public void SaveControlTemplates()
        {
            if (Rule.TypeCur == ApptReminderType.Birthday)
            {
                Rule.TemplateSMS = "";
            }
            else
            {
                Rule.TemplateSMS = textTemplateSms.Text.Replace("[ConfirmURL].", "[ConfirmURL] .");//Clicking a link with a period will not get recognized. 
            }
            Rule.TemplateEmailSubject = textTemplateSubject.Text;
            Rule.TemplateEmail = _templateEmail;
        }

    }
}
