using OpenDentBusiness;

namespace UnitTestsCore
{
    public class EServiceSignalT
    {
        public static void ClearEServiceSignalTable()
        {
            string command = "DELETE FROM eservicesignal";
            DataCore.NonQ(command);
        }
    }
}
