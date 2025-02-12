//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections.Generic;
using System.Data;

namespace OpenDentBusiness.Mobile.Crud
{
    internal class AllergyDefmCrud
    {
        ///<summary>Gets one AllergyDefm object from the database using primaryKey1(CustomerNum) and primaryKey2.  Returns null if not found.</summary>
        internal static AllergyDefm SelectOne(long customerNum, long allergyDefNum)
        {
            string command = "SELECT * FROM allergydefm "
                + "WHERE CustomerNum = " + POut.Long(customerNum) + " AND AllergyDefNum = " + POut.Long(allergyDefNum);
            List<AllergyDefm> list = TableToList(Db.GetTable(command));
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }

        ///<summary>Gets one AllergyDefm object from the database using a query.</summary>
        internal static AllergyDefm SelectOne(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n" + command);
            }
            List<AllergyDefm> list = TableToList(Db.GetTable(command));
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }

        ///<summary>Gets a list of AllergyDefm objects from the database using a query.</summary>
        internal static List<AllergyDefm> SelectMany(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n" + command);
            }
            List<AllergyDefm> list = TableToList(Db.GetTable(command));
            return list;
        }

        ///<summary>Converts a DataTable to a list of objects.</summary>
        internal static List<AllergyDefm> TableToList(DataTable table)
        {
            List<AllergyDefm> retVal = new List<AllergyDefm>();
            AllergyDefm allergyDefm;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                allergyDefm = new AllergyDefm();
                allergyDefm.CustomerNum = PIn.Long(table.Rows[i]["CustomerNum"].ToString());
                allergyDefm.AllergyDefNum = PIn.Long(table.Rows[i]["AllergyDefNum"].ToString());
                allergyDefm.Description = PIn.String(table.Rows[i]["Description"].ToString());
                allergyDefm.Snomed = (SnomedAllergy)PIn.Int(table.Rows[i]["SnomedType"].ToString());
                allergyDefm.MedicationNum = PIn.Long(table.Rows[i]["MedicationNum"].ToString());
                retVal.Add(allergyDefm);
            }
            return retVal;
        }

        ///<summary>Usually set useExistingPK=true.  Inserts one AllergyDefm into the database.</summary>
        internal static long Insert(AllergyDefm allergyDefm, bool useExistingPK)
        {
            if (!useExistingPK)
            {
                allergyDefm.AllergyDefNum = ReplicationServers.GetKey("allergydefm", "AllergyDefNum");
            }
            string command = "INSERT INTO allergydefm (";
            command += "AllergyDefNum,";
            command += "CustomerNum,Description,Snomed,MedicationNum) VALUES(";
            command += POut.Long(allergyDefm.AllergyDefNum) + ",";
            command +=
                     POut.Long(allergyDefm.CustomerNum) + ","
                + "'" + POut.String(allergyDefm.Description) + "',"
                + POut.Int((int)allergyDefm.Snomed) + ","
                + POut.Long(allergyDefm.MedicationNum) + ")";
            Db.NonQ(command);//There is no autoincrement in the mobile server.
            return allergyDefm.AllergyDefNum;
        }

        ///<summary>Updates one AllergyDefm in the database.</summary>
        internal static void Update(AllergyDefm allergyDefm)
        {
            string command = "UPDATE allergydefm SET "
                + "Description  = '" + POut.String(allergyDefm.Description) + "', "
                + "Snomed       =  " + POut.Int((int)allergyDefm.Snomed) + ", "
                + "MedicationNum=  " + POut.Long(allergyDefm.MedicationNum) + " "
                + "WHERE CustomerNum = " + POut.Long(allergyDefm.CustomerNum) + " AND AllergyDefNum = " + POut.Long(allergyDefm.AllergyDefNum);
            Db.NonQ(command);
        }

        ///<summary>Deletes one AllergyDefm from the database.</summary>
        internal static void Delete(long customerNum, long allergyDefNum)
        {
            string command = "DELETE FROM allergydefm "
                + "WHERE CustomerNum = " + POut.Long(customerNum) + " AND AllergyDefNum = " + POut.Long(allergyDefNum);
            Db.NonQ(command);
        }

        ///<summary>Converts one AllergyDef object to its mobile equivalent.  Warning! CustomerNum will always be 0.</summary>
        internal static AllergyDefm ConvertToM(AllergyDef allergyDef)
        {
            AllergyDefm allergyDefm = new AllergyDefm();
            //CustomerNum cannot be set.  Remains 0.
            allergyDefm.AllergyDefNum = allergyDef.AllergyDefNum;
            allergyDefm.Description = allergyDef.Description;
            allergyDefm.Snomed = allergyDef.SnomedType;
            allergyDefm.MedicationNum = allergyDef.MedicationNum;
            return allergyDefm;
        }

    }
}