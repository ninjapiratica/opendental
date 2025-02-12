//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class PayPeriodCrud {
		///<summary>Gets one PayPeriod object from the database using the primary key.  Returns null if not found.</summary>
		public static PayPeriod SelectOne(long payPeriodNum) {
			string command="SELECT * FROM payperiod "
				+"WHERE PayPeriodNum = "+POut.Long(payPeriodNum);
			List<PayPeriod> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one PayPeriod object from the database using a query.</summary>
		public static PayPeriod SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PayPeriod> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of PayPeriod objects from the database using a query.</summary>
		public static List<PayPeriod> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PayPeriod> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<PayPeriod> TableToList(DataTable table) {
			List<PayPeriod> retVal=new List<PayPeriod>();
			PayPeriod payPeriod;
			foreach(DataRow row in table.Rows) {
				payPeriod=new PayPeriod();
				payPeriod.PayPeriodNum= PIn.Long  (row["PayPeriodNum"].ToString());
				payPeriod.DateStart   = PIn.Date  (row["DateStart"].ToString());
				payPeriod.DateStop    = PIn.Date  (row["DateStop"].ToString());
				payPeriod.DatePaycheck= PIn.Date  (row["DatePaycheck"].ToString());
				retVal.Add(payPeriod);
			}
			return retVal;
		}

		///<summary>Converts a list of PayPeriod into a DataTable.</summary>
		public static DataTable ListToTable(List<PayPeriod> listPayPeriods,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="PayPeriod";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("PayPeriodNum");
			table.Columns.Add("DateStart");
			table.Columns.Add("DateStop");
			table.Columns.Add("DatePaycheck");
			foreach(PayPeriod payPeriod in listPayPeriods) {
				table.Rows.Add(new object[] {
					POut.Long  (payPeriod.PayPeriodNum),
					POut.DateT (payPeriod.DateStart,false),
					POut.DateT (payPeriod.DateStop,false),
					POut.DateT (payPeriod.DatePaycheck,false),
				});
			}
			return table;
		}

		///<summary>Inserts one PayPeriod into the database.  Returns the new priKey.</summary>
		public static long Insert(PayPeriod payPeriod) {
			return Insert(payPeriod,false);
		}

		///<summary>Inserts one PayPeriod into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(PayPeriod payPeriod,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				payPeriod.PayPeriodNum=ReplicationServers.GetKey("payperiod","PayPeriodNum");
			}
			string command="INSERT INTO payperiod (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="PayPeriodNum,";
			}
			command+="DateStart,DateStop,DatePaycheck) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(payPeriod.PayPeriodNum)+",";
			}
			command+=
				     POut.Date  (payPeriod.DateStart)+","
				+    POut.Date  (payPeriod.DateStop)+","
				+    POut.Date  (payPeriod.DatePaycheck)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				payPeriod.PayPeriodNum=Db.NonQ(command,true,"PayPeriodNum","payPeriod");
			}
			return payPeriod.PayPeriodNum;
		}

		///<summary>Inserts one PayPeriod into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(PayPeriod payPeriod) {
			return InsertNoCache(payPeriod,false);
		}

		///<summary>Inserts one PayPeriod into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(PayPeriod payPeriod,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO payperiod (";
			if(!useExistingPK && isRandomKeys) {
				payPeriod.PayPeriodNum=ReplicationServers.GetKeyNoCache("payperiod","PayPeriodNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="PayPeriodNum,";
			}
			command+="DateStart,DateStop,DatePaycheck) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(payPeriod.PayPeriodNum)+",";
			}
			command+=
				     POut.Date  (payPeriod.DateStart)+","
				+    POut.Date  (payPeriod.DateStop)+","
				+    POut.Date  (payPeriod.DatePaycheck)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				payPeriod.PayPeriodNum=Db.NonQ(command,true,"PayPeriodNum","payPeriod");
			}
			return payPeriod.PayPeriodNum;
		}

		///<summary>Updates one PayPeriod in the database.</summary>
		public static void Update(PayPeriod payPeriod) {
			string command="UPDATE payperiod SET "
				+"DateStart   =  "+POut.Date  (payPeriod.DateStart)+", "
				+"DateStop    =  "+POut.Date  (payPeriod.DateStop)+", "
				+"DatePaycheck=  "+POut.Date  (payPeriod.DatePaycheck)+" "
				+"WHERE PayPeriodNum = "+POut.Long(payPeriod.PayPeriodNum);
			Db.NonQ(command);
		}

		///<summary>Updates one PayPeriod in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(PayPeriod payPeriod,PayPeriod oldPayPeriod) {
			string command="";
			if(payPeriod.DateStart.Date != oldPayPeriod.DateStart.Date) {
				if(command!="") { command+=",";}
				command+="DateStart = "+POut.Date(payPeriod.DateStart)+"";
			}
			if(payPeriod.DateStop.Date != oldPayPeriod.DateStop.Date) {
				if(command!="") { command+=",";}
				command+="DateStop = "+POut.Date(payPeriod.DateStop)+"";
			}
			if(payPeriod.DatePaycheck.Date != oldPayPeriod.DatePaycheck.Date) {
				if(command!="") { command+=",";}
				command+="DatePaycheck = "+POut.Date(payPeriod.DatePaycheck)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE payperiod SET "+command
				+" WHERE PayPeriodNum = "+POut.Long(payPeriod.PayPeriodNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(PayPeriod,PayPeriod) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(PayPeriod payPeriod,PayPeriod oldPayPeriod) {
			if(payPeriod.DateStart.Date != oldPayPeriod.DateStart.Date) {
				return true;
			}
			if(payPeriod.DateStop.Date != oldPayPeriod.DateStop.Date) {
				return true;
			}
			if(payPeriod.DatePaycheck.Date != oldPayPeriod.DatePaycheck.Date) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one PayPeriod from the database.</summary>
		public static void Delete(long payPeriodNum) {
			string command="DELETE FROM payperiod "
				+"WHERE PayPeriodNum = "+POut.Long(payPeriodNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many PayPeriods from the database.</summary>
		public static void DeleteMany(List<long> listPayPeriodNums) {
			if(listPayPeriodNums==null || listPayPeriodNums.Count==0) {
				return;
			}
			string command="DELETE FROM payperiod "
				+"WHERE PayPeriodNum IN("+string.Join(",",listPayPeriodNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}