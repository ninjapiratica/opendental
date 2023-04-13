using OpenDentBusiness;

namespace UnitTestsCore
{
    public class EmailAttachT
    {

        ///<summary>Deletes everything from the table.  Does not truncate the table so that PKs are not reused on accident.</summary>
        public static void ClearEmailAttachTable()
        {
            string command = "DELETE FROM emailattach WHERE EmailAttachNum > 0";
            DataCore.NonQ(command);
        }
    }
}
