﻿using System.Data;

namespace OpenDentBusiness
{
    public class Reports
    {
        ///<summary>Gets a table of data using low permissions.</summary>
        public static DataTable GetTable(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetTableLow(command);
            }
            return Db.GetTableLow(command);
        }
    }
}
