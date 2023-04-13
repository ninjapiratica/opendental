using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class CloudBillings
    {
        ///<summary></summary>
        public static long Insert(CloudBilling cloudBilling)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                cloudBilling.CloudBillingNum = Meth.GetLong(MethodBase.GetCurrentMethod(), cloudBilling);
                return cloudBilling.CloudBillingNum;
            }
            return Crud.CloudBillingCrud.Insert(cloudBilling);
        }

        ///<summary>Should only be called if ODHQ.</summary>
        public static int AddCloudRepeatingChargesHelper(DateTime dateRun)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetInt(MethodBase.GetCurrentMethod(), dateRun);
            }
            //Get all bills that are due to be posted as of this date.
            string command = "SELECT * FROM cloudbilling " +
                "WHERE DateOfBill <= " + POut.Date(dateRun.Date, true) + " AND DateTimeProceduresPosted = '0001-01-01 00:00:00'";
            List<CloudBilling> listBillsDue = Crud.CloudBillingCrud.SelectMany(command);
            int retVal = 0;
            foreach (CloudBilling cloudBilling in listBillsDue)
            {
                //List of procedures for this billing cycle was serialized to CloudBilling.ProceduresJson by ProcessBilling thread. Deserialize and post them.
                List<Procedure> listProcs = JsonConvert.DeserializeObject<List<Procedure>>(cloudBilling.ProceduresJson);
                foreach (Procedure proc in listProcs)
                {
                    Procedures.Insert(proc);
                    retVal++;
                }
                cloudBilling.DateTimeProceduresPosted = DateTime_.Now;
                Crud.CloudBillingCrud.Update(cloudBilling);
            }
            return retVal;
        }

        /*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary></summary>
		public static List<CloudBilling> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<CloudBilling>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM cloudbilling WHERE PatNum = "+POut.Long(patNum);
			return Crud.CloudBillingCrud.SelectMany(command);
		}
		
		///<summary>Gets one CloudBilling from the db.</summary>
		public static CloudBilling GetOne(long cloudBillingNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<CloudBilling>(MethodBase.GetCurrentMethod(),cloudBillingNum);
			}
			return Crud.CloudBillingCrud.SelectOne(cloudBillingNum);
		}
		#endregion Methods - Get
		#region Methods - Modify

		///<summary></summary>
		public static void Update(CloudBilling cloudBilling){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cloudBilling);
				return;
			}
			Crud.CloudBillingCrud.Update(cloudBilling);
		}
		///<summary></summary>
		public static void Delete(long cloudBillingNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cloudBillingNum);
				return;
			}
			Crud.CloudBillingCrud.Delete(cloudBillingNum);
		}
		#endregion Methods - Modify
		#region Methods - Misc
		

		
		#endregion Methods - Misc
		*/



    }
}