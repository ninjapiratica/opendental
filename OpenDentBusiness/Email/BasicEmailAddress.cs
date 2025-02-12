﻿namespace OpenDentBusiness.Email
{

    ///<summary>Represents an email address at its rawest form. Stays away from associating with Open Dental.</summary>
    public class BasicEmailAddress
    {

        ///<summary>The SMTP server for the email. For example, smtp.gmail.com</summary>
        public string SMTPserver;
        ///<summary>Username.</summary>
        public string EmailUsername;
        ///<summary>Password associated with this email address. Not encrypted.</summary>
        public string EmailPassword;
        ///<summary>Usually 587, sometimes 25 or 465.</summary>
        public int ServerPort;
        ///<summary>If SSL should be used.</summary>
        public bool UseSSL;
        ///<summary>OAuth token used for account authorization.</summary>
        public string AccessToken;
        ///<summary>OAuth token used to refresh the AccessToken.</summary>
        public string RefreshToken;
        ///<summary>OAuth type used. 0-None, 1-Google, 2-Microsoft</summary>
        public BasicOAuthType AuthenticationType;
    }

    public enum BasicOAuthType
    {
        ///<summary>0 - Not using OAuth</summary>
        None,
        ///<summary>1 - Using OAuth for Google</summary>
        Google,
        ///<summary>2 - Using OAuth for Microsoft</summary>
        Microsoft,
    }
}
