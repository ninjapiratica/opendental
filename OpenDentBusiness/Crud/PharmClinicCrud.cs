//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class PharmClinicCrud {
		///<summary>Gets one PharmClinic object from the database using the primary key.  Returns null if not found.</summary>
		public static PharmClinic SelectOne(long pharmClinicNum) {
			string command="SELECT * FROM pharmclinic "
				+"WHERE PharmClinicNum = "+POut.Long(pharmClinicNum);
			List<PharmClinic> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one PharmClinic object from the database using a query.</summary>
		public static PharmClinic SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PharmClinic> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of PharmClinic objects from the database using a query.</summary>
		public static List<PharmClinic> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<PharmClinic> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<PharmClinic> TableToList(DataTable table) {
			List<PharmClinic> retVal=new List<PharmClinic>();
			PharmClinic pharmClinic;
			foreach(DataRow row in table.Rows) {
				pharmClinic=new PharmClinic();
				pharmClinic.PharmClinicNum= PIn.Long  (row["PharmClinicNum"].ToString());
				pharmClinic.PharmacyNum   = PIn.Long  (row["PharmacyNum"].ToString());
				pharmClinic.ClinicNum     = PIn.Long  (row["ClinicNum"].ToString());
				retVal.Add(pharmClinic);
			}
			return retVal;
		}

		///<summary>Converts a list of PharmClinic into a DataTable.</summary>
		public static DataTable ListToTable(List<PharmClinic> listPharmClinics,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="PharmClinic";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("PharmClinicNum");
			table.Columns.Add("PharmacyNum");
			table.Columns.Add("ClinicNum");
			foreach(PharmClinic pharmClinic in listPharmClinics) {
				table.Rows.Add(new object[] {
					POut.Long  (pharmClinic.PharmClinicNum),
					POut.Long  (pharmClinic.PharmacyNum),
					POut.Long  (pharmClinic.ClinicNum),
				});
			}
			return table;
		}

		///<summary>Inserts one PharmClinic into the database.  Returns the new priKey.</summary>
		public static long Insert(PharmClinic pharmClinic) {
			return Insert(pharmClinic,false);
		}

		///<summary>Inserts one PharmClinic into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(PharmClinic pharmClinic,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				pharmClinic.PharmClinicNum=ReplicationServers.GetKey("pharmclinic","PharmClinicNum");
			}
			string command="INSERT INTO pharmclinic (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="PharmClinicNum,";
			}
			command+="PharmacyNum,ClinicNum) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(pharmClinic.PharmClinicNum)+",";
			}
			command+=
				     POut.Long  (pharmClinic.PharmacyNum)+","
				+    POut.Long  (pharmClinic.ClinicNum)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				pharmClinic.PharmClinicNum=Db.NonQ(command,true,"PharmClinicNum","pharmClinic");
			}
			return pharmClinic.PharmClinicNum;
		}

		///<summary>Inserts one PharmClinic into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(PharmClinic pharmClinic) {
			return InsertNoCache(pharmClinic,false);
		}

		///<summary>Inserts one PharmClinic into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(PharmClinic pharmClinic,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO pharmclinic (";
			if(!useExistingPK && isRandomKeys) {
				pharmClinic.PharmClinicNum=ReplicationServers.GetKeyNoCache("pharmclinic","PharmClinicNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="PharmClinicNum,";
			}
			command+="PharmacyNum,ClinicNum) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(pharmClinic.PharmClinicNum)+",";
			}
			command+=
				     POut.Long  (pharmClinic.PharmacyNum)+","
				+    POut.Long  (pharmClinic.ClinicNum)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				pharmClinic.PharmClinicNum=Db.NonQ(command,true,"PharmClinicNum","pharmClinic");
			}
			return pharmClinic.PharmClinicNum;
		}

		///<summary>Updates one PharmClinic in the database.</summary>
		public static void Update(PharmClinic pharmClinic) {
			string command="UPDATE pharmclinic SET "
				+"PharmacyNum   =  "+POut.Long  (pharmClinic.PharmacyNum)+", "
				+"ClinicNum     =  "+POut.Long  (pharmClinic.ClinicNum)+" "
				+"WHERE PharmClinicNum = "+POut.Long(pharmClinic.PharmClinicNum);
			Db.NonQ(command);
		}

		///<summary>Updates one PharmClinic in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(PharmClinic pharmClinic,PharmClinic oldPharmClinic) {
			string command="";
			if(pharmClinic.PharmacyNum != oldPharmClinic.PharmacyNum) {
				if(command!="") { command+=",";}
				command+="PharmacyNum = "+POut.Long(pharmClinic.PharmacyNum)+"";
			}
			if(pharmClinic.ClinicNum != oldPharmClinic.ClinicNum) {
				if(command!="") { command+=",";}
				command+="ClinicNum = "+POut.Long(pharmClinic.ClinicNum)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE pharmclinic SET "+command
				+" WHERE PharmClinicNum = "+POut.Long(pharmClinic.PharmClinicNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(PharmClinic,PharmClinic) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(PharmClinic pharmClinic,PharmClinic oldPharmClinic) {
			if(pharmClinic.PharmacyNum != oldPharmClinic.PharmacyNum) {
				return true;
			}
			if(pharmClinic.ClinicNum != oldPharmClinic.ClinicNum) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one PharmClinic from the database.</summary>
		public static void Delete(long pharmClinicNum) {
			string command="DELETE FROM pharmclinic "
				+"WHERE PharmClinicNum = "+POut.Long(pharmClinicNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many PharmClinics from the database.</summary>
		public static void DeleteMany(List<long> listPharmClinicNums) {
			if(listPharmClinicNums==null || listPharmClinicNums.Count==0) {
				return;
			}
			string command="DELETE FROM pharmclinic "
				+"WHERE PharmClinicNum IN("+string.Join(",",listPharmClinicNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.</summary>
		public static bool Sync(List<PharmClinic> listNew,List<PharmClinic> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<PharmClinic> listIns    =new List<PharmClinic>();
			List<PharmClinic> listUpdNew =new List<PharmClinic>();
			List<PharmClinic> listUpdDB  =new List<PharmClinic>();
			List<PharmClinic> listDel    =new List<PharmClinic>();
			listNew.Sort((PharmClinic x,PharmClinic y) => { return x.PharmClinicNum.CompareTo(y.PharmClinicNum); });
			listDB.Sort((PharmClinic x,PharmClinic y) => { return x.PharmClinicNum.CompareTo(y.PharmClinicNum); });
			int idxNew=0;
			int idxDB=0;
			int rowsUpdatedCount=0;
			PharmClinic fieldNew;
			PharmClinic fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.PharmClinicNum<fieldDB.PharmClinicNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.PharmClinicNum>fieldDB.PharmClinicNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				listUpdNew.Add(fieldNew);
				listUpdDB.Add(fieldDB);
				idxNew++;
				idxDB++;
			}
			//Commit changes to DB
			for(int i=0;i<listIns.Count;i++) {
				Insert(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				if(Update(listUpdNew[i],listUpdDB[i])) {
					rowsUpdatedCount++;
				}
			}
			DeleteMany(listDel.Select(x => x.PharmClinicNum).ToList());
			if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {
				return true;
			}
			return false;
		}

	}
}