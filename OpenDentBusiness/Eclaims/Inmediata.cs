using CodeBase;
using System;

namespace OpenDentBusiness.Eclaims
{
    /// <summary>
    /// Summary description for RECS.
    /// </summary>
    public class Inmediata
    {

        public static string ErrorMessage = "";

        ///<summary></summary>
        public Inmediata()
        {

        }

        ///<summary>Returns true if the communications were successful, and false if they failed.</summary>
        public static bool Launch(Clearinghouse clearinghouseClin, int batchNum)
        {//called from Eclaims.cs. Clinic-level clearinghouse passed in.
            try
            {
                //call the client program
                ODFileUtils.ProcessStart(clearinghouseClin.ClientProgram, doWaitForODCloudClientResponse: true);
            }
            catch (Exception ex)
            {
                //X12.Rollback(clearhouse,batchNum);//doesn't actually do anything
                ErrorMessage = ex.Message;
                return false;
            }
            return true;
        }


    }
}
