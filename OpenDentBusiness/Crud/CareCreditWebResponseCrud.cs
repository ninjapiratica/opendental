//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class CareCreditWebResponseCrud {
		///<summary>Gets one CareCreditWebResponse object from the database using the primary key.  Returns null if not found.</summary>
		public static CareCreditWebResponse SelectOne(long careCreditWebResponseNum) {
			string command="SELECT * FROM carecreditwebresponse "
				+"WHERE CareCreditWebResponseNum = "+POut.Long(careCreditWebResponseNum);
			List<CareCreditWebResponse> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one CareCreditWebResponse object from the database using a query.</summary>
		public static CareCreditWebResponse SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<CareCreditWebResponse> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of CareCreditWebResponse objects from the database using a query.</summary>
		public static List<CareCreditWebResponse> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<CareCreditWebResponse> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<CareCreditWebResponse> TableToList(DataTable table) {
			List<CareCreditWebResponse> retVal=new List<CareCreditWebResponse>();
			CareCreditWebResponse careCreditWebResponse;
			foreach(DataRow row in table.Rows) {
				careCreditWebResponse=new CareCreditWebResponse();
				careCreditWebResponse.CareCreditWebResponseNum= PIn.Long  (row["CareCreditWebResponseNum"].ToString());
				careCreditWebResponse.PatNum                  = PIn.Long  (row["PatNum"].ToString());
				careCreditWebResponse.PayNum                  = PIn.Long  (row["PayNum"].ToString());
				careCreditWebResponse.RefNumber               = PIn.String(row["RefNumber"].ToString());
				careCreditWebResponse.Amount                  = PIn.Double(row["Amount"].ToString());
				careCreditWebResponse.WebToken                = PIn.String(row["WebToken"].ToString());
				string processingStatus=row["ProcessingStatus"].ToString();
				if(processingStatus=="") {
					careCreditWebResponse.ProcessingStatus      =(OpenDentBusiness.CareCreditWebStatus)0;
				}
				else try{
					careCreditWebResponse.ProcessingStatus      =(OpenDentBusiness.CareCreditWebStatus)Enum.Parse(typeof(OpenDentBusiness.CareCreditWebStatus),processingStatus);
				}
				catch{
					careCreditWebResponse.ProcessingStatus      =(OpenDentBusiness.CareCreditWebStatus)0;
				}
				careCreditWebResponse.DateTimeEntry           = PIn.DateT (row["DateTimeEntry"].ToString());
				careCreditWebResponse.DateTimePending         = PIn.DateT (row["DateTimePending"].ToString());
				careCreditWebResponse.DateTimeCompleted       = PIn.DateT (row["DateTimeCompleted"].ToString());
				careCreditWebResponse.DateTimeExpired         = PIn.DateT (row["DateTimeExpired"].ToString());
				careCreditWebResponse.DateTimeLastError       = PIn.DateT (row["DateTimeLastError"].ToString());
				careCreditWebResponse.LastResponseStr         = PIn.String(row["LastResponseStr"].ToString());
				careCreditWebResponse.ClinicNum               = PIn.Long  (row["ClinicNum"].ToString());
				string serviceType=row["ServiceType"].ToString();
				if(serviceType=="") {
					careCreditWebResponse.ServiceType           =(OpenDentBusiness.CareCreditServiceType)0;
				}
				else try{
					careCreditWebResponse.ServiceType           =(OpenDentBusiness.CareCreditServiceType)Enum.Parse(typeof(OpenDentBusiness.CareCreditServiceType),serviceType);
				}
				catch{
					careCreditWebResponse.ServiceType           =(OpenDentBusiness.CareCreditServiceType)0;
				}
				string transType=row["TransType"].ToString();
				if(transType=="") {
					careCreditWebResponse.TransType             =(OpenDentBusiness.CareCreditTransType)0;
				}
				else try{
					careCreditWebResponse.TransType             =(OpenDentBusiness.CareCreditTransType)Enum.Parse(typeof(OpenDentBusiness.CareCreditTransType),transType);
				}
				catch{
					careCreditWebResponse.TransType             =(OpenDentBusiness.CareCreditTransType)0;
				}
				careCreditWebResponse.MerchantNumber          = PIn.String(row["MerchantNumber"].ToString());
				careCreditWebResponse.HasLogged               = PIn.Bool  (row["HasLogged"].ToString());
				retVal.Add(careCreditWebResponse);
			}
			return retVal;
		}

		///<summary>Converts a list of CareCreditWebResponse into a DataTable.</summary>
		public static DataTable ListToTable(List<CareCreditWebResponse> listCareCreditWebResponses,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="CareCreditWebResponse";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("CareCreditWebResponseNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("PayNum");
			table.Columns.Add("RefNumber");
			table.Columns.Add("Amount");
			table.Columns.Add("WebToken");
			table.Columns.Add("ProcessingStatus");
			table.Columns.Add("DateTimeEntry");
			table.Columns.Add("DateTimePending");
			table.Columns.Add("DateTimeCompleted");
			table.Columns.Add("DateTimeExpired");
			table.Columns.Add("DateTimeLastError");
			table.Columns.Add("LastResponseStr");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("ServiceType");
			table.Columns.Add("TransType");
			table.Columns.Add("MerchantNumber");
			table.Columns.Add("HasLogged");
			foreach(CareCreditWebResponse careCreditWebResponse in listCareCreditWebResponses) {
				table.Rows.Add(new object[] {
					POut.Long  (careCreditWebResponse.CareCreditWebResponseNum),
					POut.Long  (careCreditWebResponse.PatNum),
					POut.Long  (careCreditWebResponse.PayNum),
					            careCreditWebResponse.RefNumber,
					POut.Double(careCreditWebResponse.Amount),
					            careCreditWebResponse.WebToken,
					POut.Int   ((int)careCreditWebResponse.ProcessingStatus),
					POut.DateT (careCreditWebResponse.DateTimeEntry,false),
					POut.DateT (careCreditWebResponse.DateTimePending,false),
					POut.DateT (careCreditWebResponse.DateTimeCompleted,false),
					POut.DateT (careCreditWebResponse.DateTimeExpired,false),
					POut.DateT (careCreditWebResponse.DateTimeLastError,false),
					            careCreditWebResponse.LastResponseStr,
					POut.Long  (careCreditWebResponse.ClinicNum),
					POut.Int   ((int)careCreditWebResponse.ServiceType),
					POut.Int   ((int)careCreditWebResponse.TransType),
					            careCreditWebResponse.MerchantNumber,
					POut.Bool  (careCreditWebResponse.HasLogged),
				});
			}
			return table;
		}

		///<summary>Inserts one CareCreditWebResponse into the database.  Returns the new priKey.</summary>
		public static long Insert(CareCreditWebResponse careCreditWebResponse) {
			return Insert(careCreditWebResponse,false);
		}

		///<summary>Inserts one CareCreditWebResponse into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(CareCreditWebResponse careCreditWebResponse,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				careCreditWebResponse.CareCreditWebResponseNum=ReplicationServers.GetKey("carecreditwebresponse","CareCreditWebResponseNum");
			}
			string command="INSERT INTO carecreditwebresponse (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="CareCreditWebResponseNum,";
			}
			command+="PatNum,PayNum,RefNumber,Amount,WebToken,ProcessingStatus,DateTimeEntry,DateTimePending,DateTimeCompleted,DateTimeExpired,DateTimeLastError,LastResponseStr,ClinicNum,ServiceType,TransType,MerchantNumber,HasLogged) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(careCreditWebResponse.CareCreditWebResponseNum)+",";
			}
			command+=
				     POut.Long  (careCreditWebResponse.PatNum)+","
				+    POut.Long  (careCreditWebResponse.PayNum)+","
				+"'"+POut.String(careCreditWebResponse.RefNumber)+"',"
				+		 POut.Double(careCreditWebResponse.Amount)+","
				+"'"+POut.String(careCreditWebResponse.WebToken)+"',"
				+"'"+POut.String(careCreditWebResponse.ProcessingStatus.ToString())+"',"
				+    DbHelper.Now()+","
				+    POut.DateT (careCreditWebResponse.DateTimePending)+","
				+    POut.DateT (careCreditWebResponse.DateTimeCompleted)+","
				+    POut.DateT (careCreditWebResponse.DateTimeExpired)+","
				+    POut.DateT (careCreditWebResponse.DateTimeLastError)+","
				+    DbHelper.ParamChar+"paramLastResponseStr,"
				+    POut.Long  (careCreditWebResponse.ClinicNum)+","
				+"'"+POut.String(careCreditWebResponse.ServiceType.ToString())+"',"
				+"'"+POut.String(careCreditWebResponse.TransType.ToString())+"',"
				+"'"+POut.String(careCreditWebResponse.MerchantNumber)+"',"
				+    POut.Bool  (careCreditWebResponse.HasLogged)+")";
			if(careCreditWebResponse.LastResponseStr==null) {
				careCreditWebResponse.LastResponseStr="";
			}
			OdSqlParameter paramLastResponseStr=new OdSqlParameter("paramLastResponseStr",OdDbType.Text,POut.StringParam(careCreditWebResponse.LastResponseStr));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramLastResponseStr);
			}
			else {
				careCreditWebResponse.CareCreditWebResponseNum=Db.NonQ(command,true,"CareCreditWebResponseNum","careCreditWebResponse",paramLastResponseStr);
			}
			return careCreditWebResponse.CareCreditWebResponseNum;
		}

		///<summary>Inserts one CareCreditWebResponse into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(CareCreditWebResponse careCreditWebResponse) {
			return InsertNoCache(careCreditWebResponse,false);
		}

		///<summary>Inserts one CareCreditWebResponse into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(CareCreditWebResponse careCreditWebResponse,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO carecreditwebresponse (";
			if(!useExistingPK && isRandomKeys) {
				careCreditWebResponse.CareCreditWebResponseNum=ReplicationServers.GetKeyNoCache("carecreditwebresponse","CareCreditWebResponseNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="CareCreditWebResponseNum,";
			}
			command+="PatNum,PayNum,RefNumber,Amount,WebToken,ProcessingStatus,DateTimeEntry,DateTimePending,DateTimeCompleted,DateTimeExpired,DateTimeLastError,LastResponseStr,ClinicNum,ServiceType,TransType,MerchantNumber,HasLogged) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(careCreditWebResponse.CareCreditWebResponseNum)+",";
			}
			command+=
				     POut.Long  (careCreditWebResponse.PatNum)+","
				+    POut.Long  (careCreditWebResponse.PayNum)+","
				+"'"+POut.String(careCreditWebResponse.RefNumber)+"',"
				+	   POut.Double(careCreditWebResponse.Amount)+","
				+"'"+POut.String(careCreditWebResponse.WebToken)+"',"
				+"'"+POut.String(careCreditWebResponse.ProcessingStatus.ToString())+"',"
				+    DbHelper.Now()+","
				+    POut.DateT (careCreditWebResponse.DateTimePending)+","
				+    POut.DateT (careCreditWebResponse.DateTimeCompleted)+","
				+    POut.DateT (careCreditWebResponse.DateTimeExpired)+","
				+    POut.DateT (careCreditWebResponse.DateTimeLastError)+","
				+    DbHelper.ParamChar+"paramLastResponseStr,"
				+    POut.Long  (careCreditWebResponse.ClinicNum)+","
				+"'"+POut.String(careCreditWebResponse.ServiceType.ToString())+"',"
				+"'"+POut.String(careCreditWebResponse.TransType.ToString())+"',"
				+"'"+POut.String(careCreditWebResponse.MerchantNumber)+"',"
				+    POut.Bool  (careCreditWebResponse.HasLogged)+")";
			if(careCreditWebResponse.LastResponseStr==null) {
				careCreditWebResponse.LastResponseStr="";
			}
			OdSqlParameter paramLastResponseStr=new OdSqlParameter("paramLastResponseStr",OdDbType.Text,POut.StringParam(careCreditWebResponse.LastResponseStr));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramLastResponseStr);
			}
			else {
				careCreditWebResponse.CareCreditWebResponseNum=Db.NonQ(command,true,"CareCreditWebResponseNum","careCreditWebResponse",paramLastResponseStr);
			}
			return careCreditWebResponse.CareCreditWebResponseNum;
		}

		///<summary>Updates one CareCreditWebResponse in the database.</summary>
		public static void Update(CareCreditWebResponse careCreditWebResponse) {
			string command="UPDATE carecreditwebresponse SET "
				+"PatNum                  =  "+POut.Long  (careCreditWebResponse.PatNum)+", "
				+"PayNum                  =  "+POut.Long  (careCreditWebResponse.PayNum)+", "
				+"RefNumber               = '"+POut.String(careCreditWebResponse.RefNumber)+"', "
				+"Amount                  =  "+POut.Double(careCreditWebResponse.Amount)+", "
				+"WebToken                = '"+POut.String(careCreditWebResponse.WebToken)+"', "
				+"ProcessingStatus        = '"+POut.String(careCreditWebResponse.ProcessingStatus.ToString())+"', "
				//DateTimeEntry not allowed to change
				+"DateTimePending         =  "+POut.DateT (careCreditWebResponse.DateTimePending)+", "
				+"DateTimeCompleted       =  "+POut.DateT (careCreditWebResponse.DateTimeCompleted)+", "
				+"DateTimeExpired         =  "+POut.DateT (careCreditWebResponse.DateTimeExpired)+", "
				+"DateTimeLastError       =  "+POut.DateT (careCreditWebResponse.DateTimeLastError)+", "
				+"LastResponseStr         =  "+DbHelper.ParamChar+"paramLastResponseStr, "
				+"ClinicNum               =  "+POut.Long  (careCreditWebResponse.ClinicNum)+", "
				+"ServiceType             = '"+POut.String(careCreditWebResponse.ServiceType.ToString())+"', "
				+"TransType               = '"+POut.String(careCreditWebResponse.TransType.ToString())+"', "
				+"MerchantNumber          = '"+POut.String(careCreditWebResponse.MerchantNumber)+"', "
				+"HasLogged               =  "+POut.Bool  (careCreditWebResponse.HasLogged)+" "
				+"WHERE CareCreditWebResponseNum = "+POut.Long(careCreditWebResponse.CareCreditWebResponseNum);
			if(careCreditWebResponse.LastResponseStr==null) {
				careCreditWebResponse.LastResponseStr="";
			}
			OdSqlParameter paramLastResponseStr=new OdSqlParameter("paramLastResponseStr",OdDbType.Text,POut.StringParam(careCreditWebResponse.LastResponseStr));
			Db.NonQ(command,paramLastResponseStr);
		}

		///<summary>Updates one CareCreditWebResponse in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(CareCreditWebResponse careCreditWebResponse,CareCreditWebResponse oldCareCreditWebResponse) {
			string command="";
			if(careCreditWebResponse.PatNum != oldCareCreditWebResponse.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(careCreditWebResponse.PatNum)+"";
			}
			if(careCreditWebResponse.PayNum != oldCareCreditWebResponse.PayNum) {
				if(command!="") { command+=",";}
				command+="PayNum = "+POut.Long(careCreditWebResponse.PayNum)+"";
			}
			if(careCreditWebResponse.RefNumber != oldCareCreditWebResponse.RefNumber) {
				if(command!="") { command+=",";}
				command+="RefNumber = '"+POut.String(careCreditWebResponse.RefNumber)+"'";
			}
			if(careCreditWebResponse.Amount != oldCareCreditWebResponse.Amount) {
				if(command!="") { command+=",";}
				command+="Amount = "+POut.Double(careCreditWebResponse.Amount)+"";
			}
			if(careCreditWebResponse.WebToken != oldCareCreditWebResponse.WebToken) {
				if(command!="") { command+=",";}
				command+="WebToken = '"+POut.String(careCreditWebResponse.WebToken)+"'";
			}
			if(careCreditWebResponse.ProcessingStatus != oldCareCreditWebResponse.ProcessingStatus) {
				if(command!="") { command+=",";}
				command+="ProcessingStatus = '"+POut.String(careCreditWebResponse.ProcessingStatus.ToString())+"'";
			}
			//DateTimeEntry not allowed to change
			if(careCreditWebResponse.DateTimePending != oldCareCreditWebResponse.DateTimePending) {
				if(command!="") { command+=",";}
				command+="DateTimePending = "+POut.DateT(careCreditWebResponse.DateTimePending)+"";
			}
			if(careCreditWebResponse.DateTimeCompleted != oldCareCreditWebResponse.DateTimeCompleted) {
				if(command!="") { command+=",";}
				command+="DateTimeCompleted = "+POut.DateT(careCreditWebResponse.DateTimeCompleted)+"";
			}
			if(careCreditWebResponse.DateTimeExpired != oldCareCreditWebResponse.DateTimeExpired) {
				if(command!="") { command+=",";}
				command+="DateTimeExpired = "+POut.DateT(careCreditWebResponse.DateTimeExpired)+"";
			}
			if(careCreditWebResponse.DateTimeLastError != oldCareCreditWebResponse.DateTimeLastError) {
				if(command!="") { command+=",";}
				command+="DateTimeLastError = "+POut.DateT(careCreditWebResponse.DateTimeLastError)+"";
			}
			if(careCreditWebResponse.LastResponseStr != oldCareCreditWebResponse.LastResponseStr) {
				if(command!="") { command+=",";}
				command+="LastResponseStr = "+DbHelper.ParamChar+"paramLastResponseStr";
			}
			if(careCreditWebResponse.ClinicNum != oldCareCreditWebResponse.ClinicNum) {
				if(command!="") { command+=",";}
				command+="ClinicNum = "+POut.Long(careCreditWebResponse.ClinicNum)+"";
			}
			if(careCreditWebResponse.ServiceType != oldCareCreditWebResponse.ServiceType) {
				if(command!="") { command+=",";}
				command+="ServiceType = '"+POut.String(careCreditWebResponse.ServiceType.ToString())+"'";
			}
			if(careCreditWebResponse.TransType != oldCareCreditWebResponse.TransType) {
				if(command!="") { command+=",";}
				command+="TransType = '"+POut.String(careCreditWebResponse.TransType.ToString())+"'";
			}
			if(careCreditWebResponse.MerchantNumber != oldCareCreditWebResponse.MerchantNumber) {
				if(command!="") { command+=",";}
				command+="MerchantNumber = '"+POut.String(careCreditWebResponse.MerchantNumber)+"'";
			}
			if(careCreditWebResponse.HasLogged != oldCareCreditWebResponse.HasLogged) {
				if(command!="") { command+=",";}
				command+="HasLogged = "+POut.Bool(careCreditWebResponse.HasLogged)+"";
			}
			if(command=="") {
				return false;
			}
			if(careCreditWebResponse.LastResponseStr==null) {
				careCreditWebResponse.LastResponseStr="";
			}
			OdSqlParameter paramLastResponseStr=new OdSqlParameter("paramLastResponseStr",OdDbType.Text,POut.StringParam(careCreditWebResponse.LastResponseStr));
			command="UPDATE carecreditwebresponse SET "+command
				+" WHERE CareCreditWebResponseNum = "+POut.Long(careCreditWebResponse.CareCreditWebResponseNum);
			Db.NonQ(command,paramLastResponseStr);
			return true;
		}

		///<summary>Returns true if Update(CareCreditWebResponse,CareCreditWebResponse) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(CareCreditWebResponse careCreditWebResponse,CareCreditWebResponse oldCareCreditWebResponse) {
			if(careCreditWebResponse.PatNum != oldCareCreditWebResponse.PatNum) {
				return true;
			}
			if(careCreditWebResponse.PayNum != oldCareCreditWebResponse.PayNum) {
				return true;
			}
			if(careCreditWebResponse.RefNumber != oldCareCreditWebResponse.RefNumber) {
				return true;
			}
			if(careCreditWebResponse.Amount != oldCareCreditWebResponse.Amount) {
				return true;
			}
			if(careCreditWebResponse.WebToken != oldCareCreditWebResponse.WebToken) {
				return true;
			}
			if(careCreditWebResponse.ProcessingStatus != oldCareCreditWebResponse.ProcessingStatus) {
				return true;
			}
			//DateTimeEntry not allowed to change
			if(careCreditWebResponse.DateTimePending != oldCareCreditWebResponse.DateTimePending) {
				return true;
			}
			if(careCreditWebResponse.DateTimeCompleted != oldCareCreditWebResponse.DateTimeCompleted) {
				return true;
			}
			if(careCreditWebResponse.DateTimeExpired != oldCareCreditWebResponse.DateTimeExpired) {
				return true;
			}
			if(careCreditWebResponse.DateTimeLastError != oldCareCreditWebResponse.DateTimeLastError) {
				return true;
			}
			if(careCreditWebResponse.LastResponseStr != oldCareCreditWebResponse.LastResponseStr) {
				return true;
			}
			if(careCreditWebResponse.ClinicNum != oldCareCreditWebResponse.ClinicNum) {
				return true;
			}
			if(careCreditWebResponse.ServiceType != oldCareCreditWebResponse.ServiceType) {
				return true;
			}
			if(careCreditWebResponse.TransType != oldCareCreditWebResponse.TransType) {
				return true;
			}
			if(careCreditWebResponse.MerchantNumber != oldCareCreditWebResponse.MerchantNumber) {
				return true;
			}
			if(careCreditWebResponse.HasLogged != oldCareCreditWebResponse.HasLogged) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one CareCreditWebResponse from the database.</summary>
		public static void Delete(long careCreditWebResponseNum) {
			string command="DELETE FROM carecreditwebresponse "
				+"WHERE CareCreditWebResponseNum = "+POut.Long(careCreditWebResponseNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many CareCreditWebResponses from the database.</summary>
		public static void DeleteMany(List<long> listCareCreditWebResponseNums) {
			if(listCareCreditWebResponseNums==null || listCareCreditWebResponseNums.Count==0) {
				return;
			}
			string command="DELETE FROM carecreditwebresponse "
				+"WHERE CareCreditWebResponseNum IN("+string.Join(",",listCareCreditWebResponseNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}