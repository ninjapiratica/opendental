//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class DisplayFieldCrud {
		///<summary>Gets one DisplayField object from the database using the primary key.  Returns null if not found.</summary>
		public static DisplayField SelectOne(long displayFieldNum) {
			string command="SELECT * FROM displayfield "
				+"WHERE DisplayFieldNum = "+POut.Long(displayFieldNum);
			List<DisplayField> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one DisplayField object from the database using a query.</summary>
		public static DisplayField SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<DisplayField> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of DisplayField objects from the database using a query.</summary>
		public static List<DisplayField> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<DisplayField> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<DisplayField> TableToList(DataTable table) {
			List<DisplayField> retVal=new List<DisplayField>();
			DisplayField displayField;
			foreach(DataRow row in table.Rows) {
				displayField=new DisplayField();
				displayField.DisplayFieldNum    = PIn.Long  (row["DisplayFieldNum"].ToString());
				displayField.InternalName       = PIn.String(row["InternalName"].ToString());
				displayField.ItemOrder          = PIn.Int   (row["ItemOrder"].ToString());
				displayField.Description        = PIn.String(row["Description"].ToString());
				displayField.ColumnWidth        = PIn.Int   (row["ColumnWidth"].ToString());
				displayField.Category           = (OpenDentBusiness.DisplayFieldCategory)PIn.Int(row["Category"].ToString());
				displayField.ChartViewNum       = PIn.Long  (row["ChartViewNum"].ToString());
				displayField.PickList           = PIn.String(row["PickList"].ToString());
				displayField.DescriptionOverride= PIn.String(row["DescriptionOverride"].ToString());
				retVal.Add(displayField);
			}
			return retVal;
		}

		///<summary>Converts a list of DisplayField into a DataTable.</summary>
		public static DataTable ListToTable(List<DisplayField> listDisplayFields,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="DisplayField";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("DisplayFieldNum");
			table.Columns.Add("InternalName");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("Description");
			table.Columns.Add("ColumnWidth");
			table.Columns.Add("Category");
			table.Columns.Add("ChartViewNum");
			table.Columns.Add("PickList");
			table.Columns.Add("DescriptionOverride");
			foreach(DisplayField displayField in listDisplayFields) {
				table.Rows.Add(new object[] {
					POut.Long  (displayField.DisplayFieldNum),
					            displayField.InternalName,
					POut.Int   (displayField.ItemOrder),
					            displayField.Description,
					POut.Int   (displayField.ColumnWidth),
					POut.Int   ((int)displayField.Category),
					POut.Long  (displayField.ChartViewNum),
					            displayField.PickList,
					            displayField.DescriptionOverride,
				});
			}
			return table;
		}

		///<summary>Inserts one DisplayField into the database.  Returns the new priKey.</summary>
		public static long Insert(DisplayField displayField) {
			return Insert(displayField,false);
		}

		///<summary>Inserts one DisplayField into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(DisplayField displayField,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				displayField.DisplayFieldNum=ReplicationServers.GetKey("displayfield","DisplayFieldNum");
			}
			string command="INSERT INTO displayfield (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="DisplayFieldNum,";
			}
			command+="InternalName,ItemOrder,Description,ColumnWidth,Category,ChartViewNum,PickList,DescriptionOverride) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(displayField.DisplayFieldNum)+",";
			}
			command+=
				 "'"+POut.String(displayField.InternalName)+"',"
				+    POut.Int   (displayField.ItemOrder)+","
				+"'"+POut.String(displayField.Description)+"',"
				+    POut.Int   (displayField.ColumnWidth)+","
				+    POut.Int   ((int)displayField.Category)+","
				+    POut.Long  (displayField.ChartViewNum)+","
				+    DbHelper.ParamChar+"paramPickList,"
				+"'"+POut.String(displayField.DescriptionOverride)+"')";
			if(displayField.PickList==null) {
				displayField.PickList="";
			}
			OdSqlParameter paramPickList=new OdSqlParameter("paramPickList",OdDbType.Text,POut.StringParam(displayField.PickList));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramPickList);
			}
			else {
				displayField.DisplayFieldNum=Db.NonQ(command,true,"DisplayFieldNum","displayField",paramPickList);
			}
			return displayField.DisplayFieldNum;
		}

		///<summary>Inserts one DisplayField into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(DisplayField displayField) {
			return InsertNoCache(displayField,false);
		}

		///<summary>Inserts one DisplayField into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(DisplayField displayField,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO displayfield (";
			if(!useExistingPK && isRandomKeys) {
				displayField.DisplayFieldNum=ReplicationServers.GetKeyNoCache("displayfield","DisplayFieldNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="DisplayFieldNum,";
			}
			command+="InternalName,ItemOrder,Description,ColumnWidth,Category,ChartViewNum,PickList,DescriptionOverride) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(displayField.DisplayFieldNum)+",";
			}
			command+=
				 "'"+POut.String(displayField.InternalName)+"',"
				+    POut.Int   (displayField.ItemOrder)+","
				+"'"+POut.String(displayField.Description)+"',"
				+    POut.Int   (displayField.ColumnWidth)+","
				+    POut.Int   ((int)displayField.Category)+","
				+    POut.Long  (displayField.ChartViewNum)+","
				+    DbHelper.ParamChar+"paramPickList,"
				+"'"+POut.String(displayField.DescriptionOverride)+"')";
			if(displayField.PickList==null) {
				displayField.PickList="";
			}
			OdSqlParameter paramPickList=new OdSqlParameter("paramPickList",OdDbType.Text,POut.StringParam(displayField.PickList));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramPickList);
			}
			else {
				displayField.DisplayFieldNum=Db.NonQ(command,true,"DisplayFieldNum","displayField",paramPickList);
			}
			return displayField.DisplayFieldNum;
		}

		///<summary>Updates one DisplayField in the database.</summary>
		public static void Update(DisplayField displayField) {
			string command="UPDATE displayfield SET "
				+"InternalName       = '"+POut.String(displayField.InternalName)+"', "
				+"ItemOrder          =  "+POut.Int   (displayField.ItemOrder)+", "
				+"Description        = '"+POut.String(displayField.Description)+"', "
				+"ColumnWidth        =  "+POut.Int   (displayField.ColumnWidth)+", "
				+"Category           =  "+POut.Int   ((int)displayField.Category)+", "
				+"ChartViewNum       =  "+POut.Long  (displayField.ChartViewNum)+", "
				+"PickList           =  "+DbHelper.ParamChar+"paramPickList, "
				+"DescriptionOverride= '"+POut.String(displayField.DescriptionOverride)+"' "
				+"WHERE DisplayFieldNum = "+POut.Long(displayField.DisplayFieldNum);
			if(displayField.PickList==null) {
				displayField.PickList="";
			}
			OdSqlParameter paramPickList=new OdSqlParameter("paramPickList",OdDbType.Text,POut.StringParam(displayField.PickList));
			Db.NonQ(command,paramPickList);
		}

		///<summary>Updates one DisplayField in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(DisplayField displayField,DisplayField oldDisplayField) {
			string command="";
			if(displayField.InternalName != oldDisplayField.InternalName) {
				if(command!="") { command+=",";}
				command+="InternalName = '"+POut.String(displayField.InternalName)+"'";
			}
			if(displayField.ItemOrder != oldDisplayField.ItemOrder) {
				if(command!="") { command+=",";}
				command+="ItemOrder = "+POut.Int(displayField.ItemOrder)+"";
			}
			if(displayField.Description != oldDisplayField.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(displayField.Description)+"'";
			}
			if(displayField.ColumnWidth != oldDisplayField.ColumnWidth) {
				if(command!="") { command+=",";}
				command+="ColumnWidth = "+POut.Int(displayField.ColumnWidth)+"";
			}
			if(displayField.Category != oldDisplayField.Category) {
				if(command!="") { command+=",";}
				command+="Category = "+POut.Int   ((int)displayField.Category)+"";
			}
			if(displayField.ChartViewNum != oldDisplayField.ChartViewNum) {
				if(command!="") { command+=",";}
				command+="ChartViewNum = "+POut.Long(displayField.ChartViewNum)+"";
			}
			if(displayField.PickList != oldDisplayField.PickList) {
				if(command!="") { command+=",";}
				command+="PickList = "+DbHelper.ParamChar+"paramPickList";
			}
			if(displayField.DescriptionOverride != oldDisplayField.DescriptionOverride) {
				if(command!="") { command+=",";}
				command+="DescriptionOverride = '"+POut.String(displayField.DescriptionOverride)+"'";
			}
			if(command=="") {
				return false;
			}
			if(displayField.PickList==null) {
				displayField.PickList="";
			}
			OdSqlParameter paramPickList=new OdSqlParameter("paramPickList",OdDbType.Text,POut.StringParam(displayField.PickList));
			command="UPDATE displayfield SET "+command
				+" WHERE DisplayFieldNum = "+POut.Long(displayField.DisplayFieldNum);
			Db.NonQ(command,paramPickList);
			return true;
		}

		///<summary>Returns true if Update(DisplayField,DisplayField) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(DisplayField displayField,DisplayField oldDisplayField) {
			if(displayField.InternalName != oldDisplayField.InternalName) {
				return true;
			}
			if(displayField.ItemOrder != oldDisplayField.ItemOrder) {
				return true;
			}
			if(displayField.Description != oldDisplayField.Description) {
				return true;
			}
			if(displayField.ColumnWidth != oldDisplayField.ColumnWidth) {
				return true;
			}
			if(displayField.Category != oldDisplayField.Category) {
				return true;
			}
			if(displayField.ChartViewNum != oldDisplayField.ChartViewNum) {
				return true;
			}
			if(displayField.PickList != oldDisplayField.PickList) {
				return true;
			}
			if(displayField.DescriptionOverride != oldDisplayField.DescriptionOverride) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one DisplayField from the database.</summary>
		public static void Delete(long displayFieldNum) {
			string command="DELETE FROM displayfield "
				+"WHERE DisplayFieldNum = "+POut.Long(displayFieldNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many DisplayFields from the database.</summary>
		public static void DeleteMany(List<long> listDisplayFieldNums) {
			if(listDisplayFieldNums==null || listDisplayFieldNums.Count==0) {
				return;
			}
			string command="DELETE FROM displayfield "
				+"WHERE DisplayFieldNum IN("+string.Join(",",listDisplayFieldNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}