//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Crud{
	public class RecallCrud {
		///<summary>Gets one Recall object from the database using the primary key.  Returns null if not found.</summary>
		public static Recall SelectOne(long recallNum) {
			string command="SELECT * FROM recall "
				+"WHERE RecallNum = "+POut.Long(recallNum);
			List<Recall> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Recall object from the database using a query.</summary>
		public static Recall SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Recall> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Recall objects from the database using a query.</summary>
		public static List<Recall> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Recall> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Recall> TableToList(DataTable table) {
			List<Recall> retVal=new List<Recall>();
			Recall recall;
			foreach(DataRow row in table.Rows) {
				recall=new Recall();
				recall.RecallNum          = PIn.Long  (row["RecallNum"].ToString());
				recall.PatNum             = PIn.Long  (row["PatNum"].ToString());
				recall.DateDueCalc        = PIn.Date  (row["DateDueCalc"].ToString());
				recall.DateDue            = PIn.Date  (row["DateDue"].ToString());
				recall.DatePrevious       = PIn.Date  (row["DatePrevious"].ToString());
				recall.RecallInterval     = new Interval(PIn.Int(row["RecallInterval"].ToString()));
				recall.RecallStatus       = PIn.Long  (row["RecallStatus"].ToString());
				recall.Note               = PIn.String(row["Note"].ToString());
				recall.IsDisabled         = PIn.Bool  (row["IsDisabled"].ToString());
				recall.DateTStamp         = PIn.DateT (row["DateTStamp"].ToString());
				recall.RecallTypeNum      = PIn.Long  (row["RecallTypeNum"].ToString());
				recall.DisableUntilBalance= PIn.Double(row["DisableUntilBalance"].ToString());
				recall.DisableUntilDate   = PIn.Date  (row["DisableUntilDate"].ToString());
				recall.DateScheduled      = PIn.Date  (row["DateScheduled"].ToString());
				recall.Priority           = (OpenDentBusiness.RecallPriority)PIn.Int(row["Priority"].ToString());
				recall.TimePatternOverride= PIn.String(row["TimePatternOverride"].ToString());
				retVal.Add(recall);
			}
			return retVal;
		}

		///<summary>Converts a list of Recall into a DataTable.</summary>
		public static DataTable ListToTable(List<Recall> listRecalls,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Recall";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("RecallNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("DateDueCalc");
			table.Columns.Add("DateDue");
			table.Columns.Add("DatePrevious");
			table.Columns.Add("RecallInterval");
			table.Columns.Add("RecallStatus");
			table.Columns.Add("Note");
			table.Columns.Add("IsDisabled");
			table.Columns.Add("DateTStamp");
			table.Columns.Add("RecallTypeNum");
			table.Columns.Add("DisableUntilBalance");
			table.Columns.Add("DisableUntilDate");
			table.Columns.Add("DateScheduled");
			table.Columns.Add("Priority");
			table.Columns.Add("TimePatternOverride");
			foreach(Recall recall in listRecalls) {
				table.Rows.Add(new object[] {
					POut.Long  (recall.RecallNum),
					POut.Long  (recall.PatNum),
					POut.DateT (recall.DateDueCalc,false),
					POut.DateT (recall.DateDue,false),
					POut.DateT (recall.DatePrevious,false),
					POut.Int   (recall.RecallInterval.ToInt()),
					POut.Long  (recall.RecallStatus),
					            recall.Note,
					POut.Bool  (recall.IsDisabled),
					POut.DateT (recall.DateTStamp,false),
					POut.Long  (recall.RecallTypeNum),
					POut.Double(recall.DisableUntilBalance),
					POut.DateT (recall.DisableUntilDate,false),
					POut.DateT (recall.DateScheduled,false),
					POut.Int   ((int)recall.Priority),
					            recall.TimePatternOverride,
				});
			}
			return table;
		}

		///<summary>Inserts one Recall into the database.  Returns the new priKey.</summary>
		public static long Insert(Recall recall) {
			return Insert(recall,false);
		}

		///<summary>Inserts one Recall into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Recall recall,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				recall.RecallNum=ReplicationServers.GetKey("recall","RecallNum");
			}
			string command="INSERT INTO recall (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="RecallNum,";
			}
			command+="PatNum,DateDueCalc,DateDue,DatePrevious,RecallInterval,RecallStatus,Note,IsDisabled,RecallTypeNum,DisableUntilBalance,DisableUntilDate,DateScheduled,Priority,TimePatternOverride) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(recall.RecallNum)+",";
			}
			command+=
				     POut.Long  (recall.PatNum)+","
				+    POut.Date  (recall.DateDueCalc)+","
				+    POut.Date  (recall.DateDue)+","
				+    POut.Date  (recall.DatePrevious)+","
				+    POut.Int   (recall.RecallInterval.ToInt())+","
				+    POut.Long  (recall.RecallStatus)+","
				+    DbHelper.ParamChar+"paramNote,"
				+    POut.Bool  (recall.IsDisabled)+","
				//DateTStamp can only be set by MySQL
				+    POut.Long  (recall.RecallTypeNum)+","
				+		 POut.Double(recall.DisableUntilBalance)+","
				+    POut.Date  (recall.DisableUntilDate)+","
				+    POut.Date  (recall.DateScheduled)+","
				+    POut.Int   ((int)recall.Priority)+","
				+"'"+POut.String(recall.TimePatternOverride)+"')";
			if(recall.Note==null) {
				recall.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(recall.Note));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramNote);
			}
			else {
				recall.RecallNum=Db.NonQ(command,true,"RecallNum","recall",paramNote);
			}
			return recall.RecallNum;
		}

		///<summary>Inserts many Recalls into the database.</summary>
		public static void InsertMany(List<Recall> listRecalls) {
			InsertMany(listRecalls,false);
		}

		///<summary>Inserts many Recalls into the database.  Provides option to use the existing priKey.</summary>
		public static void InsertMany(List<Recall> listRecalls,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				foreach(Recall recall in listRecalls) {
					Insert(recall);
				}
			}
			else {
				StringBuilder sbCommands=null;
				int index=0;
				int countRows=0;
				while(index < listRecalls.Count) {
					Recall recall=listRecalls[index];
					StringBuilder sbRow=new StringBuilder("(");
					bool hasComma=false;
					if(sbCommands==null) {
						sbCommands=new StringBuilder();
						sbCommands.Append("INSERT INTO recall (");
						if(useExistingPK) {
							sbCommands.Append("RecallNum,");
						}
						sbCommands.Append("PatNum,DateDueCalc,DateDue,DatePrevious,RecallInterval,RecallStatus,Note,IsDisabled,RecallTypeNum,DisableUntilBalance,DisableUntilDate,DateScheduled,Priority,TimePatternOverride) VALUES ");
						countRows=0;
					}
					else {
						hasComma=true;
					}
					if(useExistingPK) {
						sbRow.Append(POut.Long(recall.RecallNum)); sbRow.Append(",");
					}
					sbRow.Append(POut.Long(recall.PatNum)); sbRow.Append(",");
					sbRow.Append(POut.Date(recall.DateDueCalc)); sbRow.Append(",");
					sbRow.Append(POut.Date(recall.DateDue)); sbRow.Append(",");
					sbRow.Append(POut.Date(recall.DatePrevious)); sbRow.Append(",");
					sbRow.Append(POut.Int(recall.RecallInterval.ToInt())); sbRow.Append(",");
					sbRow.Append(POut.Long(recall.RecallStatus)); sbRow.Append(",");
					sbRow.Append("'"+POut.String(recall.Note)+"'"); sbRow.Append(",");
					sbRow.Append(POut.Bool(recall.IsDisabled)); sbRow.Append(",");
					//DateTStamp can only be set by MySQL
					sbRow.Append(POut.Long(recall.RecallTypeNum)); sbRow.Append(",");
					sbRow.Append(POut.Double(recall.DisableUntilBalance)); sbRow.Append(",");
					sbRow.Append(POut.Date(recall.DisableUntilDate)); sbRow.Append(",");
					sbRow.Append(POut.Date(recall.DateScheduled)); sbRow.Append(",");
					sbRow.Append(POut.Int((int)recall.Priority)); sbRow.Append(",");
					sbRow.Append("'"+POut.String(recall.TimePatternOverride)+"'"); sbRow.Append(")");
					if(sbCommands.Length+sbRow.Length+1 > TableBase.MaxAllowedPacketCount && countRows > 0) {
						Db.NonQ(sbCommands.ToString());
						sbCommands=null;
					}
					else {
						if(hasComma) {
							sbCommands.Append(",");
						}
						sbCommands.Append(sbRow.ToString());
						countRows++;
						if(index==listRecalls.Count-1) {
							Db.NonQ(sbCommands.ToString());
						}
						index++;
					}
				}
			}
		}

		///<summary>Inserts one Recall into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Recall recall) {
			return InsertNoCache(recall,false);
		}

		///<summary>Inserts one Recall into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Recall recall,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO recall (";
			if(!useExistingPK && isRandomKeys) {
				recall.RecallNum=ReplicationServers.GetKeyNoCache("recall","RecallNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="RecallNum,";
			}
			command+="PatNum,DateDueCalc,DateDue,DatePrevious,RecallInterval,RecallStatus,Note,IsDisabled,RecallTypeNum,DisableUntilBalance,DisableUntilDate,DateScheduled,Priority,TimePatternOverride) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(recall.RecallNum)+",";
			}
			command+=
				     POut.Long  (recall.PatNum)+","
				+    POut.Date  (recall.DateDueCalc)+","
				+    POut.Date  (recall.DateDue)+","
				+    POut.Date  (recall.DatePrevious)+","
				+    POut.Int   (recall.RecallInterval.ToInt())+","
				+    POut.Long  (recall.RecallStatus)+","
				+    DbHelper.ParamChar+"paramNote,"
				+    POut.Bool  (recall.IsDisabled)+","
				//DateTStamp can only be set by MySQL
				+    POut.Long  (recall.RecallTypeNum)+","
				+	   POut.Double(recall.DisableUntilBalance)+","
				+    POut.Date  (recall.DisableUntilDate)+","
				+    POut.Date  (recall.DateScheduled)+","
				+    POut.Int   ((int)recall.Priority)+","
				+"'"+POut.String(recall.TimePatternOverride)+"')";
			if(recall.Note==null) {
				recall.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(recall.Note));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramNote);
			}
			else {
				recall.RecallNum=Db.NonQ(command,true,"RecallNum","recall",paramNote);
			}
			return recall.RecallNum;
		}

		///<summary>Updates one Recall in the database.</summary>
		public static void Update(Recall recall) {
			string command="UPDATE recall SET "
				+"PatNum             =  "+POut.Long  (recall.PatNum)+", "
				+"DateDueCalc        =  "+POut.Date  (recall.DateDueCalc)+", "
				+"DateDue            =  "+POut.Date  (recall.DateDue)+", "
				+"DatePrevious       =  "+POut.Date  (recall.DatePrevious)+", "
				+"RecallInterval     =  "+POut.Int   (recall.RecallInterval.ToInt())+", "
				+"RecallStatus       =  "+POut.Long  (recall.RecallStatus)+", "
				+"Note               =  "+DbHelper.ParamChar+"paramNote, "
				+"IsDisabled         =  "+POut.Bool  (recall.IsDisabled)+", "
				//DateTStamp can only be set by MySQL
				+"RecallTypeNum      =  "+POut.Long  (recall.RecallTypeNum)+", "
				+"DisableUntilBalance=  "+POut.Double(recall.DisableUntilBalance)+", "
				+"DisableUntilDate   =  "+POut.Date  (recall.DisableUntilDate)+", "
				+"DateScheduled      =  "+POut.Date  (recall.DateScheduled)+", "
				+"Priority           =  "+POut.Int   ((int)recall.Priority)+", "
				+"TimePatternOverride= '"+POut.String(recall.TimePatternOverride)+"' "
				+"WHERE RecallNum = "+POut.Long(recall.RecallNum);
			if(recall.Note==null) {
				recall.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(recall.Note));
			Db.NonQ(command,paramNote);
		}

		///<summary>Updates one Recall in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Recall recall,Recall oldRecall) {
			string command="";
			if(recall.PatNum != oldRecall.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(recall.PatNum)+"";
			}
			if(recall.DateDueCalc.Date != oldRecall.DateDueCalc.Date) {
				if(command!="") { command+=",";}
				command+="DateDueCalc = "+POut.Date(recall.DateDueCalc)+"";
			}
			if(recall.DateDue.Date != oldRecall.DateDue.Date) {
				if(command!="") { command+=",";}
				command+="DateDue = "+POut.Date(recall.DateDue)+"";
			}
			if(recall.DatePrevious.Date != oldRecall.DatePrevious.Date) {
				if(command!="") { command+=",";}
				command+="DatePrevious = "+POut.Date(recall.DatePrevious)+"";
			}
			if(recall.RecallInterval != oldRecall.RecallInterval) {
				if(command!="") { command+=",";}
				command+="RecallInterval = "+POut.Int(recall.RecallInterval.ToInt())+"";
			}
			if(recall.RecallStatus != oldRecall.RecallStatus) {
				if(command!="") { command+=",";}
				command+="RecallStatus = "+POut.Long(recall.RecallStatus)+"";
			}
			if(recall.Note != oldRecall.Note) {
				if(command!="") { command+=",";}
				command+="Note = "+DbHelper.ParamChar+"paramNote";
			}
			if(recall.IsDisabled != oldRecall.IsDisabled) {
				if(command!="") { command+=",";}
				command+="IsDisabled = "+POut.Bool(recall.IsDisabled)+"";
			}
			//DateTStamp can only be set by MySQL
			if(recall.RecallTypeNum != oldRecall.RecallTypeNum) {
				if(command!="") { command+=",";}
				command+="RecallTypeNum = "+POut.Long(recall.RecallTypeNum)+"";
			}
			if(recall.DisableUntilBalance != oldRecall.DisableUntilBalance) {
				if(command!="") { command+=",";}
				command+="DisableUntilBalance = "+POut.Double(recall.DisableUntilBalance)+"";
			}
			if(recall.DisableUntilDate.Date != oldRecall.DisableUntilDate.Date) {
				if(command!="") { command+=",";}
				command+="DisableUntilDate = "+POut.Date(recall.DisableUntilDate)+"";
			}
			if(recall.DateScheduled.Date != oldRecall.DateScheduled.Date) {
				if(command!="") { command+=",";}
				command+="DateScheduled = "+POut.Date(recall.DateScheduled)+"";
			}
			if(recall.Priority != oldRecall.Priority) {
				if(command!="") { command+=",";}
				command+="Priority = "+POut.Int   ((int)recall.Priority)+"";
			}
			if(recall.TimePatternOverride != oldRecall.TimePatternOverride) {
				if(command!="") { command+=",";}
				command+="TimePatternOverride = '"+POut.String(recall.TimePatternOverride)+"'";
			}
			if(command=="") {
				return false;
			}
			if(recall.Note==null) {
				recall.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(recall.Note));
			command="UPDATE recall SET "+command
				+" WHERE RecallNum = "+POut.Long(recall.RecallNum);
			Db.NonQ(command,paramNote);
			return true;
		}

		///<summary>Returns true if Update(Recall,Recall) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Recall recall,Recall oldRecall) {
			if(recall.PatNum != oldRecall.PatNum) {
				return true;
			}
			if(recall.DateDueCalc.Date != oldRecall.DateDueCalc.Date) {
				return true;
			}
			if(recall.DateDue.Date != oldRecall.DateDue.Date) {
				return true;
			}
			if(recall.DatePrevious.Date != oldRecall.DatePrevious.Date) {
				return true;
			}
			if(recall.RecallInterval != oldRecall.RecallInterval) {
				return true;
			}
			if(recall.RecallStatus != oldRecall.RecallStatus) {
				return true;
			}
			if(recall.Note != oldRecall.Note) {
				return true;
			}
			if(recall.IsDisabled != oldRecall.IsDisabled) {
				return true;
			}
			//DateTStamp can only be set by MySQL
			if(recall.RecallTypeNum != oldRecall.RecallTypeNum) {
				return true;
			}
			if(recall.DisableUntilBalance != oldRecall.DisableUntilBalance) {
				return true;
			}
			if(recall.DisableUntilDate.Date != oldRecall.DisableUntilDate.Date) {
				return true;
			}
			if(recall.DateScheduled.Date != oldRecall.DateScheduled.Date) {
				return true;
			}
			if(recall.Priority != oldRecall.Priority) {
				return true;
			}
			if(recall.TimePatternOverride != oldRecall.TimePatternOverride) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one Recall from the database.</summary>
		public static void Delete(long recallNum) {
			string command="DELETE FROM recall "
				+"WHERE RecallNum = "+POut.Long(recallNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many Recalls from the database.</summary>
		public static void DeleteMany(List<long> listRecallNums) {
			if(listRecallNums==null || listRecallNums.Count==0) {
				return;
			}
			string command="DELETE FROM recall "
				+"WHERE RecallNum IN("+string.Join(",",listRecallNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}