using OpenDentBusiness;
using System.Collections.Generic;

namespace UnitTestsCore
{
    public class UserOdPrefT
    {

        public static List<UserOdPref> GetByUser(long userNum)
        {
            string command = "SELECT * FROM userodpref WHERE UserNum=" + POut.Long(userNum);
            return OpenDentBusiness.Crud.UserOdPrefCrud.SelectMany(command);
        }
    }
}
