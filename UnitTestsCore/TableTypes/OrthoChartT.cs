using OpenDentBusiness;

namespace UnitTestsCore
{
    public class OrthoChartT
    {
        public static void ClearOrthoChartTable()
        {
            string command = "DELETE FROM orthochart WHERE OrthoChartNum > 0";
            DataCore.NonQ(command);
        }
    }
}
