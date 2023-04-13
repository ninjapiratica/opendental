﻿using OpenDentBusiness.WebTypes.AutoComm;
using System;

namespace OpenDentBusiness
{
    ///<summary>When a reminder is sent for an appointment a record of that send is stored here. This is used to prevent re-sends of the same reminder.</summary>
    [Serializable, CrudTable(HasBatchWriteMethods = true)]
    public class PromotionLog : AutoCommSent
    {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey = true)]
        public long PromotionLogNum;
        ///<summary>FK to promotion.PromotionNum</summary>
        public long PromotionNum;
        ///<summary>A foreign key from the email hosting API that allows us to receive status updates on this specific email.</summary>
        public long EmailHostingFK;
        ///<summary>Enum:PromotionLogStatus </summary>
        public PromotionLogStatus PromotionStatus;

        #region Obsolete
        ///<summary>Deprecated. Included for backward compatibility (even though we were never passing PromotionLogs via DTO). FK to emailmessage.EmailMessageNum
        ///The email that was sent for this promotion.</summary>
        [CrudColumn(IsNotDbColumn = true)]
        public long EmailMessageNum;
        #endregion

        ///<summary></summary>
        public PromotionLog Clone()
        {
            return (PromotionLog)MemberwiseClone();
        }
    }

    ///<summary>Preserve order. If one needs to be added here, consider the verisonless webhook at HQ.</summary>
    public enum PromotionLogStatus
    {
        ///<summary>0 - Unknown</summary>
        Unknown,
        ///<summary>1 - Promotion has not been sent.</summary>
        Pending,
        ///<summary>2 - Email has bounced because email does not exist.</summary>
        Bounced,
        ///<summary>3 - User has unsubscribed in the passed and this was rejected.</summary>
        Unsubscribed,
        ///<summary>4 - This email was sent and then marked as spam by the user.</summary>
        Complaint,
        ///<summary>5 - The email sent and delivered successfully.</summary>
        Delivered,
        ///<summary>6 - The email failed to send for a different reason than any of the reasons above.</summary>
        Failed,
        ///<summary>7 - The email was opened by the user.</summary>
        Opened,
    }
}
