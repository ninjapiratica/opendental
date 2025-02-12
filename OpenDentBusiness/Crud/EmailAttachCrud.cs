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
	public class EmailAttachCrud {
		///<summary>Gets one EmailAttach object from the database using the primary key.  Returns null if not found.</summary>
		public static EmailAttach SelectOne(long emailAttachNum) {
			string command="SELECT * FROM emailattach "
				+"WHERE EmailAttachNum = "+POut.Long(emailAttachNum);
			List<EmailAttach> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one EmailAttach object from the database using a query.</summary>
		public static EmailAttach SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EmailAttach> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of EmailAttach objects from the database using a query.</summary>
		public static List<EmailAttach> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<EmailAttach> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<EmailAttach> TableToList(DataTable table) {
			List<EmailAttach> retVal=new List<EmailAttach>();
			EmailAttach emailAttach;
			foreach(DataRow row in table.Rows) {
				emailAttach=new EmailAttach();
				emailAttach.EmailAttachNum   = PIn.Long  (row["EmailAttachNum"].ToString());
				emailAttach.EmailMessageNum  = PIn.Long  (row["EmailMessageNum"].ToString());
				emailAttach.DisplayedFileName= PIn.String(row["DisplayedFileName"].ToString());
				emailAttach.ActualFileName   = PIn.String(row["ActualFileName"].ToString());
				emailAttach.EmailTemplateNum = PIn.Long  (row["EmailTemplateNum"].ToString());
				retVal.Add(emailAttach);
			}
			return retVal;
		}

		///<summary>Converts a list of EmailAttach into a DataTable.</summary>
		public static DataTable ListToTable(List<EmailAttach> listEmailAttachs,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="EmailAttach";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("EmailAttachNum");
			table.Columns.Add("EmailMessageNum");
			table.Columns.Add("DisplayedFileName");
			table.Columns.Add("ActualFileName");
			table.Columns.Add("EmailTemplateNum");
			foreach(EmailAttach emailAttach in listEmailAttachs) {
				table.Rows.Add(new object[] {
					POut.Long  (emailAttach.EmailAttachNum),
					POut.Long  (emailAttach.EmailMessageNum),
					            emailAttach.DisplayedFileName,
					            emailAttach.ActualFileName,
					POut.Long  (emailAttach.EmailTemplateNum),
				});
			}
			return table;
		}

		///<summary>Inserts one EmailAttach into the database.  Returns the new priKey.</summary>
		public static long Insert(EmailAttach emailAttach) {
			return Insert(emailAttach,false);
		}

		///<summary>Inserts one EmailAttach into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(EmailAttach emailAttach,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				emailAttach.EmailAttachNum=ReplicationServers.GetKey("emailattach","EmailAttachNum");
			}
			string command="INSERT INTO emailattach (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="EmailAttachNum,";
			}
			command+="EmailMessageNum,DisplayedFileName,ActualFileName,EmailTemplateNum) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(emailAttach.EmailAttachNum)+",";
			}
			command+=
				     POut.Long  (emailAttach.EmailMessageNum)+","
				+"'"+POut.String(emailAttach.DisplayedFileName)+"',"
				+"'"+POut.String(emailAttach.ActualFileName)+"',"
				+    POut.Long  (emailAttach.EmailTemplateNum)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				emailAttach.EmailAttachNum=Db.NonQ(command,true,"EmailAttachNum","emailAttach");
			}
			return emailAttach.EmailAttachNum;
		}

		///<summary>Inserts many EmailAttachs into the database.</summary>
		public static void InsertMany(List<EmailAttach> listEmailAttachs) {
			InsertMany(listEmailAttachs,false);
		}

		///<summary>Inserts many EmailAttachs into the database.  Provides option to use the existing priKey.</summary>
		public static void InsertMany(List<EmailAttach> listEmailAttachs,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				foreach(EmailAttach emailAttach in listEmailAttachs) {
					Insert(emailAttach);
				}
			}
			else {
				StringBuilder sbCommands=null;
				int index=0;
				int countRows=0;
				while(index < listEmailAttachs.Count) {
					EmailAttach emailAttach=listEmailAttachs[index];
					StringBuilder sbRow=new StringBuilder("(");
					bool hasComma=false;
					if(sbCommands==null) {
						sbCommands=new StringBuilder();
						sbCommands.Append("INSERT INTO emailattach (");
						if(useExistingPK) {
							sbCommands.Append("EmailAttachNum,");
						}
						sbCommands.Append("EmailMessageNum,DisplayedFileName,ActualFileName,EmailTemplateNum) VALUES ");
						countRows=0;
					}
					else {
						hasComma=true;
					}
					if(useExistingPK) {
						sbRow.Append(POut.Long(emailAttach.EmailAttachNum)); sbRow.Append(",");
					}
					sbRow.Append(POut.Long(emailAttach.EmailMessageNum)); sbRow.Append(",");
					sbRow.Append("'"+POut.String(emailAttach.DisplayedFileName)+"'"); sbRow.Append(",");
					sbRow.Append("'"+POut.String(emailAttach.ActualFileName)+"'"); sbRow.Append(",");
					sbRow.Append(POut.Long(emailAttach.EmailTemplateNum)); sbRow.Append(")");
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
						if(index==listEmailAttachs.Count-1) {
							Db.NonQ(sbCommands.ToString());
						}
						index++;
					}
				}
			}
		}

		///<summary>Inserts one EmailAttach into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(EmailAttach emailAttach) {
			return InsertNoCache(emailAttach,false);
		}

		///<summary>Inserts one EmailAttach into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(EmailAttach emailAttach,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO emailattach (";
			if(!useExistingPK && isRandomKeys) {
				emailAttach.EmailAttachNum=ReplicationServers.GetKeyNoCache("emailattach","EmailAttachNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="EmailAttachNum,";
			}
			command+="EmailMessageNum,DisplayedFileName,ActualFileName,EmailTemplateNum) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(emailAttach.EmailAttachNum)+",";
			}
			command+=
				     POut.Long  (emailAttach.EmailMessageNum)+","
				+"'"+POut.String(emailAttach.DisplayedFileName)+"',"
				+"'"+POut.String(emailAttach.ActualFileName)+"',"
				+    POut.Long  (emailAttach.EmailTemplateNum)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				emailAttach.EmailAttachNum=Db.NonQ(command,true,"EmailAttachNum","emailAttach");
			}
			return emailAttach.EmailAttachNum;
		}

		///<summary>Updates one EmailAttach in the database.</summary>
		public static void Update(EmailAttach emailAttach) {
			string command="UPDATE emailattach SET "
				+"EmailMessageNum  =  "+POut.Long  (emailAttach.EmailMessageNum)+", "
				+"DisplayedFileName= '"+POut.String(emailAttach.DisplayedFileName)+"', "
				+"ActualFileName   = '"+POut.String(emailAttach.ActualFileName)+"', "
				+"EmailTemplateNum =  "+POut.Long  (emailAttach.EmailTemplateNum)+" "
				+"WHERE EmailAttachNum = "+POut.Long(emailAttach.EmailAttachNum);
			Db.NonQ(command);
		}

		///<summary>Updates one EmailAttach in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(EmailAttach emailAttach,EmailAttach oldEmailAttach) {
			string command="";
			if(emailAttach.EmailMessageNum != oldEmailAttach.EmailMessageNum) {
				if(command!="") { command+=",";}
				command+="EmailMessageNum = "+POut.Long(emailAttach.EmailMessageNum)+"";
			}
			if(emailAttach.DisplayedFileName != oldEmailAttach.DisplayedFileName) {
				if(command!="") { command+=",";}
				command+="DisplayedFileName = '"+POut.String(emailAttach.DisplayedFileName)+"'";
			}
			if(emailAttach.ActualFileName != oldEmailAttach.ActualFileName) {
				if(command!="") { command+=",";}
				command+="ActualFileName = '"+POut.String(emailAttach.ActualFileName)+"'";
			}
			if(emailAttach.EmailTemplateNum != oldEmailAttach.EmailTemplateNum) {
				if(command!="") { command+=",";}
				command+="EmailTemplateNum = "+POut.Long(emailAttach.EmailTemplateNum)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE emailattach SET "+command
				+" WHERE EmailAttachNum = "+POut.Long(emailAttach.EmailAttachNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(EmailAttach,EmailAttach) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(EmailAttach emailAttach,EmailAttach oldEmailAttach) {
			if(emailAttach.EmailMessageNum != oldEmailAttach.EmailMessageNum) {
				return true;
			}
			if(emailAttach.DisplayedFileName != oldEmailAttach.DisplayedFileName) {
				return true;
			}
			if(emailAttach.ActualFileName != oldEmailAttach.ActualFileName) {
				return true;
			}
			if(emailAttach.EmailTemplateNum != oldEmailAttach.EmailTemplateNum) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one EmailAttach from the database.</summary>
		public static void Delete(long emailAttachNum) {
			string command="DELETE FROM emailattach "
				+"WHERE EmailAttachNum = "+POut.Long(emailAttachNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many EmailAttachs from the database.</summary>
		public static void DeleteMany(List<long> listEmailAttachNums) {
			if(listEmailAttachNums==null || listEmailAttachNums.Count==0) {
				return;
			}
			string command="DELETE FROM emailattach "
				+"WHERE EmailAttachNum IN("+string.Join(",",listEmailAttachNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list.  Returns true if db changes were made.</summary>
		public static bool Sync(List<EmailAttach> listNew,List<EmailAttach> listDB) {
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<EmailAttach> listIns    =new List<EmailAttach>();
			List<EmailAttach> listUpdNew =new List<EmailAttach>();
			List<EmailAttach> listUpdDB  =new List<EmailAttach>();
			List<EmailAttach> listDel    =new List<EmailAttach>();
			listNew.Sort((EmailAttach x,EmailAttach y) => { return x.EmailAttachNum.CompareTo(y.EmailAttachNum); });
			listDB.Sort((EmailAttach x,EmailAttach y) => { return x.EmailAttachNum.CompareTo(y.EmailAttachNum); });
			int idxNew=0;
			int idxDB=0;
			int rowsUpdatedCount=0;
			EmailAttach fieldNew;
			EmailAttach fieldDB;
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
				else if(fieldNew.EmailAttachNum<fieldDB.EmailAttachNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.EmailAttachNum>fieldDB.EmailAttachNum) {//dbPK less than newPK, dbItem is 'next'
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
			DeleteMany(listDel.Select(x => x.EmailAttachNum).ToList());
			if(rowsUpdatedCount>0 || listIns.Count>0 || listDel.Count>0) {
				return true;
			}
			return false;
		}

	}
}