﻿using CodeBase;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness
{
    public class PeerInfos
    {

        ///<summary>Returns a list of every peer that is in a session. Includes both hosts and clients (non-techs).</summary>
        public static List<PeerInfo> GetActiveSessions(bool useConnectionStore)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<PeerInfo>>(MethodBase.GetCurrentMethod(), useConnectionStore);
            }
            string command = "SELECT UserName,DateTimeJoined FROM peerinfo WHERE SessionInfoNum > 0";
            List<PeerInfo> listPeerInfos = DataAction.GetRemoteSupport(() => Db.GetTable(command), useConnectionStore)
                .Select()
                .Select(x => new PeerInfo()
                {
                    UserName = PIn.String(x["UserName"].ToString()),
                    DateTimeJoined = PIn.DateT(x["DateTimeJoined"].ToString())
                })
                .ToList();
            return listPeerInfos;
        }

        ///<summary>Returns a list of every peer that is in a session. Includes both hosts and clients (non-techs).</summary>
        public static List<PeerInfo> GetActiveSessions(bool useConnectionStore, bool setEmployeeNum)
        {
            //No need to check MiddleTierRole; no call to db.
            List<PeerInfo> listPeerInfos = new List<PeerInfo>();
            ODException.SwallowAnyException(() => listPeerInfos = GetActiveSessions(useConnectionStore));
            if (setEmployeeNum)
            {
                SetEmployeeNum(ref listPeerInfos);
            }
            return listPeerInfos;
        }

        ///<summary>Returns the active session that the employee is in or returns null.</summary>
        public static PeerInfo GetActiveSessionForEmployee(long employeeNum)
        {
            return GetActiveSessions(false, true).FirstOrDefault(x => x.EmployeeNum == employeeNum);
        }

        ///<summary>Sets the non-database EmployeeNum field for all peers passed in that have a corresponding employee entity.</summary>
        public static void SetEmployeeNum(ref List<PeerInfo> listPeerInfos)
        {
            if (listPeerInfos.IsNullOrEmpty())
            {
                return;
            }
            //Set the EmployeeNum field for every peer that represents an employee. Employees use work email addresses as user names.
            for (int i = 0; i < listPeerInfos.Count; i++)
            {
                string userName = listPeerInfos[i].UserName.Trim().ToLower();
                Employee employee = Employees.GetFirstOrDefault(x => x.EmailWork.Trim().ToLower() == userName);
                if (employee != null)
                {
                    listPeerInfos[i].EmployeeNum = employee.EmployeeNum;
                }
            }
        }

    }
}
