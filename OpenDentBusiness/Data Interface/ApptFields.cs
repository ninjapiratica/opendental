using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class ApptFields
    {
        #region Get Methods

        ///<summary>Gets one ApptField from the db.</summary>
        public static ApptField GetOne(long apptFieldNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<ApptField>(MethodBase.GetCurrentMethod(), apptFieldNum);
            }
            return Crud.ApptFieldCrud.SelectOne(apptFieldNum);
        }

        public static List<ApptField> GetForAppt(long aptNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<ApptField>>(MethodBase.GetCurrentMethod(), aptNum);
            }
            string command = "SELECT * FROM apptfield WHERE AptNum = " + POut.Long(aptNum);
            return Crud.ApptFieldCrud.SelectMany(command);
        }

        ///<summary>Returns whether there are more than 0 apptFields with the given field name.</summary>
        public static bool IsFieldNameInUse(string fieldName)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetBool(MethodBase.GetCurrentMethod(), fieldName);
            }
            string command = "SELECT COUNT(*) FROM apptfield WHERE FieldName='" + POut.String(fieldName) + "'";
            return Db.GetCount(command) != "0";
        }
        #endregion

        #region Insert

        ///<summary></summary>
        public static long Insert(ApptField apptField)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                apptField.ApptFieldNum = Meth.GetLong(MethodBase.GetCurrentMethod(), apptField);
                return apptField.ApptFieldNum;
            }
            return Crud.ApptFieldCrud.Insert(apptField);
        }
        #endregion

        #region Update

        ///<summary></summary>
        public static void Update(ApptField apptField)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), apptField);
                return;
            }
            Crud.ApptFieldCrud.Update(apptField);
        }

        ///<summary>Deletes any pre-existing appt fields for the AptNum and FieldName combo and then inserts the apptField passed in.</summary>
        public static long Upsert(ApptField apptField)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                apptField.ApptFieldNum = Meth.GetLong(MethodBase.GetCurrentMethod(), apptField);
                return apptField.ApptFieldNum;
            }
            //There could already be an appt field in the database due to concurrency.  Delete all entries prior to inserting the new one.
            DeleteFieldForAppt(apptField.FieldName, apptField.AptNum);//Last in wins.
            return Insert(apptField);
        }

        #endregion

        #region Delete

        ///<summary></summary>
        public static void Delete(long apptFieldNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), apptFieldNum);
                return;
            }
            string command = "DELETE FROM apptfield WHERE ApptFieldNum = " + POut.Long(apptFieldNum);
            Db.NonQ(command);
        }

        ///<summary>Deletes all fields for the appointment and field name passed in.</summary>
        public static void DeleteFieldForAppt(string fieldName, long aptNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), fieldName, aptNum);
                return;
            }
            string command = $@"DELETE FROM apptfield 
				WHERE AptNum = {POut.Long(aptNum)}
				AND FieldName ='{POut.String(fieldName)}'";
            Db.NonQ(command);
        }

        #endregion
    }
}