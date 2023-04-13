using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;

namespace UnitTestsCore
{
    public class SmsFromMobileT
    {

        public static SmsFromMobile CreateSmsFromMobile(long patNum, long clinicNum, string patPhone, string provPhone, DateTime dateTimeReceived,
            string msgText = "", string guidMessage = "")
        {
            if (string.IsNullOrEmpty(guidMessage))
            {
                guidMessage = Guid.NewGuid().ToString();
            }
            SmsFromMobile smsFromMobile = new SmsFromMobile()
            {
                GuidMessage = guidMessage,
                PatNum = patNum,
                MobilePhoneNumber = patPhone,
                SmsPhoneNumber = provPhone,
                ClinicNum = clinicNum,
                DateTimeReceived = dateTimeReceived,
                MsgText = string.IsNullOrEmpty(msgText) ? MiscUtils.CreateRandomAlphaNumericString(10) : msgText,
            };
            smsFromMobile.SmsFromMobileNum = SmsFromMobiles.Insert(smsFromMobile);
            return smsFromMobile;
        }

        public static List<SmsFromMobile> GetAll()
        {
            string command = "SELECT * FROM smsfrommobile";
            return OpenDentBusiness.Crud.SmsFromMobileCrud.SelectMany(command);
        }
    }
}
