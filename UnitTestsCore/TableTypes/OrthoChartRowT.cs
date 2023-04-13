using OpenDentBusiness;

namespace UnitTestsCore
{
    public class OrthoChartRowT
    {
        public static void ClearOrthoChartRowTable()
        {
            string command = "DELETE FROM orthochartrow WHERE OrthoChartRowNum > 0";
            DataCore.NonQ(command);
        }
    }
}
