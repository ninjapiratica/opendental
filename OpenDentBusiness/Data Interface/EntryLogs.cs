using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class EntryLogs
    {
        #region Get Methods

        ///<summary>Gets one EntryLog from the db.</summary>
        public static EntryLog GetOne(long entryLogNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<EntryLog>(MethodBase.GetCurrentMethod(), entryLogNum);
            }
            return Crud.EntryLogCrud.SelectOne(entryLogNum);
        }
        #endregion

        #region Insert
        ///<summary></summary>
        public static long Insert(EntryLog entryLog)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                entryLog.EntryLogNum = Meth.GetLong(MethodBase.GetCurrentMethod(), entryLog);
                return entryLog.EntryLogNum;
            }
            return Crud.EntryLogCrud.Insert(entryLog);
        }

        public static void InsertMany(List<EntryLog> listEntry)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), listEntry);
                return;
            }
            Crud.EntryLogCrud.InsertMany(listEntry);
        }
        #endregion
    }
}