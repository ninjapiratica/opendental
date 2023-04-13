using OpenDentBusiness;

namespace UnitTestsCore
{
    public class AllergyT
    {
        public static void ClearAllergyTable()
        {
            string command = $"DELETE FROM allergy WHERE AllergyNum > 0";
            DataCore.NonQ(command);
        }
    }
}
