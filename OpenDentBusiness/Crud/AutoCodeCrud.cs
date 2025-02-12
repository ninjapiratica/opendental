//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class AutoCodeCrud {
		///<summary>Gets one AutoCode object from the database using the primary key.  Returns null if not found.</summary>
		public static AutoCode SelectOne(long autoCodeNum) {
			string command="SELECT * FROM autocode "
				+"WHERE AutoCodeNum = "+POut.Long(autoCodeNum);
			List<AutoCode> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one AutoCode object from the database using a query.</summary>
		public static AutoCode SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<AutoCode> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of AutoCode objects from the database using a query.</summary>
		public static List<AutoCode> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<AutoCode> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<AutoCode> TableToList(DataTable table) {
			List<AutoCode> retVal=new List<AutoCode>();
			AutoCode autoCode;
			foreach(DataRow row in table.Rows) {
				autoCode=new AutoCode();
				autoCode.AutoCodeNum  = PIn.Long  (row["AutoCodeNum"].ToString());
				autoCode.Description  = PIn.String(row["Description"].ToString());
				autoCode.IsHidden     = PIn.Bool  (row["IsHidden"].ToString());
				autoCode.LessIntrusive= PIn.Bool  (row["LessIntrusive"].ToString());
				retVal.Add(autoCode);
			}
			return retVal;
		}

		///<summary>Converts a list of AutoCode into a DataTable.</summary>
		public static DataTable ListToTable(List<AutoCode> listAutoCodes,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="AutoCode";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("AutoCodeNum");
			table.Columns.Add("Description");
			table.Columns.Add("IsHidden");
			table.Columns.Add("LessIntrusive");
			foreach(AutoCode autoCode in listAutoCodes) {
				table.Rows.Add(new object[] {
					POut.Long  (autoCode.AutoCodeNum),
					            autoCode.Description,
					POut.Bool  (autoCode.IsHidden),
					POut.Bool  (autoCode.LessIntrusive),
				});
			}
			return table;
		}

		///<summary>Inserts one AutoCode into the database.  Returns the new priKey.</summary>
		public static long Insert(AutoCode autoCode) {
			return Insert(autoCode,false);
		}

		///<summary>Inserts one AutoCode into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(AutoCode autoCode,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				autoCode.AutoCodeNum=ReplicationServers.GetKey("autocode","AutoCodeNum");
			}
			string command="INSERT INTO autocode (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="AutoCodeNum,";
			}
			command+="Description,IsHidden,LessIntrusive) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(autoCode.AutoCodeNum)+",";
			}
			command+=
				 "'"+POut.String(autoCode.Description)+"',"
				+    POut.Bool  (autoCode.IsHidden)+","
				+    POut.Bool  (autoCode.LessIntrusive)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				autoCode.AutoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autoCode");
			}
			return autoCode.AutoCodeNum;
		}

		///<summary>Inserts one AutoCode into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(AutoCode autoCode) {
			return InsertNoCache(autoCode,false);
		}

		///<summary>Inserts one AutoCode into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(AutoCode autoCode,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO autocode (";
			if(!useExistingPK && isRandomKeys) {
				autoCode.AutoCodeNum=ReplicationServers.GetKeyNoCache("autocode","AutoCodeNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="AutoCodeNum,";
			}
			command+="Description,IsHidden,LessIntrusive) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(autoCode.AutoCodeNum)+",";
			}
			command+=
				 "'"+POut.String(autoCode.Description)+"',"
				+    POut.Bool  (autoCode.IsHidden)+","
				+    POut.Bool  (autoCode.LessIntrusive)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				autoCode.AutoCodeNum=Db.NonQ(command,true,"AutoCodeNum","autoCode");
			}
			return autoCode.AutoCodeNum;
		}

		///<summary>Updates one AutoCode in the database.</summary>
		public static void Update(AutoCode autoCode) {
			string command="UPDATE autocode SET "
				+"Description  = '"+POut.String(autoCode.Description)+"', "
				+"IsHidden     =  "+POut.Bool  (autoCode.IsHidden)+", "
				+"LessIntrusive=  "+POut.Bool  (autoCode.LessIntrusive)+" "
				+"WHERE AutoCodeNum = "+POut.Long(autoCode.AutoCodeNum);
			Db.NonQ(command);
		}

		///<summary>Updates one AutoCode in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(AutoCode autoCode,AutoCode oldAutoCode) {
			string command="";
			if(autoCode.Description != oldAutoCode.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(autoCode.Description)+"'";
			}
			if(autoCode.IsHidden != oldAutoCode.IsHidden) {
				if(command!="") { command+=",";}
				command+="IsHidden = "+POut.Bool(autoCode.IsHidden)+"";
			}
			if(autoCode.LessIntrusive != oldAutoCode.LessIntrusive) {
				if(command!="") { command+=",";}
				command+="LessIntrusive = "+POut.Bool(autoCode.LessIntrusive)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE autocode SET "+command
				+" WHERE AutoCodeNum = "+POut.Long(autoCode.AutoCodeNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(AutoCode,AutoCode) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(AutoCode autoCode,AutoCode oldAutoCode) {
			if(autoCode.Description != oldAutoCode.Description) {
				return true;
			}
			if(autoCode.IsHidden != oldAutoCode.IsHidden) {
				return true;
			}
			if(autoCode.LessIntrusive != oldAutoCode.LessIntrusive) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one AutoCode from the database.</summary>
		public static void Delete(long autoCodeNum) {
			string command="DELETE FROM autocode "
				+"WHERE AutoCodeNum = "+POut.Long(autoCodeNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many AutoCodes from the database.</summary>
		public static void DeleteMany(List<long> listAutoCodeNums) {
			if(listAutoCodeNums==null || listAutoCodeNums.Count==0) {
				return;
			}
			string command="DELETE FROM autocode "
				+"WHERE AutoCodeNum IN("+string.Join(",",listAutoCodeNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}