using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class ConnectionGroups
    {
        ///<summary></summary>
        public static List<ConnectionGroup> GetAll()
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<ConnectionGroup>>(MethodBase.GetCurrentMethod());
            }
            string command = "SELECT * FROM connectiongroup ORDER BY Description";
            return Crud.ConnectionGroupCrud.SelectMany(command);
        }

        ///<summary></summary>
        public static long Insert(ConnectionGroup connectionGroup)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                connectionGroup.ConnectionGroupNum = Meth.GetLong(MethodBase.GetCurrentMethod(), connectionGroup);
                return connectionGroup.ConnectionGroupNum;
            }
            return Crud.ConnectionGroupCrud.Insert(connectionGroup);
        }

        ///<summary></summary>
        public static void Update(ConnectionGroup connectionGroup)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), connectionGroup);
                return;
            }
            Crud.ConnectionGroupCrud.Update(connectionGroup);
        }

        ///<summary></summary>
        public static void Delete(long connectionGroupNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), connectionGroupNum);
                return;
            }
            string command = "DELETE FROM connectiongroup WHERE ConnectionGroupNum = " + POut.Long(connectionGroupNum);
            Db.NonQ(command);
        }
    }
}