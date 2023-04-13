using OpenDentBusiness;

namespace UnitTestsCore
{
    public class AlertSubT
    {
        public static void ClearAlertSubTable()
        {
            string command = "DELETE FROM alertsub WHERE AlertSubNum > 0";
            DataCore.NonQ(command);
        }
    }
}
