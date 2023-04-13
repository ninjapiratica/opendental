using OpenDentBusiness.WebTypes.AutoComm;
using System;

namespace OpenDentBusiness
{
    ///<summary></summary>
    [Serializable]
    [CrudTable(HasBatchWriteMethods = true)]
    public class PatientPortalInvite : AutoCommAppt
    {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey = true)]
        public long PatientPortalInviteNum;
    }
}

