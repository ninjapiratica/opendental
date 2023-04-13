using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class AlertSubs
    {
        public static void DeleteAndInsertForSuperUsers(List<Userod> listUsers, List<AlertSub> listAlertSubs)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), listUsers, listAlertSubs);
                return;
            }
            if (listUsers == null || listUsers.Count < 1)
            {
                return;
            }
            string command = "DELETE FROM alertsub WHERE UserNum IN(" + string.Join(",", listUsers.Select(x => x.UserNum).ToList()) + ")";
            Db.NonQ(command);
            foreach (AlertSub alertSub in listAlertSubs)
            {
                command = "INSERT INTO alertsub (UserNum,ClinicNum,Type) VALUES(" + alertSub.UserNum.ToString() + "," + alertSub.ClinicNum.ToString() + "," + ((int)alertSub.Type).ToString() + ")";
                Db.NonQ(command);
            }
        }

        #region Get Methods
        public static List<AlertSub> GetAll()
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<AlertSub>>(MethodBase.GetCurrentMethod());
            }
            string command = "SELECT * FROM alertsub";
            return Crud.AlertSubCrud.SelectMany(command);
        }

        ///<summary>Returns list of all AlertSubs for given userNum. Can also specify a clinicNum as well.</summary>
        public static List<AlertSub> GetAllForUser(long userNum, long clinicNum = -1)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<AlertSub>>(MethodBase.GetCurrentMethod(), userNum, clinicNum);
            }
            string command = "SELECT * FROM alertsub WHERE UserNum=" + POut.Long(userNum);
            if (clinicNum != -1)
            {
                command += " AND ClinicNum=" + POut.Long(clinicNum);
            }
            return Crud.AlertSubCrud.SelectMany(command);
        }
        #endregion

        ///<summary>Gets one AlertSub from the db.</summary>
        public static AlertSub GetOne(long alertSubNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<AlertSub>(MethodBase.GetCurrentMethod(), alertSubNum);
            }
            return Crud.AlertSubCrud.SelectOne(alertSubNum);
        }

        ///<summary></summary>
        public static long Insert(AlertSub alertSub)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                alertSub.AlertSubNum = Meth.GetLong(MethodBase.GetCurrentMethod(), alertSub);
                return alertSub.AlertSubNum;
            }
            return Crud.AlertSubCrud.Insert(alertSub);
        }

        ///<summary></summary>
        public static void Update(AlertSub alertSub)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), alertSub);
                return;
            }
            Crud.AlertSubCrud.Update(alertSub);
        }

        ///<summary></summary>
        public static void Delete(long alertSubNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), alertSubNum);
                return;
            }
            Crud.AlertSubCrud.Delete(alertSubNum);
        }

        ///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
        ///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new Apptcomm items.</summary>
        public static bool Sync(List<AlertSub> listNew, List<AlertSub> listOld)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetBool(MethodBase.GetCurrentMethod(), listNew, listOld);
            }
            return Crud.AlertSubCrud.Sync(listNew, listOld);
        }

        ///<summary>Gets all of the distinct AlertTypes across all of a user's AlertSubs.</summary>
        public static List<AlertType> GetAllAlertTypesForUser(long userNum)
        {
            //No need to check MiddleTierRole; no call to db.
            //Get all AlertCategoryNums for the user's AlertSubs.
            List<long> listAlertCatNums = GetAllForUser(userNum).Select(x => x.AlertCategoryNum).ToList();
            //Get all links between the AlertCategories and AlertTypes.
            List<AlertCategoryLink> listAlertCategoryLinks = AlertCategoryLinks.GetWhere(x => listAlertCatNums.Contains(x.AlertCategoryNum));
            //Return all distinct AlertTypes associated to links.
            return listAlertCategoryLinks.Select(x => x.AlertType).Distinct().ToList();
        }
    }
}