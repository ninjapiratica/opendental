//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class FlowDefLinkCrud {
		///<summary>Gets one FlowDefLink object from the database using the primary key.  Returns null if not found.</summary>
		public static FlowDefLink SelectOne(long flowDefLinkNum) {
			string command="SELECT * FROM flowdeflink "
				+"WHERE FlowDefLinkNum = "+POut.Long(flowDefLinkNum);
			List<FlowDefLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one FlowDefLink object from the database using a query.</summary>
		public static FlowDefLink SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<FlowDefLink> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of FlowDefLink objects from the database using a query.</summary>
		public static List<FlowDefLink> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<FlowDefLink> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<FlowDefLink> TableToList(DataTable table) {
			List<FlowDefLink> retVal=new List<FlowDefLink>();
			FlowDefLink flowDefLink;
			foreach(DataRow row in table.Rows) {
				flowDefLink=new FlowDefLink();
				flowDefLink.FlowDefLinkNum= PIn.Long  (row["FlowDefLinkNum"].ToString());
				flowDefLink.FlowDefNum    = PIn.Long  (row["FlowDefNum"].ToString());
				flowDefLink.Fkey          = PIn.Long  (row["Fkey"].ToString());
				flowDefLink.FlowType      = (OpenDentBusiness.EnumFlowType)PIn.Int(row["FlowType"].ToString());
				retVal.Add(flowDefLink);
			}
			return retVal;
		}

		///<summary>Converts a list of FlowDefLink into a DataTable.</summary>
		public static DataTable ListToTable(List<FlowDefLink> listFlowDefLinks,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="FlowDefLink";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("FlowDefLinkNum");
			table.Columns.Add("FlowDefNum");
			table.Columns.Add("Fkey");
			table.Columns.Add("FlowType");
			foreach(FlowDefLink flowDefLink in listFlowDefLinks) {
				table.Rows.Add(new object[] {
					POut.Long  (flowDefLink.FlowDefLinkNum),
					POut.Long  (flowDefLink.FlowDefNum),
					POut.Long  (flowDefLink.Fkey),
					POut.Int   ((int)flowDefLink.FlowType),
				});
			}
			return table;
		}

		///<summary>Inserts one FlowDefLink into the database.  Returns the new priKey.</summary>
		public static long Insert(FlowDefLink flowDefLink) {
			return Insert(flowDefLink,false);
		}

		///<summary>Inserts one FlowDefLink into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(FlowDefLink flowDefLink,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				flowDefLink.FlowDefLinkNum=ReplicationServers.GetKey("flowdeflink","FlowDefLinkNum");
			}
			string command="INSERT INTO flowdeflink (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="FlowDefLinkNum,";
			}
			command+="FlowDefNum,Fkey,FlowType) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(flowDefLink.FlowDefLinkNum)+",";
			}
			command+=
				     POut.Long  (flowDefLink.FlowDefNum)+","
				+    POut.Long  (flowDefLink.Fkey)+","
				+    POut.Int   ((int)flowDefLink.FlowType)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				flowDefLink.FlowDefLinkNum=Db.NonQ(command,true,"FlowDefLinkNum","flowDefLink");
			}
			return flowDefLink.FlowDefLinkNum;
		}

		///<summary>Inserts one FlowDefLink into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(FlowDefLink flowDefLink) {
			return InsertNoCache(flowDefLink,false);
		}

		///<summary>Inserts one FlowDefLink into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(FlowDefLink flowDefLink,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO flowdeflink (";
			if(!useExistingPK && isRandomKeys) {
				flowDefLink.FlowDefLinkNum=ReplicationServers.GetKeyNoCache("flowdeflink","FlowDefLinkNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="FlowDefLinkNum,";
			}
			command+="FlowDefNum,Fkey,FlowType) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(flowDefLink.FlowDefLinkNum)+",";
			}
			command+=
				     POut.Long  (flowDefLink.FlowDefNum)+","
				+    POut.Long  (flowDefLink.Fkey)+","
				+    POut.Int   ((int)flowDefLink.FlowType)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				flowDefLink.FlowDefLinkNum=Db.NonQ(command,true,"FlowDefLinkNum","flowDefLink");
			}
			return flowDefLink.FlowDefLinkNum;
		}

		///<summary>Updates one FlowDefLink in the database.</summary>
		public static void Update(FlowDefLink flowDefLink) {
			string command="UPDATE flowdeflink SET "
				+"FlowDefNum    =  "+POut.Long  (flowDefLink.FlowDefNum)+", "
				+"Fkey          =  "+POut.Long  (flowDefLink.Fkey)+", "
				+"FlowType      =  "+POut.Int   ((int)flowDefLink.FlowType)+" "
				+"WHERE FlowDefLinkNum = "+POut.Long(flowDefLink.FlowDefLinkNum);
			Db.NonQ(command);
		}

		///<summary>Updates one FlowDefLink in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(FlowDefLink flowDefLink,FlowDefLink oldFlowDefLink) {
			string command="";
			if(flowDefLink.FlowDefNum != oldFlowDefLink.FlowDefNum) {
				if(command!="") { command+=",";}
				command+="FlowDefNum = "+POut.Long(flowDefLink.FlowDefNum)+"";
			}
			if(flowDefLink.Fkey != oldFlowDefLink.Fkey) {
				if(command!="") { command+=",";}
				command+="Fkey = "+POut.Long(flowDefLink.Fkey)+"";
			}
			if(flowDefLink.FlowType != oldFlowDefLink.FlowType) {
				if(command!="") { command+=",";}
				command+="FlowType = "+POut.Int   ((int)flowDefLink.FlowType)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE flowdeflink SET "+command
				+" WHERE FlowDefLinkNum = "+POut.Long(flowDefLink.FlowDefLinkNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(FlowDefLink,FlowDefLink) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(FlowDefLink flowDefLink,FlowDefLink oldFlowDefLink) {
			if(flowDefLink.FlowDefNum != oldFlowDefLink.FlowDefNum) {
				return true;
			}
			if(flowDefLink.Fkey != oldFlowDefLink.Fkey) {
				return true;
			}
			if(flowDefLink.FlowType != oldFlowDefLink.FlowType) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one FlowDefLink from the database.</summary>
		public static void Delete(long flowDefLinkNum) {
			string command="DELETE FROM flowdeflink "
				+"WHERE FlowDefLinkNum = "+POut.Long(flowDefLinkNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many FlowDefLinks from the database.</summary>
		public static void DeleteMany(List<long> listFlowDefLinkNums) {
			if(listFlowDefLinkNums==null || listFlowDefLinkNums.Count==0) {
				return;
			}
			string command="DELETE FROM flowdeflink "
				+"WHERE FlowDefLinkNum IN("+string.Join(",",listFlowDefLinkNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}
	}
}