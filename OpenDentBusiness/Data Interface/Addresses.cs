using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class Addresses
    {
        #region Get Methods
        ///<summary>Returns null if none.</summary>
        public static Address GetOneByPatNum(long patNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<Address>(MethodBase.GetCurrentMethod(), patNum);
            }
            string command = "SELECT * FROM address WHERE PatNumTaxPhysical = " + POut.Long(patNum);
            return Crud.AddressCrud.SelectOne(command);
        }
        #endregion Get Methods

        #region Modification Methods
        ///<summary></summary>
        public static long Insert(Address address)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                address.AddressNum = Meth.GetLong(MethodBase.GetCurrentMethod(), address);
                return address.AddressNum;
            }
            return Crud.AddressCrud.Insert(address);
        }

        ///<summary></summary>
        public static void Update(Address address)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), address);
                return;
            }
            Crud.AddressCrud.Update(address);
        }

        ///<summary></summary>
        public static void Delete(long addressNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), addressNum);
                return;
            }
            Crud.AddressCrud.Delete(addressNum);
        }
        #endregion Modification Methods
    }
}