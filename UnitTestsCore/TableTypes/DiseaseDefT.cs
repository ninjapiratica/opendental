﻿using OpenDentBusiness;

namespace UnitTestsCore
{
    public class DiseaseDefT
    {

        ///<summary>Inserts the new DiseaseDef address and returns it.</summary>
        public static DiseaseDef CreateDiseaseDef(string diseaseName = "", string icd10code = "")
        {
            DiseaseDef diseaseDef = new DiseaseDef();
            diseaseDef.DiseaseName = diseaseName;
            if (diseaseName == "")
            {
                diseaseDef.DiseaseName = "Fatal Illness";
            }
            diseaseDef.Icd10Code = icd10code;
            DiseaseDefs.Insert(diseaseDef);
            DiseaseDefs.RefreshCache();
            return diseaseDef;
        }

        public static void ClearDiseaseDefTable()
        {
            string command = "DELETE FROM diseasedef";
            DataCore.NonQ(command);
        }

    }
}
