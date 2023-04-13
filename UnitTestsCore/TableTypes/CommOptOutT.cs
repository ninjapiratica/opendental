﻿using OpenDentBusiness;
using System.Linq;

namespace UnitTestsCore
{
    public class CommOptOutT
    {

        public static void Create(Patient pat, CommOptOutType optOutType, params CommOptOutMode[] arrOptOutModes)
        {
            CommOptOuts.Upsert(new CommOptOut
            {
                PatNum = pat.PatNum,
                OptOutSms = arrOptOutModes.Any(x => x == CommOptOutMode.Text) ? optOutType : 0,
                OptOutEmail = arrOptOutModes.Any(x => x == CommOptOutMode.Email) ? optOutType : 0,
            });
        }
    }
}
