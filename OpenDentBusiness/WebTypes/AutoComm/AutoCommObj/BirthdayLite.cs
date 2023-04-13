﻿using System;
using System.Collections.Generic;

namespace OpenDentBusiness.AutoComm
{
    public class BirthdayLite : AutoCommObj
    {
        /// <summary>The patient's birthdate. </summary>
        public DateTime Birthdate;

        public override void SetPatientContact(PatComm patComm, Dictionary<long, PatComm> dictPatComms)
        {
            base.SetPatientContact(patComm, dictPatComms);
            Birthdate = patComm?.Birthdate ?? DateTime.MinValue;
        }
    }
}
