﻿using CodeBase;
using System;
using WebServiceSerializer;

namespace OpenDentBusiness.WebTypes.WebForms
{
    public class WebUtils
    {

        /// <summary></summary>
        /// <param name="regKey"></param>
        /// <returns></returns>
        public static long GetDentalOfficeID(string regKey = null)
        {
            if (string.IsNullOrEmpty(regKey))
            {
                regKey = PrefC.GetString(PrefName.RegistrationKey);
            }
            try
            {
                string payload = PayloadHelper.CreatePayloadWebHostSynch(regKey, new PayloadItem(regKey, "RegKey"));
                return WebSerializer.DeserializeTag<long>(SheetsSynchProxy.GetWebServiceInstance().GetDentalOfficeID(payload), "Success");
            }
            catch (Exception ex)
            {
                ex.DoNothing();
            }
            return 0;
        }

        public static long GetRegKeyID(string regKey = null)
        {
            if (string.IsNullOrEmpty(regKey))
            {
                regKey = PrefC.GetString(PrefName.RegistrationKey);
            }
            try
            {
                string payload = PayloadHelper.CreatePayloadWebHostSynch(regKey, new PayloadItem(regKey, "RegKey"));
                return WebSerializer.DeserializeTag<long>(SheetsSynchProxy.GetWebServiceInstance().GetRegistrationKeyID(payload), "Success");
            }
            catch (Exception ex)
            {
                ex.DoNothing();
            }
            return 0;
        }

        /// <summary></summary>
        /// <param name="regKey"></param>
        /// <returns></returns>
        public static string GetSheetDefAddress(string regKey = null)
        {
            if (string.IsNullOrEmpty(regKey))
            {
                regKey = PrefC.GetString(PrefName.RegistrationKey);
            }
            try
            {
                string payload = PayloadHelper.CreatePayloadWebHostSynch(regKey, new PayloadItem(regKey, "RegKey"));
                return WebSerializer.DeserializeTag<string>(SheetsSynchProxy.GetWebServiceInstance().GetSheetDefAddress(payload), "Success");
            }
            catch (Exception ex)
            {
                ex.DoNothing();
            }
            return "";
        }
    }
}
