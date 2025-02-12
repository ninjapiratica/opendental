//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class SupplierCrud {
		///<summary>Gets one Supplier object from the database using the primary key.  Returns null if not found.</summary>
		public static Supplier SelectOne(long supplierNum) {
			string command="SELECT * FROM supplier "
				+"WHERE SupplierNum = "+POut.Long(supplierNum);
			List<Supplier> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Supplier object from the database using a query.</summary>
		public static Supplier SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Supplier> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Supplier objects from the database using a query.</summary>
		public static List<Supplier> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Supplier> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Supplier> TableToList(DataTable table) {
			List<Supplier> retVal=new List<Supplier>();
			Supplier supplier;
			foreach(DataRow row in table.Rows) {
				supplier=new Supplier();
				supplier.SupplierNum= PIn.Long  (row["SupplierNum"].ToString());
				supplier.Name       = PIn.String(row["Name"].ToString());
				supplier.Phone      = PIn.String(row["Phone"].ToString());
				supplier.CustomerId = PIn.String(row["CustomerId"].ToString());
				supplier.Website    = PIn.String(row["Website"].ToString());
				supplier.UserName   = PIn.String(row["UserName"].ToString());
				supplier.Password   = PIn.String(row["Password"].ToString());
				supplier.Note       = PIn.String(row["Note"].ToString());
				retVal.Add(supplier);
			}
			return retVal;
		}

		///<summary>Converts a list of Supplier into a DataTable.</summary>
		public static DataTable ListToTable(List<Supplier> listSuppliers,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Supplier";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("SupplierNum");
			table.Columns.Add("Name");
			table.Columns.Add("Phone");
			table.Columns.Add("CustomerId");
			table.Columns.Add("Website");
			table.Columns.Add("UserName");
			table.Columns.Add("Password");
			table.Columns.Add("Note");
			foreach(Supplier supplier in listSuppliers) {
				table.Rows.Add(new object[] {
					POut.Long  (supplier.SupplierNum),
					            supplier.Name,
					            supplier.Phone,
					            supplier.CustomerId,
					            supplier.Website,
					            supplier.UserName,
					            supplier.Password,
					            supplier.Note,
				});
			}
			return table;
		}

		///<summary>Inserts one Supplier into the database.  Returns the new priKey.</summary>
		public static long Insert(Supplier supplier) {
			return Insert(supplier,false);
		}

		///<summary>Inserts one Supplier into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Supplier supplier,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				supplier.SupplierNum=ReplicationServers.GetKey("supplier","SupplierNum");
			}
			string command="INSERT INTO supplier (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="SupplierNum,";
			}
			command+="Name,Phone,CustomerId,Website,UserName,Password,Note) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(supplier.SupplierNum)+",";
			}
			command+=
				 "'"+POut.String(supplier.Name)+"',"
				+"'"+POut.String(supplier.Phone)+"',"
				+"'"+POut.String(supplier.CustomerId)+"',"
				+    DbHelper.ParamChar+"paramWebsite,"
				+"'"+POut.String(supplier.UserName)+"',"
				+"'"+POut.String(supplier.Password)+"',"
				+    DbHelper.ParamChar+"paramNote)";
			if(supplier.Website==null) {
				supplier.Website="";
			}
			OdSqlParameter paramWebsite=new OdSqlParameter("paramWebsite",OdDbType.Text,POut.StringParam(supplier.Website));
			if(supplier.Note==null) {
				supplier.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(supplier.Note));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramWebsite,paramNote);
			}
			else {
				supplier.SupplierNum=Db.NonQ(command,true,"SupplierNum","supplier",paramWebsite,paramNote);
			}
			return supplier.SupplierNum;
		}

		///<summary>Inserts one Supplier into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Supplier supplier) {
			return InsertNoCache(supplier,false);
		}

		///<summary>Inserts one Supplier into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Supplier supplier,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO supplier (";
			if(!useExistingPK && isRandomKeys) {
				supplier.SupplierNum=ReplicationServers.GetKeyNoCache("supplier","SupplierNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="SupplierNum,";
			}
			command+="Name,Phone,CustomerId,Website,UserName,Password,Note) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(supplier.SupplierNum)+",";
			}
			command+=
				 "'"+POut.String(supplier.Name)+"',"
				+"'"+POut.String(supplier.Phone)+"',"
				+"'"+POut.String(supplier.CustomerId)+"',"
				+    DbHelper.ParamChar+"paramWebsite,"
				+"'"+POut.String(supplier.UserName)+"',"
				+"'"+POut.String(supplier.Password)+"',"
				+    DbHelper.ParamChar+"paramNote)";
			if(supplier.Website==null) {
				supplier.Website="";
			}
			OdSqlParameter paramWebsite=new OdSqlParameter("paramWebsite",OdDbType.Text,POut.StringParam(supplier.Website));
			if(supplier.Note==null) {
				supplier.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(supplier.Note));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramWebsite,paramNote);
			}
			else {
				supplier.SupplierNum=Db.NonQ(command,true,"SupplierNum","supplier",paramWebsite,paramNote);
			}
			return supplier.SupplierNum;
		}

		///<summary>Updates one Supplier in the database.</summary>
		public static void Update(Supplier supplier) {
			string command="UPDATE supplier SET "
				+"Name       = '"+POut.String(supplier.Name)+"', "
				+"Phone      = '"+POut.String(supplier.Phone)+"', "
				+"CustomerId = '"+POut.String(supplier.CustomerId)+"', "
				+"Website    =  "+DbHelper.ParamChar+"paramWebsite, "
				+"UserName   = '"+POut.String(supplier.UserName)+"', "
				+"Password   = '"+POut.String(supplier.Password)+"', "
				+"Note       =  "+DbHelper.ParamChar+"paramNote "
				+"WHERE SupplierNum = "+POut.Long(supplier.SupplierNum);
			if(supplier.Website==null) {
				supplier.Website="";
			}
			OdSqlParameter paramWebsite=new OdSqlParameter("paramWebsite",OdDbType.Text,POut.StringParam(supplier.Website));
			if(supplier.Note==null) {
				supplier.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(supplier.Note));
			Db.NonQ(command,paramWebsite,paramNote);
		}

		///<summary>Updates one Supplier in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Supplier supplier,Supplier oldSupplier) {
			string command="";
			if(supplier.Name != oldSupplier.Name) {
				if(command!="") { command+=",";}
				command+="Name = '"+POut.String(supplier.Name)+"'";
			}
			if(supplier.Phone != oldSupplier.Phone) {
				if(command!="") { command+=",";}
				command+="Phone = '"+POut.String(supplier.Phone)+"'";
			}
			if(supplier.CustomerId != oldSupplier.CustomerId) {
				if(command!="") { command+=",";}
				command+="CustomerId = '"+POut.String(supplier.CustomerId)+"'";
			}
			if(supplier.Website != oldSupplier.Website) {
				if(command!="") { command+=",";}
				command+="Website = "+DbHelper.ParamChar+"paramWebsite";
			}
			if(supplier.UserName != oldSupplier.UserName) {
				if(command!="") { command+=",";}
				command+="UserName = '"+POut.String(supplier.UserName)+"'";
			}
			if(supplier.Password != oldSupplier.Password) {
				if(command!="") { command+=",";}
				command+="Password = '"+POut.String(supplier.Password)+"'";
			}
			if(supplier.Note != oldSupplier.Note) {
				if(command!="") { command+=",";}
				command+="Note = "+DbHelper.ParamChar+"paramNote";
			}
			if(command=="") {
				return false;
			}
			if(supplier.Website==null) {
				supplier.Website="";
			}
			OdSqlParameter paramWebsite=new OdSqlParameter("paramWebsite",OdDbType.Text,POut.StringParam(supplier.Website));
			if(supplier.Note==null) {
				supplier.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(supplier.Note));
			command="UPDATE supplier SET "+command
				+" WHERE SupplierNum = "+POut.Long(supplier.SupplierNum);
			Db.NonQ(command,paramWebsite,paramNote);
			return true;
		}

		///<summary>Returns true if Update(Supplier,Supplier) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Supplier supplier,Supplier oldSupplier) {
			if(supplier.Name != oldSupplier.Name) {
				return true;
			}
			if(supplier.Phone != oldSupplier.Phone) {
				return true;
			}
			if(supplier.CustomerId != oldSupplier.CustomerId) {
				return true;
			}
			if(supplier.Website != oldSupplier.Website) {
				return true;
			}
			if(supplier.UserName != oldSupplier.UserName) {
				return true;
			}
			if(supplier.Password != oldSupplier.Password) {
				return true;
			}
			if(supplier.Note != oldSupplier.Note) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one Supplier from the database.</summary>
		public static void Delete(long supplierNum) {
			string command="DELETE FROM supplier "
				+"WHERE SupplierNum = "+POut.Long(supplierNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many Suppliers from the database.</summary>
		public static void DeleteMany(List<long> listSupplierNums) {
			if(listSupplierNums==null || listSupplierNums.Count==0) {
				return;
			}
			string command="DELETE FROM supplier "
				+"WHERE SupplierNum IN("+string.Join(",",listSupplierNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}