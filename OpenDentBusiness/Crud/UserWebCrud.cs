//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Crud
{
    public class UserWebCrud
    {
        ///<summary>Gets one UserWeb object from the database using the primary key.  Returns null if not found.</summary>
        public static UserWeb SelectOne(long userWebNum)
        {
            string command = "SELECT * FROM userweb "
                + "WHERE UserWebNum = " + POut.Long(userWebNum);
            List<UserWeb> list = TableToList(Db.GetTable(command));
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }

        ///<summary>Gets one UserWeb object from the database using a query.</summary>
        public static UserWeb SelectOne(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n" + command);
            }
            List<UserWeb> list = TableToList(Db.GetTable(command));
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }

        ///<summary>Gets a list of UserWeb objects from the database using a query.</summary>
        public static List<UserWeb> SelectMany(string command)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n" + command);
            }
            List<UserWeb> list = TableToList(Db.GetTable(command));
            return list;
        }

        ///<summary>Converts a DataTable to a list of objects.</summary>
        public static List<UserWeb> TableToList(DataTable table)
        {
            List<UserWeb> retVal = new List<UserWeb>();
            UserWeb userWeb;
            foreach (DataRow row in table.Rows)
            {
                userWeb = new UserWeb();
                userWeb.UserWebNum = PIn.Long(row["UserWebNum"].ToString());
                userWeb.FKey = PIn.Long(row["FKey"].ToString());
                userWeb.FKeyType = (OpenDentBusiness.UserWebFKeyType)PIn.Int(row["FKeyType"].ToString());
                userWeb.UserName = PIn.String(row["UserName"].ToString());
                userWeb.Password = PIn.String(row["Password"].ToString());
                userWeb.PasswordResetCode = PIn.String(row["PasswordResetCode"].ToString());
                userWeb.RequireUserNameChange = PIn.Bool(row["RequireUserNameChange"].ToString());
                userWeb.DateTimeLastLogin = PIn.DateT(row["DateTimeLastLogin"].ToString());
                userWeb.RequirePasswordChange = PIn.Bool(row["RequirePasswordChange"].ToString());
                retVal.Add(userWeb);
            }
            return retVal;
        }

        ///<summary>Converts a list of UserWeb into a DataTable.</summary>
        public static DataTable ListToTable(List<UserWeb> listUserWebs, string tableName = "")
        {
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = "UserWeb";
            }
            DataTable table = new DataTable(tableName);
            table.Columns.Add("UserWebNum");
            table.Columns.Add("FKey");
            table.Columns.Add("FKeyType");
            table.Columns.Add("UserName");
            table.Columns.Add("Password");
            table.Columns.Add("PasswordResetCode");
            table.Columns.Add("RequireUserNameChange");
            table.Columns.Add("DateTimeLastLogin");
            table.Columns.Add("RequirePasswordChange");
            foreach (UserWeb userWeb in listUserWebs)
            {
                table.Rows.Add(new object[] {
                    POut.Long  (userWeb.UserWebNum),
                    POut.Long  (userWeb.FKey),
                    POut.Int   ((int)userWeb.FKeyType),
                                userWeb.UserName,
                                userWeb.Password,
                                userWeb.PasswordResetCode,
                    POut.Bool  (userWeb.RequireUserNameChange),
                    POut.DateT (userWeb.DateTimeLastLogin,false),
                    POut.Bool  (userWeb.RequirePasswordChange),
                });
            }
            return table;
        }

        ///<summary>Inserts one UserWeb into the database.  Returns the new priKey.</summary>
        public static long Insert(UserWeb userWeb)
        {
            return Insert(userWeb, false);
        }

        ///<summary>Inserts one UserWeb into the database.  Provides option to use the existing priKey.</summary>
        public static long Insert(UserWeb userWeb, bool useExistingPK)
        {
            if (!useExistingPK && PrefC.RandomKeys)
            {
                userWeb.UserWebNum = ReplicationServers.GetKey("userweb", "UserWebNum");
            }
            string command = "INSERT INTO userweb (";
            if (useExistingPK || PrefC.RandomKeys)
            {
                command += "UserWebNum,";
            }
            command += "FKey,FKeyType,UserName,Password,PasswordResetCode,RequireUserNameChange,DateTimeLastLogin,RequirePasswordChange) VALUES(";
            if (useExistingPK || PrefC.RandomKeys)
            {
                command += POut.Long(userWeb.UserWebNum) + ",";
            }
            command +=
                     POut.Long(userWeb.FKey) + ","
                + POut.Int((int)userWeb.FKeyType) + ","
                + "'" + POut.String(userWeb.UserName) + "',"
                + "'" + POut.String(userWeb.Password) + "',"
                + "'" + POut.String(userWeb.PasswordResetCode) + "',"
                + POut.Bool(userWeb.RequireUserNameChange) + ","
                + POut.DateT(userWeb.DateTimeLastLogin) + ","
                + POut.Bool(userWeb.RequirePasswordChange) + ")";
            if (useExistingPK || PrefC.RandomKeys)
            {
                Db.NonQ(command);
            }
            else
            {
                userWeb.UserWebNum = Db.NonQ(command, true, "UserWebNum", "userWeb");
            }
            return userWeb.UserWebNum;
        }

        ///<summary>Inserts many UserWebs into the database.</summary>
        public static void InsertMany(List<UserWeb> listUserWebs)
        {
            InsertMany(listUserWebs, false);
        }

        ///<summary>Inserts many UserWebs into the database.  Provides option to use the existing priKey.</summary>
        public static void InsertMany(List<UserWeb> listUserWebs, bool useExistingPK)
        {
            if (!useExistingPK && PrefC.RandomKeys)
            {
                foreach (UserWeb userWeb in listUserWebs)
                {
                    Insert(userWeb);
                }
            }
            else
            {
                StringBuilder sbCommands = null;
                int index = 0;
                int countRows = 0;
                while (index < listUserWebs.Count)
                {
                    UserWeb userWeb = listUserWebs[index];
                    StringBuilder sbRow = new StringBuilder("(");
                    bool hasComma = false;
                    if (sbCommands == null)
                    {
                        sbCommands = new StringBuilder();
                        sbCommands.Append("INSERT INTO userweb (");
                        if (useExistingPK)
                        {
                            sbCommands.Append("UserWebNum,");
                        }
                        sbCommands.Append("FKey,FKeyType,UserName,Password,PasswordResetCode,RequireUserNameChange,DateTimeLastLogin,RequirePasswordChange) VALUES ");
                        countRows = 0;
                    }
                    else
                    {
                        hasComma = true;
                    }
                    if (useExistingPK)
                    {
                        sbRow.Append(POut.Long(userWeb.UserWebNum)); sbRow.Append(",");
                    }
                    sbRow.Append(POut.Long(userWeb.FKey)); sbRow.Append(",");
                    sbRow.Append(POut.Int((int)userWeb.FKeyType)); sbRow.Append(",");
                    sbRow.Append("'" + POut.String(userWeb.UserName) + "'"); sbRow.Append(",");
                    sbRow.Append("'" + POut.String(userWeb.Password) + "'"); sbRow.Append(",");
                    sbRow.Append("'" + POut.String(userWeb.PasswordResetCode) + "'"); sbRow.Append(",");
                    sbRow.Append(POut.Bool(userWeb.RequireUserNameChange)); sbRow.Append(",");
                    sbRow.Append(POut.DateT(userWeb.DateTimeLastLogin)); sbRow.Append(",");
                    sbRow.Append(POut.Bool(userWeb.RequirePasswordChange)); sbRow.Append(")");
                    if (sbCommands.Length + sbRow.Length + 1 > TableBase.MaxAllowedPacketCount && countRows > 0)
                    {
                        Db.NonQ(sbCommands.ToString());
                        sbCommands = null;
                    }
                    else
                    {
                        if (hasComma)
                        {
                            sbCommands.Append(",");
                        }
                        sbCommands.Append(sbRow.ToString());
                        countRows++;
                        if (index == listUserWebs.Count - 1)
                        {
                            Db.NonQ(sbCommands.ToString());
                        }
                        index++;
                    }
                }
            }
        }

        ///<summary>Inserts one UserWeb into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
        public static long InsertNoCache(UserWeb userWeb)
        {
            return InsertNoCache(userWeb, false);
        }

        ///<summary>Inserts one UserWeb into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
        public static long InsertNoCache(UserWeb userWeb, bool useExistingPK)
        {
            bool isRandomKeys = Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
            string command = "INSERT INTO userweb (";
            if (!useExistingPK && isRandomKeys)
            {
                userWeb.UserWebNum = ReplicationServers.GetKeyNoCache("userweb", "UserWebNum");
            }
            if (isRandomKeys || useExistingPK)
            {
                command += "UserWebNum,";
            }
            command += "FKey,FKeyType,UserName,Password,PasswordResetCode,RequireUserNameChange,DateTimeLastLogin,RequirePasswordChange) VALUES(";
            if (isRandomKeys || useExistingPK)
            {
                command += POut.Long(userWeb.UserWebNum) + ",";
            }
            command +=
                     POut.Long(userWeb.FKey) + ","
                + POut.Int((int)userWeb.FKeyType) + ","
                + "'" + POut.String(userWeb.UserName) + "',"
                + "'" + POut.String(userWeb.Password) + "',"
                + "'" + POut.String(userWeb.PasswordResetCode) + "',"
                + POut.Bool(userWeb.RequireUserNameChange) + ","
                + POut.DateT(userWeb.DateTimeLastLogin) + ","
                + POut.Bool(userWeb.RequirePasswordChange) + ")";
            if (useExistingPK || isRandomKeys)
            {
                Db.NonQ(command);
            }
            else
            {
                userWeb.UserWebNum = Db.NonQ(command, true, "UserWebNum", "userWeb");
            }
            return userWeb.UserWebNum;
        }

        ///<summary>Updates one UserWeb in the database.</summary>
        public static void Update(UserWeb userWeb)
        {
            string command = "UPDATE userweb SET "
                + "FKey                 =  " + POut.Long(userWeb.FKey) + ", "
                + "FKeyType             =  " + POut.Int((int)userWeb.FKeyType) + ", "
                + "UserName             = '" + POut.String(userWeb.UserName) + "', "
                + "Password             = '" + POut.String(userWeb.Password) + "', "
                + "PasswordResetCode    = '" + POut.String(userWeb.PasswordResetCode) + "', "
                + "RequireUserNameChange=  " + POut.Bool(userWeb.RequireUserNameChange) + ", "
                + "DateTimeLastLogin    =  " + POut.DateT(userWeb.DateTimeLastLogin) + ", "
                + "RequirePasswordChange=  " + POut.Bool(userWeb.RequirePasswordChange) + " "
                + "WHERE UserWebNum = " + POut.Long(userWeb.UserWebNum);
            Db.NonQ(command);
        }

        ///<summary>Updates one UserWeb in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
        public static bool Update(UserWeb userWeb, UserWeb oldUserWeb)
        {
            string command = "";
            if (userWeb.FKey != oldUserWeb.FKey)
            {
                if (command != "") { command += ","; }
                command += "FKey = " + POut.Long(userWeb.FKey) + "";
            }
            if (userWeb.FKeyType != oldUserWeb.FKeyType)
            {
                if (command != "") { command += ","; }
                command += "FKeyType = " + POut.Int((int)userWeb.FKeyType) + "";
            }
            if (userWeb.UserName != oldUserWeb.UserName)
            {
                if (command != "") { command += ","; }
                command += "UserName = '" + POut.String(userWeb.UserName) + "'";
            }
            if (userWeb.Password != oldUserWeb.Password)
            {
                if (command != "") { command += ","; }
                command += "Password = '" + POut.String(userWeb.Password) + "'";
            }
            if (userWeb.PasswordResetCode != oldUserWeb.PasswordResetCode)
            {
                if (command != "") { command += ","; }
                command += "PasswordResetCode = '" + POut.String(userWeb.PasswordResetCode) + "'";
            }
            if (userWeb.RequireUserNameChange != oldUserWeb.RequireUserNameChange)
            {
                if (command != "") { command += ","; }
                command += "RequireUserNameChange = " + POut.Bool(userWeb.RequireUserNameChange) + "";
            }
            if (userWeb.DateTimeLastLogin != oldUserWeb.DateTimeLastLogin)
            {
                if (command != "") { command += ","; }
                command += "DateTimeLastLogin = " + POut.DateT(userWeb.DateTimeLastLogin) + "";
            }
            if (userWeb.RequirePasswordChange != oldUserWeb.RequirePasswordChange)
            {
                if (command != "") { command += ","; }
                command += "RequirePasswordChange = " + POut.Bool(userWeb.RequirePasswordChange) + "";
            }
            if (command == "")
            {
                return false;
            }
            command = "UPDATE userweb SET " + command
                + " WHERE UserWebNum = " + POut.Long(userWeb.UserWebNum);
            Db.NonQ(command);
            return true;
        }

        ///<summary>Returns true if Update(UserWeb,UserWeb) would make changes to the database.
        ///Does not make any changes to the database and can be called before remoting role is checked.</summary>
        public static bool UpdateComparison(UserWeb userWeb, UserWeb oldUserWeb)
        {
            if (userWeb.FKey != oldUserWeb.FKey)
            {
                return true;
            }
            if (userWeb.FKeyType != oldUserWeb.FKeyType)
            {
                return true;
            }
            if (userWeb.UserName != oldUserWeb.UserName)
            {
                return true;
            }
            if (userWeb.Password != oldUserWeb.Password)
            {
                return true;
            }
            if (userWeb.PasswordResetCode != oldUserWeb.PasswordResetCode)
            {
                return true;
            }
            if (userWeb.RequireUserNameChange != oldUserWeb.RequireUserNameChange)
            {
                return true;
            }
            if (userWeb.DateTimeLastLogin != oldUserWeb.DateTimeLastLogin)
            {
                return true;
            }
            if (userWeb.RequirePasswordChange != oldUserWeb.RequirePasswordChange)
            {
                return true;
            }
            return false;
        }

        ///<summary>Deletes one UserWeb from the database.</summary>
        public static void Delete(long userWebNum)
        {
            string command = "DELETE FROM userweb "
                + "WHERE UserWebNum = " + POut.Long(userWebNum);
            Db.NonQ(command);
        }

        ///<summary>Deletes many UserWebs from the database.</summary>
        public static void DeleteMany(List<long> listUserWebNums)
        {
            if (listUserWebNums == null || listUserWebNums.Count == 0)
            {
                return;
            }
            string command = "DELETE FROM userweb "
                + "WHERE UserWebNum IN(" + string.Join(",", listUserWebNums.Select(x => POut.Long(x))) + ")";
            Db.NonQ(command);
        }

    }
}