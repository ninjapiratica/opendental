//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections.Generic;
using System.Data;

namespace OpenDentBusiness.Mobile.Crud
{
    internal class LabResultmCrud
    {
        ///<summary>Gets one LabResultm object from the database using primaryKey1(CustomerNum) and primaryKey2.  Returns null if not found.</summary>
        internal static LabResultm SelectOne(long customerNum, long labResultNum)
        {
            string command = "SELECT * FROM labresultm "
                + "WHERE CustomerNum = " + POut.Long(customerNum) + " AND LabResultNum = " + POut.Long(labResultNum);
            List<LabResultm> list = TableToList(Db.GetTable(command));
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }

        ///<summary>Gets one LabResultm object from the database using a query.</summary>
        internal static LabResultm SelectOne(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n" + command);
            }
            List<LabResultm> list = TableToList(Db.GetTable(command));
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }

        ///<summary>Gets a list of LabResultm objects from the database using a query.</summary>
        internal static List<LabResultm> SelectMany(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n" + command);
            }
            List<LabResultm> list = TableToList(Db.GetTable(command));
            return list;
        }

        ///<summary>Converts a DataTable to a list of objects.</summary>
        internal static List<LabResultm> TableToList(DataTable table)
        {
            List<LabResultm> retVal = new List<LabResultm>();
            LabResultm labResultm;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                labResultm = new LabResultm();
                labResultm.CustomerNum = PIn.Long(table.Rows[i]["CustomerNum"].ToString());
                labResultm.LabResultNum = PIn.Long(table.Rows[i]["LabResultNum"].ToString());
                labResultm.LabPanelNum = PIn.Long(table.Rows[i]["LabPanelNum"].ToString());
                labResultm.DateTimeTest = PIn.DateT(table.Rows[i]["DateTimeTest"].ToString());
                labResultm.TestName = PIn.String(table.Rows[i]["TestName"].ToString());
                labResultm.TestID = PIn.String(table.Rows[i]["TestID"].ToString());
                labResultm.ObsValue = PIn.String(table.Rows[i]["ObsValue"].ToString());
                labResultm.ObsUnits = PIn.String(table.Rows[i]["ObsUnits"].ToString());
                labResultm.ObsRange = PIn.String(table.Rows[i]["ObsRange"].ToString());
                labResultm.AbnormalFlag = (LabAbnormalFlag)PIn.Int(table.Rows[i]["AbnormalFlag"].ToString());
                retVal.Add(labResultm);
            }
            return retVal;
        }

        ///<summary>Usually set useExistingPK=true.  Inserts one LabResultm into the database.</summary>
        internal static long Insert(LabResultm labResultm, bool useExistingPK)
        {
            if (!useExistingPK)
            {
                labResultm.LabResultNum = ReplicationServers.GetKey("labresultm", "LabResultNum");
            }
            string command = "INSERT INTO labresultm (";
            command += "LabResultNum,";
            command += "CustomerNum,LabPanelNum,DateTimeTest,TestName,TestID,ObsValue,ObsUnits,ObsRange,AbnormalFlag) VALUES(";
            command += POut.Long(labResultm.LabResultNum) + ",";
            command +=
                     POut.Long(labResultm.CustomerNum) + ","
                + POut.Long(labResultm.LabPanelNum) + ","
                + POut.DateT(labResultm.DateTimeTest) + ","
                + "'" + POut.String(labResultm.TestName) + "',"
                + "'" + POut.String(labResultm.TestID) + "',"
                + "'" + POut.String(labResultm.ObsValue) + "',"
                + "'" + POut.String(labResultm.ObsUnits) + "',"
                + "'" + POut.String(labResultm.ObsRange) + "',"
                + POut.Int((int)labResultm.AbnormalFlag) + ")";
            Db.NonQ(command);//There is no autoincrement in the mobile server.
            return labResultm.LabResultNum;
        }

        ///<summary>Updates one LabResultm in the database.</summary>
        internal static void Update(LabResultm labResultm)
        {
            string command = "UPDATE labresultm SET "
                + "LabPanelNum =  " + POut.Long(labResultm.LabPanelNum) + ", "
                + "DateTimeTest=  " + POut.DateT(labResultm.DateTimeTest) + ", "
                + "TestName    = '" + POut.String(labResultm.TestName) + "', "
                + "TestID      = '" + POut.String(labResultm.TestID) + "', "
                + "ObsValue    = '" + POut.String(labResultm.ObsValue) + "', "
                + "ObsUnits    = '" + POut.String(labResultm.ObsUnits) + "', "
                + "ObsRange    = '" + POut.String(labResultm.ObsRange) + "', "
                + "AbnormalFlag=  " + POut.Int((int)labResultm.AbnormalFlag) + " "
                + "WHERE CustomerNum = " + POut.Long(labResultm.CustomerNum) + " AND LabResultNum = " + POut.Long(labResultm.LabResultNum);
            Db.NonQ(command);
        }

        ///<summary>Deletes one LabResultm from the database.</summary>
        internal static void Delete(long customerNum, long labResultNum)
        {
            string command = "DELETE FROM labresultm "
                + "WHERE CustomerNum = " + POut.Long(customerNum) + " AND LabResultNum = " + POut.Long(labResultNum);
            Db.NonQ(command);
        }

        ///<summary>Converts one LabResult object to its mobile equivalent.  Warning! CustomerNum will always be 0.</summary>
        internal static LabResultm ConvertToM(LabResult labResult)
        {
            LabResultm labResultm = new LabResultm();
            //CustomerNum cannot be set.  Remains 0.
            labResultm.LabResultNum = labResult.LabResultNum;
            labResultm.LabPanelNum = labResult.LabPanelNum;
            labResultm.DateTimeTest = labResult.DateTimeTest;
            labResultm.TestName = labResult.TestName;
            labResultm.TestID = labResult.TestID;
            labResultm.ObsValue = labResult.ObsValue;
            labResultm.ObsUnits = labResult.ObsUnits;
            labResultm.ObsRange = labResult.ObsRange;
            labResultm.AbnormalFlag = labResult.AbnormalFlag;
            return labResultm;
        }

    }
}