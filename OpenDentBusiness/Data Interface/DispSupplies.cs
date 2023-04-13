using System.Data;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class DispSupplies
    {
        ///<summary></summary>
        public static DataTable RefreshDispensary(long provNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetTable(MethodBase.GetCurrentMethod(), provNum);
            }
            string command = "SELECT supply.Descript,dispsupply.DateDispensed,dispsupply.DispQuantity,dispsupply.Note "
                + "FROM dispsupply LEFT JOIN supply ON dispsupply.SupplyNum=supply.SupplyNum "
                    + "WHERE dispsupply.ProvNum=" + POut.Long(provNum) + " "
                    + "ORDER BY DateDispensed,Descript";
            return Db.GetTable(command);
        }

        ///<summary></summary>
        public static long Insert(DispSupply dispSupply)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                dispSupply.DispSupplyNum = Meth.GetLong(MethodBase.GetCurrentMethod(), dispSupply);
                return dispSupply.DispSupplyNum;
            }
            return Crud.DispSupplyCrud.Insert(dispSupply);
        }

        /*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<DispSupply> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<DispSupply>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM dispsupply WHERE PatNum = "+POut.Long(patNum);
			return Crud.DispSupplyCrud.SelectMany(command);
		}

		///<summary>Gets one DispSupply from the db.</summary>
		public static DispSupply GetOne(long dispSupplyNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<DispSupply>(MethodBase.GetCurrentMethod(),dispSupplyNum);
			}
			return Crud.DispSupplyCrud.SelectOne(dispSupplyNum);
		}

		

		///<summary></summary>
		public static void Update(DispSupply dispSupply){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dispSupply);
				return;
			}
			Crud.DispSupplyCrud.Update(dispSupply);
		}

		///<summary></summary>
		public static void Delete(long dispSupplyNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dispSupplyNum);
				return;
			}
			string command= "DELETE FROM dispsupply WHERE DispSupplyNum = "+POut.Long(dispSupplyNum);
			Db.NonQ(command);
		}
		*/
    }
}