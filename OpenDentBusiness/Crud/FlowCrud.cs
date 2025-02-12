//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class FlowCrud {
		///<summary>Gets one Flow object from the database using the primary key.  Returns null if not found.</summary>
		public static Flow SelectOne(long flowNum) {
			string command="SELECT * FROM flow "
				+"WHERE FlowNum = "+POut.Long(flowNum);
			List<Flow> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Flow object from the database using a query.</summary>
		public static Flow SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Flow> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Flow objects from the database using a query.</summary>
		public static List<Flow> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Flow> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Flow> TableToList(DataTable table) {
			List<Flow> retVal=new List<Flow>();
			Flow flow;
			foreach(DataRow row in table.Rows) {
				flow=new Flow();
				flow.FlowNum      = PIn.Long  (row["FlowNum"].ToString());
				flow.Description  = PIn.String(row["Description"].ToString());
				flow.PatNum       = PIn.Long  (row["PatNum"].ToString());
				flow.ClinicNum    = PIn.Long  (row["ClinicNum"].ToString());
				flow.SecDateTEntry= PIn.DateT (row["SecDateTEntry"].ToString());
				flow.IsComplete   = PIn.Bool  (row["IsComplete"].ToString());
				retVal.Add(flow);
			}
			return retVal;
		}

		///<summary>Converts a list of Flow into a DataTable.</summary>
		public static DataTable ListToTable(List<Flow> listFlows,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Flow";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("FlowNum");
			table.Columns.Add("Description");
			table.Columns.Add("PatNum");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("SecDateTEntry");
			table.Columns.Add("IsComplete");
			foreach(Flow flow in listFlows) {
				table.Rows.Add(new object[] {
					POut.Long  (flow.FlowNum),
					            flow.Description,
					POut.Long  (flow.PatNum),
					POut.Long  (flow.ClinicNum),
					POut.DateT (flow.SecDateTEntry,false),
					POut.Bool  (flow.IsComplete),
				});
			}
			return table;
		}

		///<summary>Inserts one Flow into the database.  Returns the new priKey.</summary>
		public static long Insert(Flow flow) {
			return Insert(flow,false);
		}

		///<summary>Inserts one Flow into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Flow flow,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				flow.FlowNum=ReplicationServers.GetKey("flow","FlowNum");
			}
			string command="INSERT INTO flow (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="FlowNum,";
			}
			command+="Description,PatNum,ClinicNum,SecDateTEntry,IsComplete) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(flow.FlowNum)+",";
			}
			command+=
				 "'"+POut.String(flow.Description)+"',"
				+    POut.Long  (flow.PatNum)+","
				+    POut.Long  (flow.ClinicNum)+","
				+    DbHelper.Now()+","
				+    POut.Bool  (flow.IsComplete)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				flow.FlowNum=Db.NonQ(command,true,"FlowNum","flow");
			}
			return flow.FlowNum;
		}

		///<summary>Inserts one Flow into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Flow flow) {
			return InsertNoCache(flow,false);
		}

		///<summary>Inserts one Flow into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Flow flow,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO flow (";
			if(!useExistingPK && isRandomKeys) {
				flow.FlowNum=ReplicationServers.GetKeyNoCache("flow","FlowNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="FlowNum,";
			}
			command+="Description,PatNum,ClinicNum,SecDateTEntry,IsComplete) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(flow.FlowNum)+",";
			}
			command+=
				 "'"+POut.String(flow.Description)+"',"
				+    POut.Long  (flow.PatNum)+","
				+    POut.Long  (flow.ClinicNum)+","
				+    DbHelper.Now()+","
				+    POut.Bool  (flow.IsComplete)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				flow.FlowNum=Db.NonQ(command,true,"FlowNum","flow");
			}
			return flow.FlowNum;
		}

		///<summary>Updates one Flow in the database.</summary>
		public static void Update(Flow flow) {
			string command="UPDATE flow SET "
				+"Description  = '"+POut.String(flow.Description)+"', "
				+"PatNum       =  "+POut.Long  (flow.PatNum)+", "
				+"ClinicNum    =  "+POut.Long  (flow.ClinicNum)+", "
				//SecDateTEntry not allowed to change
				+"IsComplete   =  "+POut.Bool  (flow.IsComplete)+" "
				+"WHERE FlowNum = "+POut.Long(flow.FlowNum);
			Db.NonQ(command);
		}

		///<summary>Updates one Flow in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Flow flow,Flow oldFlow) {
			string command="";
			if(flow.Description != oldFlow.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(flow.Description)+"'";
			}
			if(flow.PatNum != oldFlow.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(flow.PatNum)+"";
			}
			if(flow.ClinicNum != oldFlow.ClinicNum) {
				if(command!="") { command+=",";}
				command+="ClinicNum = "+POut.Long(flow.ClinicNum)+"";
			}
			//SecDateTEntry not allowed to change
			if(flow.IsComplete != oldFlow.IsComplete) {
				if(command!="") { command+=",";}
				command+="IsComplete = "+POut.Bool(flow.IsComplete)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE flow SET "+command
				+" WHERE FlowNum = "+POut.Long(flow.FlowNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(Flow,Flow) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Flow flow,Flow oldFlow) {
			if(flow.Description != oldFlow.Description) {
				return true;
			}
			if(flow.PatNum != oldFlow.PatNum) {
				return true;
			}
			if(flow.ClinicNum != oldFlow.ClinicNum) {
				return true;
			}
			//SecDateTEntry not allowed to change
			if(flow.IsComplete != oldFlow.IsComplete) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one Flow from the database.</summary>
		public static void Delete(long flowNum) {
			string command="DELETE FROM flow "
				+"WHERE FlowNum = "+POut.Long(flowNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many Flows from the database.</summary>
		public static void DeleteMany(List<long> listFlowNums) {
			if(listFlowNums==null || listFlowNums.Count==0) {
				return;
			}
			string command="DELETE FROM flow "
				+"WHERE FlowNum IN("+string.Join(",",listFlowNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}