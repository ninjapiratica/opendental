using OpenDentBusiness;

namespace UnitTestsCore
{
    public class OrthoProcLinkT
    {

        ///<summary>Deletes everything from the orthoproclink table.  Does not truncate the table so that PKs are not reused on accident.</summary>
        public static void ClearTable()
        {
            DataCore.NonQ("DELETE FROM orthoproclink WHERE OrthoProcLinkNum > 0");
        }

    }
}
