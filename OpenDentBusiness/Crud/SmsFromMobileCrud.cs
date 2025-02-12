//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class SmsFromMobileCrud {
		///<summary>Gets one SmsFromMobile object from the database using the primary key.  Returns null if not found.</summary>
		public static SmsFromMobile SelectOne(long smsFromMobileNum) {
			string command="SELECT * FROM smsfrommobile "
				+"WHERE SmsFromMobileNum = "+POut.Long(smsFromMobileNum);
			List<SmsFromMobile> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one SmsFromMobile object from the database using a query.</summary>
		public static SmsFromMobile SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<SmsFromMobile> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of SmsFromMobile objects from the database using a query.</summary>
		public static List<SmsFromMobile> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<SmsFromMobile> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<SmsFromMobile> TableToList(DataTable table) {
			List<SmsFromMobile> retVal=new List<SmsFromMobile>();
			SmsFromMobile smsFromMobile;
			foreach(DataRow row in table.Rows) {
				smsFromMobile=new SmsFromMobile();
				smsFromMobile.SmsFromMobileNum = PIn.Long  (row["SmsFromMobileNum"].ToString());
				smsFromMobile.PatNum           = PIn.Long  (row["PatNum"].ToString());
				smsFromMobile.ClinicNum        = PIn.Long  (row["ClinicNum"].ToString());
				smsFromMobile.CommlogNum       = PIn.Long  (row["CommlogNum"].ToString());
				smsFromMobile.MsgText          = PIn.String(row["MsgText"].ToString());
				smsFromMobile.DateTimeReceived = PIn.DateT (row["DateTimeReceived"].ToString());
				smsFromMobile.SmsPhoneNumber   = PIn.String(row["SmsPhoneNumber"].ToString());
				smsFromMobile.MobilePhoneNumber= PIn.String(row["MobilePhoneNumber"].ToString());
				smsFromMobile.MsgPart          = PIn.Int   (row["MsgPart"].ToString());
				smsFromMobile.MsgTotal         = PIn.Int   (row["MsgTotal"].ToString());
				smsFromMobile.MsgRefID         = PIn.String(row["MsgRefID"].ToString());
				smsFromMobile.SmsStatus        = (OpenDentBusiness.SmsFromStatus)PIn.Int(row["SmsStatus"].ToString());
				smsFromMobile.Flags            = PIn.String(row["Flags"].ToString());
				smsFromMobile.IsHidden         = PIn.Bool  (row["IsHidden"].ToString());
				smsFromMobile.MatchCount       = PIn.Int   (row["MatchCount"].ToString());
				smsFromMobile.GuidMessage      = PIn.String(row["GuidMessage"].ToString());
				smsFromMobile.SecDateTEdit     = PIn.DateT (row["SecDateTEdit"].ToString());
				retVal.Add(smsFromMobile);
			}
			return retVal;
		}

		///<summary>Converts a list of SmsFromMobile into a DataTable.</summary>
		public static DataTable ListToTable(List<SmsFromMobile> listSmsFromMobiles,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="SmsFromMobile";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("SmsFromMobileNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("CommlogNum");
			table.Columns.Add("MsgText");
			table.Columns.Add("DateTimeReceived");
			table.Columns.Add("SmsPhoneNumber");
			table.Columns.Add("MobilePhoneNumber");
			table.Columns.Add("MsgPart");
			table.Columns.Add("MsgTotal");
			table.Columns.Add("MsgRefID");
			table.Columns.Add("SmsStatus");
			table.Columns.Add("Flags");
			table.Columns.Add("IsHidden");
			table.Columns.Add("MatchCount");
			table.Columns.Add("GuidMessage");
			table.Columns.Add("SecDateTEdit");
			foreach(SmsFromMobile smsFromMobile in listSmsFromMobiles) {
				table.Rows.Add(new object[] {
					POut.Long  (smsFromMobile.SmsFromMobileNum),
					POut.Long  (smsFromMobile.PatNum),
					POut.Long  (smsFromMobile.ClinicNum),
					POut.Long  (smsFromMobile.CommlogNum),
					            smsFromMobile.MsgText,
					POut.DateT (smsFromMobile.DateTimeReceived,false),
					            smsFromMobile.SmsPhoneNumber,
					            smsFromMobile.MobilePhoneNumber,
					POut.Int   (smsFromMobile.MsgPart),
					POut.Int   (smsFromMobile.MsgTotal),
					            smsFromMobile.MsgRefID,
					POut.Int   ((int)smsFromMobile.SmsStatus),
					            smsFromMobile.Flags,
					POut.Bool  (smsFromMobile.IsHidden),
					POut.Int   (smsFromMobile.MatchCount),
					            smsFromMobile.GuidMessage,
					POut.DateT (smsFromMobile.SecDateTEdit,false),
				});
			}
			return table;
		}

		///<summary>Inserts one SmsFromMobile into the database.  Returns the new priKey.</summary>
		public static long Insert(SmsFromMobile smsFromMobile) {
			return Insert(smsFromMobile,false);
		}

		///<summary>Inserts one SmsFromMobile into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(SmsFromMobile smsFromMobile,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				smsFromMobile.SmsFromMobileNum=ReplicationServers.GetKey("smsfrommobile","SmsFromMobileNum");
			}
			string command="INSERT INTO smsfrommobile (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="SmsFromMobileNum,";
			}
			command+="PatNum,ClinicNum,CommlogNum,MsgText,DateTimeReceived,SmsPhoneNumber,MobilePhoneNumber,MsgPart,MsgTotal,MsgRefID,SmsStatus,Flags,IsHidden,MatchCount,GuidMessage) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(smsFromMobile.SmsFromMobileNum)+",";
			}
			command+=
				     POut.Long  (smsFromMobile.PatNum)+","
				+    POut.Long  (smsFromMobile.ClinicNum)+","
				+    POut.Long  (smsFromMobile.CommlogNum)+","
				+    DbHelper.ParamChar+"paramMsgText,"
				+    POut.DateT (smsFromMobile.DateTimeReceived)+","
				+"'"+POut.String(smsFromMobile.SmsPhoneNumber)+"',"
				+"'"+POut.String(smsFromMobile.MobilePhoneNumber)+"',"
				+    POut.Int   (smsFromMobile.MsgPart)+","
				+    POut.Int   (smsFromMobile.MsgTotal)+","
				+"'"+POut.String(smsFromMobile.MsgRefID)+"',"
				+    POut.Int   ((int)smsFromMobile.SmsStatus)+","
				+"'"+POut.String(smsFromMobile.Flags)+"',"
				+    POut.Bool  (smsFromMobile.IsHidden)+","
				+    POut.Int   (smsFromMobile.MatchCount)+","
				+"'"+POut.String(smsFromMobile.GuidMessage)+"')";
				//SecDateTEdit can only be set by MySQL
			if(smsFromMobile.MsgText==null) {
				smsFromMobile.MsgText="";
			}
			OdSqlParameter paramMsgText=new OdSqlParameter("paramMsgText",OdDbType.Text,POut.StringNote(smsFromMobile.MsgText));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramMsgText);
			}
			else {
				smsFromMobile.SmsFromMobileNum=Db.NonQ(command,true,"SmsFromMobileNum","smsFromMobile",paramMsgText);
			}
			return smsFromMobile.SmsFromMobileNum;
		}

		///<summary>Inserts one SmsFromMobile into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(SmsFromMobile smsFromMobile) {
			return InsertNoCache(smsFromMobile,false);
		}

		///<summary>Inserts one SmsFromMobile into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(SmsFromMobile smsFromMobile,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO smsfrommobile (";
			if(!useExistingPK && isRandomKeys) {
				smsFromMobile.SmsFromMobileNum=ReplicationServers.GetKeyNoCache("smsfrommobile","SmsFromMobileNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="SmsFromMobileNum,";
			}
			command+="PatNum,ClinicNum,CommlogNum,MsgText,DateTimeReceived,SmsPhoneNumber,MobilePhoneNumber,MsgPart,MsgTotal,MsgRefID,SmsStatus,Flags,IsHidden,MatchCount,GuidMessage) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(smsFromMobile.SmsFromMobileNum)+",";
			}
			command+=
				     POut.Long  (smsFromMobile.PatNum)+","
				+    POut.Long  (smsFromMobile.ClinicNum)+","
				+    POut.Long  (smsFromMobile.CommlogNum)+","
				+    DbHelper.ParamChar+"paramMsgText,"
				+    POut.DateT (smsFromMobile.DateTimeReceived)+","
				+"'"+POut.String(smsFromMobile.SmsPhoneNumber)+"',"
				+"'"+POut.String(smsFromMobile.MobilePhoneNumber)+"',"
				+    POut.Int   (smsFromMobile.MsgPart)+","
				+    POut.Int   (smsFromMobile.MsgTotal)+","
				+"'"+POut.String(smsFromMobile.MsgRefID)+"',"
				+    POut.Int   ((int)smsFromMobile.SmsStatus)+","
				+"'"+POut.String(smsFromMobile.Flags)+"',"
				+    POut.Bool  (smsFromMobile.IsHidden)+","
				+    POut.Int   (smsFromMobile.MatchCount)+","
				+"'"+POut.String(smsFromMobile.GuidMessage)+"')";
				//SecDateTEdit can only be set by MySQL
			if(smsFromMobile.MsgText==null) {
				smsFromMobile.MsgText="";
			}
			OdSqlParameter paramMsgText=new OdSqlParameter("paramMsgText",OdDbType.Text,POut.StringNote(smsFromMobile.MsgText));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramMsgText);
			}
			else {
				smsFromMobile.SmsFromMobileNum=Db.NonQ(command,true,"SmsFromMobileNum","smsFromMobile",paramMsgText);
			}
			return smsFromMobile.SmsFromMobileNum;
		}

		///<summary>Updates one SmsFromMobile in the database.</summary>
		public static void Update(SmsFromMobile smsFromMobile) {
			string command="UPDATE smsfrommobile SET "
				+"PatNum           =  "+POut.Long  (smsFromMobile.PatNum)+", "
				+"ClinicNum        =  "+POut.Long  (smsFromMobile.ClinicNum)+", "
				+"CommlogNum       =  "+POut.Long  (smsFromMobile.CommlogNum)+", "
				+"MsgText          =  "+DbHelper.ParamChar+"paramMsgText, "
				+"DateTimeReceived =  "+POut.DateT (smsFromMobile.DateTimeReceived)+", "
				+"SmsPhoneNumber   = '"+POut.String(smsFromMobile.SmsPhoneNumber)+"', "
				+"MobilePhoneNumber= '"+POut.String(smsFromMobile.MobilePhoneNumber)+"', "
				+"MsgPart          =  "+POut.Int   (smsFromMobile.MsgPart)+", "
				+"MsgTotal         =  "+POut.Int   (smsFromMobile.MsgTotal)+", "
				+"MsgRefID         = '"+POut.String(smsFromMobile.MsgRefID)+"', "
				+"SmsStatus        =  "+POut.Int   ((int)smsFromMobile.SmsStatus)+", "
				+"Flags            = '"+POut.String(smsFromMobile.Flags)+"', "
				+"IsHidden         =  "+POut.Bool  (smsFromMobile.IsHidden)+", "
				+"MatchCount       =  "+POut.Int   (smsFromMobile.MatchCount)+", "
				+"GuidMessage      = '"+POut.String(smsFromMobile.GuidMessage)+"' "
				//SecDateTEdit can only be set by MySQL
				+"WHERE SmsFromMobileNum = "+POut.Long(smsFromMobile.SmsFromMobileNum);
			if(smsFromMobile.MsgText==null) {
				smsFromMobile.MsgText="";
			}
			OdSqlParameter paramMsgText=new OdSqlParameter("paramMsgText",OdDbType.Text,POut.StringNote(smsFromMobile.MsgText));
			Db.NonQ(command,paramMsgText);
		}

		///<summary>Updates one SmsFromMobile in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(SmsFromMobile smsFromMobile,SmsFromMobile oldSmsFromMobile) {
			string command="";
			if(smsFromMobile.PatNum != oldSmsFromMobile.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(smsFromMobile.PatNum)+"";
			}
			if(smsFromMobile.ClinicNum != oldSmsFromMobile.ClinicNum) {
				if(command!="") { command+=",";}
				command+="ClinicNum = "+POut.Long(smsFromMobile.ClinicNum)+"";
			}
			if(smsFromMobile.CommlogNum != oldSmsFromMobile.CommlogNum) {
				if(command!="") { command+=",";}
				command+="CommlogNum = "+POut.Long(smsFromMobile.CommlogNum)+"";
			}
			if(smsFromMobile.MsgText != oldSmsFromMobile.MsgText) {
				if(command!="") { command+=",";}
				command+="MsgText = "+DbHelper.ParamChar+"paramMsgText";
			}
			if(smsFromMobile.DateTimeReceived != oldSmsFromMobile.DateTimeReceived) {
				if(command!="") { command+=",";}
				command+="DateTimeReceived = "+POut.DateT(smsFromMobile.DateTimeReceived)+"";
			}
			if(smsFromMobile.SmsPhoneNumber != oldSmsFromMobile.SmsPhoneNumber) {
				if(command!="") { command+=",";}
				command+="SmsPhoneNumber = '"+POut.String(smsFromMobile.SmsPhoneNumber)+"'";
			}
			if(smsFromMobile.MobilePhoneNumber != oldSmsFromMobile.MobilePhoneNumber) {
				if(command!="") { command+=",";}
				command+="MobilePhoneNumber = '"+POut.String(smsFromMobile.MobilePhoneNumber)+"'";
			}
			if(smsFromMobile.MsgPart != oldSmsFromMobile.MsgPart) {
				if(command!="") { command+=",";}
				command+="MsgPart = "+POut.Int(smsFromMobile.MsgPart)+"";
			}
			if(smsFromMobile.MsgTotal != oldSmsFromMobile.MsgTotal) {
				if(command!="") { command+=",";}
				command+="MsgTotal = "+POut.Int(smsFromMobile.MsgTotal)+"";
			}
			if(smsFromMobile.MsgRefID != oldSmsFromMobile.MsgRefID) {
				if(command!="") { command+=",";}
				command+="MsgRefID = '"+POut.String(smsFromMobile.MsgRefID)+"'";
			}
			if(smsFromMobile.SmsStatus != oldSmsFromMobile.SmsStatus) {
				if(command!="") { command+=",";}
				command+="SmsStatus = "+POut.Int   ((int)smsFromMobile.SmsStatus)+"";
			}
			if(smsFromMobile.Flags != oldSmsFromMobile.Flags) {
				if(command!="") { command+=",";}
				command+="Flags = '"+POut.String(smsFromMobile.Flags)+"'";
			}
			if(smsFromMobile.IsHidden != oldSmsFromMobile.IsHidden) {
				if(command!="") { command+=",";}
				command+="IsHidden = "+POut.Bool(smsFromMobile.IsHidden)+"";
			}
			if(smsFromMobile.MatchCount != oldSmsFromMobile.MatchCount) {
				if(command!="") { command+=",";}
				command+="MatchCount = "+POut.Int(smsFromMobile.MatchCount)+"";
			}
			if(smsFromMobile.GuidMessage != oldSmsFromMobile.GuidMessage) {
				if(command!="") { command+=",";}
				command+="GuidMessage = '"+POut.String(smsFromMobile.GuidMessage)+"'";
			}
			//SecDateTEdit can only be set by MySQL
			if(command=="") {
				return false;
			}
			if(smsFromMobile.MsgText==null) {
				smsFromMobile.MsgText="";
			}
			OdSqlParameter paramMsgText=new OdSqlParameter("paramMsgText",OdDbType.Text,POut.StringNote(smsFromMobile.MsgText));
			command="UPDATE smsfrommobile SET "+command
				+" WHERE SmsFromMobileNum = "+POut.Long(smsFromMobile.SmsFromMobileNum);
			Db.NonQ(command,paramMsgText);
			return true;
		}

		///<summary>Returns true if Update(SmsFromMobile,SmsFromMobile) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(SmsFromMobile smsFromMobile,SmsFromMobile oldSmsFromMobile) {
			if(smsFromMobile.PatNum != oldSmsFromMobile.PatNum) {
				return true;
			}
			if(smsFromMobile.ClinicNum != oldSmsFromMobile.ClinicNum) {
				return true;
			}
			if(smsFromMobile.CommlogNum != oldSmsFromMobile.CommlogNum) {
				return true;
			}
			if(smsFromMobile.MsgText != oldSmsFromMobile.MsgText) {
				return true;
			}
			if(smsFromMobile.DateTimeReceived != oldSmsFromMobile.DateTimeReceived) {
				return true;
			}
			if(smsFromMobile.SmsPhoneNumber != oldSmsFromMobile.SmsPhoneNumber) {
				return true;
			}
			if(smsFromMobile.MobilePhoneNumber != oldSmsFromMobile.MobilePhoneNumber) {
				return true;
			}
			if(smsFromMobile.MsgPart != oldSmsFromMobile.MsgPart) {
				return true;
			}
			if(smsFromMobile.MsgTotal != oldSmsFromMobile.MsgTotal) {
				return true;
			}
			if(smsFromMobile.MsgRefID != oldSmsFromMobile.MsgRefID) {
				return true;
			}
			if(smsFromMobile.SmsStatus != oldSmsFromMobile.SmsStatus) {
				return true;
			}
			if(smsFromMobile.Flags != oldSmsFromMobile.Flags) {
				return true;
			}
			if(smsFromMobile.IsHidden != oldSmsFromMobile.IsHidden) {
				return true;
			}
			if(smsFromMobile.MatchCount != oldSmsFromMobile.MatchCount) {
				return true;
			}
			if(smsFromMobile.GuidMessage != oldSmsFromMobile.GuidMessage) {
				return true;
			}
			//SecDateTEdit can only be set by MySQL
			return false;
		}

		///<summary>Deletes one SmsFromMobile from the database.</summary>
		public static void Delete(long smsFromMobileNum) {
			string command="DELETE FROM smsfrommobile "
				+"WHERE SmsFromMobileNum = "+POut.Long(smsFromMobileNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many SmsFromMobiles from the database.</summary>
		public static void DeleteMany(List<long> listSmsFromMobileNums) {
			if(listSmsFromMobileNums==null || listSmsFromMobileNums.Count==0) {
				return;
			}
			string command="DELETE FROM smsfrommobile "
				+"WHERE SmsFromMobileNum IN("+string.Join(",",listSmsFromMobileNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}