using OpenDentBusiness;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace OpenDental.Bridges
{
    /// <summary>Insurance Answers Plus</summary>
    public class Iap
    {
        ///<summary></summary>
        public const int NEXT = 1;
        ///<summary></summary>
        public const int PREV = 2;
        ///<summary></summary>
        public const int EXACT = 3;
        ///<summary></summary>
        public const int SEARCH = 4;
        ///<summary></summary>
        public const int FIRST = 5;

        ///<summary></summary>
        public const int NOERROR = 0;
        ///<summary></summary>
        public const int FILE_NOT_FOUND = 3;
        ///<summary></summary>
        public const int BAD_VERSION = 4;

        ///<summary></summary>
        public const int DATA_OK = 0;
        ///<summary></summary>
        public const int DATA_END = 255;
        ///<summary></summary>
        public const int KEY_NOT_FOUND = 2;

        ///<summary></summary>
        public const int Employer = 1;
        ///<summary></summary>
        public const int Phone = 2;
        ///<summary></summary>
        public const int InsUnder = 3;
        ///<summary></summary>
        public const int Carrier = 4;
        ///<summary></summary>
        public const int CarrierPh = 5;
        ///<summary></summary>
        public const int Group = 6;
        ///<summary></summary>
        public const int MailTo = 7;
        ///<summary></summary>
        public const int MailTo2 = 8;       //place holder
        ///<summary></summary>
        public const int MailTo3 = 9;
        ///<summary></summary>
        public const int EClaims = 10;
        ///<summary></summary>
        public const int FAXClaims = 11;
        ///<summary></summary>
        public const int DMOOption = 12;
        ///<summary></summary>
        public const int Medical = 13;
        ///<summary></summary>
        public const int GroupNum = 14;
        ///<summary></summary>
        public const int Phone2 = 15;
        ///<summary></summary>
        public const int Deductible = 16;
        ///<summary></summary>
        public const int FamilyDed = 17;
        ///<summary></summary>
        public const int Maximum = 18;
        ///<summary></summary>
        public const int BenefitYear = 19;
        ///<summary></summary>
        public const int DependentAge = 20;
        ///<summary></summary>
        public const int Preventive = 21;
        ///<summary></summary>
        public const int Basic = 22;
        ///<summary></summary>
        public const int Major = 23;
        ///<summary></summary>
        public const int InitialPlacement = 24;
        ///<summary></summary>
        public const int ExtractionClause = 25;
        ///<summary></summary>
        public const int Replacement = 26;
        ///<summary></summary>
        public const int Other = 27;
        ///<summary></summary>
        public const int Orthodontics = 28;
        ///<summary></summary>
        public const int Deductible2 = 29;
        ///<summary></summary>
        public const int Maximum2 = 30;
        ///<summary></summary>
        public const int PymtSchedule = 31;
        ///<summary></summary>
        public const int AgeLimit = 33;
        ///<summary></summary>
        public const int SignatureonFile = 34;
        ///<summary></summary>
        public const int StandardADAForm = 35;
        ///<summary></summary>
        public const int CoordinationRule = 36;
        ///<summary></summary>
        public const int CoordinationCOB = 37;
        ///<summary></summary>
        public const int NightguardsforBruxism = 41;
        ///<summary></summary>
        public const int OcclusalAdjustments = 43;
        ///<summary></summary>
        public const int XXXXXX = 44;
        ///<summary></summary>
        public const int TMJNonSurgical = 45;
        ///<summary></summary>
        public const int Implants = 47;
        ///<summary></summary>
        public const int InfectionControl = 49;
        ///<summary></summary>
        public const int Cleanings = 53;
        ///<summary></summary>
        public const int OralEvaluation = 55;
        ///<summary></summary>
        public const int Fluoride1200s = 57;
        ///<summary></summary>
        public const int Code0220 = 60;
        ///<summary></summary>
        public const int Code0272_0274 = 62;
        ///<summary></summary>
        public const int Code0210 = 64;
        ///<summary></summary>
        public const int Code0330 = 66;
        ///<summary></summary>
        public const int SpaceMaintainers = 68;
        ///<summary></summary>
        public const int EmergencyExams = 70;
        ///<summary></summary>
        public const int EmergencyTreatment = 72;
        ///<summary></summary>
        public const int Sealants1351 = 74;
        ///<summary></summary>
        public const int Fillings2100 = 77;
        ///<summary></summary>
        public const int Extractions = 78;
        ///<summary></summary>
        public const int RootCanals = 79;
        ///<summary></summary>
        public const int MolarRootCanal = 80;
        ///<summary></summary>
        public const int OralSurgery = 81;
        ///<summary></summary>
        public const int ImpactionSoftTissue = 83;
        ///<summary></summary>
        public const int ImpactionPartialBony = 85;
        ///<summary></summary>
        public const int ImpactionCompleteBony = 87;
        ///<summary></summary>
        public const int SurgicalProceduresGeneral = 89;
        ///<summary></summary>
        public const int PerioSurgicalPerioOsseous = 91;
        ///<summary></summary>
        public const int SurgicalPerioOther = 93;
        ///<summary></summary>
        public const int RootPlaning = 96;
        ///<summary></summary>
        public const int Scaling4345 = 99;
        ///<summary></summary>
        public const int PerioPx = 101;
        ///<summary></summary>
        public const int PerioComment = 104;
        ///<summary></summary>
        public const int IVSedation = 105;
        ///<summary></summary>
        public const int General9220 = 106;
        ///<summary></summary>
        public const int Relines5700s = 108;
        ///<summary></summary>
        public const int StainlessSteelCrowns = 109;
        ///<summary></summary>
        public const int Crowns2700s = 110;
        ///<summary></summary>
        public const int Bridges6200 = 111;
        ///<summary></summary>
        public const int Partials5200s = 112;
        ///<summary></summary>
        public const int Dentures5100s = 113;
        ///<summary></summary>
        public const int EmpNumberXXX = 114;
        ///<summary></summary>
        public const int DateXXX = 115;
        ///<summary></summary>
        public const int Line4 = 116;
        ///<summary></summary>
        public const int Note = 117;
        ///<summary></summary>
        public const int Plan = 118;
        ///<summary></summary>
        public const int BuildUps = 119;
        ///<summary></summary>
        public const int PosteriorComposites = 121;
        //<summary></summary>
        // public string dbFile; // Practice-Web AAD / SDK 03/07

        [DllImport("iaplus10.dll")]
        private static extern int IAPInitSystem(string UserRegNumber);
        [DllImport("iaplus10.dll")]
        private static extern int IAPOpenDatabase(string zDataPath);
        [DllImport("iaplus10.dll")]
        private static extern int IAPReadARecord(ref int iReadAction, string zReadType, string zReadKey);
        [DllImport("iaplus10.dll")]
        private static extern void IAPCloseDataBase();
        [DllImport("iaplus10.dll")]
        private static extern string IAPReturnData(ref int DataFldNo);
        //Declare Function IAPInitSystem Lib "iaplus10.dll" (ByVal UserRegNumber As String) As Long
        //Declare Function IAPOpenDatabase Lib "iaplus10.dll" (ByVal zDataPath As String) As Long
        //Declare Function IAPReadARecord Lib "iaplus10.dll" (iReadAction As Long, ByVal zReadType As String, ByVal zReadKey As String) As Long
        //Declare Sub IAPCloseDataBase Lib "iaplus10.dll" ()
        //Declare Function IAPReturnData Lib "iaplus10.dll" (DataFldNo As Long) As String

        ///<summary>Gets a list of up to 40 employers. The items in the list alternate between the insurance plan number and the employer name, starting
        ///with the first insurance plan number.</summary>
        public static ArrayList GetList(string iapNumber)
        {
            ArrayList list = new ArrayList();
            string dbFile = Programs.GetProgramPath(Programs.GetCur(ProgramName.IAP));//@"C:\IAPLUS\";
            int retCode = IAPInitSystem("test");//any value
            int result = IAPOpenDatabase(dbFile);
            if (result == FILE_NOT_FOUND)
            {
                MessageBox.Show("File not found: " + dbFile);
                return new ArrayList();
            }
            if (result == BAD_VERSION)
            {
                MessageBox.Show("Bad version: " + dbFile);
                return new ArrayList();
            }
            int iReadAction = SEARCH;//FIRST;
            result = IAPReadARecord(ref iReadAction, "N", iapNumber);
            if (result == KEY_NOT_FOUND)
            {
                return new ArrayList();
            }
            else
            {
                iReadAction = NEXT;
                int DataFldNo = Employer;
                int DataPlanNo = EmpNumberXXX;
                int loop = 0;
                do
                {
                    list.Add(IAPReturnData(ref DataPlanNo));
                    list.Add(IAPReturnData(ref DataFldNo));
                    loop++;
                }
                while (loop < 40 && IAPReadARecord(ref iReadAction, "N", iapNumber) == DATA_OK);//"N" is for search by iap number.
            }
            IAPCloseDataBase();
            return list;
        }

        ///<summary>Surround by try/catch. This should be followed by ReadField calls.  Then, when done, CloseDatabase.</summary>
        public static void ReadRecord(string iapNumber)
        {
            string dbFile = Programs.GetCur(ProgramName.IAP).Path;//@"C:\IAPlus\";
            IAPInitSystem("test");//any value
            int result = IAPOpenDatabase(dbFile);
            if (result == FILE_NOT_FOUND)
            {
                throw new ApplicationException("File not found: " + dbFile);
            }
            if (result == BAD_VERSION)
            {
                throw new ApplicationException("Bad version: " + dbFile);
            }
            int iReadAction = SEARCH;//EXACT;
            result = IAPReadARecord(ref iReadAction, "N", iapNumber);
            if (result == KEY_NOT_FOUND)
            {
                throw new ApplicationException("Key not found: " + iapNumber);
            }
        }

        ///<summary>Surround by try/catch. Always call ReadRecord first.  When done with all ReadFields, CloseDatabase.</summary>
        public static string ReadField(int fieldNum)
        {
            return IAPReturnData(ref fieldNum);
        }

        ///<summary>Surround by try/catch. Always call ReadRecord first.  When done with all ReadFields, CloseDatabase.</summary>
        public static void CloseDatabase()
        {
            IAPCloseDataBase();
        }

        /*
		Dim dbFile As String
		Dim recordKey As String
		dbFile = "c:\code\clients\iaplus\iaplus10\dmcdata.db"
		IAPInitSystem "test"
		If IAPOpenDatabase(dbFile) Then
				MsgBox "Unable to open database '" + dbFile + "'"
				Exit Sub
		End If
		recordKey = Space$(1000)
		If IAPReadARecord(IAP_FIRST, "N", recordKey) Then
				MsgBox "Error reading first record"
		Else
				Do
						List1.AddItem IAPReturnData(IAP_Employer)
				Loop Until IAPReadARecord(IAP_NEXT, "N", recordKey)
		End If
		IAPCloseDataBase
		*/

    }

}