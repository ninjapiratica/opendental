﻿using System.Reflection;

namespace OpenDentBusiness
{
    public class WebChatPrefs
    {

        public static string GetString(WebChatPrefName prefName)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetString(MethodBase.GetCurrentMethod(), prefName);
            }
            WebChatPref pref = null;
            WebChatMisc.DbAction(delegate ()
            {
                string command = "SELECT * FROM webchatpref WHERE PrefName='" + POut.String(prefName.ToString()) + "'";
                pref = Crud.WebChatPrefCrud.SelectOne(command);
            });
            if (pref == null)
            {
                return "";
            }
            return pref.ValueString;
        }

    }
}