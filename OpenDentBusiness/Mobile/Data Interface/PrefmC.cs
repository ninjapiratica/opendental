﻿using CodeBase;
using System;
using System.Collections.Generic;

namespace OpenDentBusiness.Mobile
{
    public class PrefmC
    {
        public Dictionary<string, Prefm> Dict = new Dictionary<string, Prefm>();// cannot have a static variable here because we want something unique for each patient.
        ///<summary>Gets a pref of type string.</summary>
        public static string GetString(PrefmName prefmName)
        {
            try
            {
                PrefmC prefmC = Prefms.LoadPreferences();
                if (!prefmC.Dict.ContainsKey(prefmName.ToString()))
                {
                    throw new Exception(prefmName + " is an invalid pref name.");
                }
                return prefmC.Dict[prefmName.ToString()].ValueString;
            }
            catch (Exception ex)
            {
                ex.DoNothing();
                return "";
            }
        }








    }
}



