using OpenDentBusiness;
using System;
using System.Diagnostics;

namespace OpenDental.Bridges
{
    ///<summary></summary>
    public class MeditLink
    {

        private const string UUID = "E02698AC-C1AE-4699-9307-F4A58DED47FE";

        ///<summary></summary>
        public MeditLink()
        {

        }

        ///<summary></summary>
        public static void SendData(Program program, Patient patient)
        {
            //Purposefully surround all command... argument identifiers sent to MeditLink in double quotes.
            string commandArgs = "\"--commandUuid\" \"" + UUID + "\"";
            string path = Programs.GetProgramPath(program);
            if (patient != null)
            {
                string optionalParameters = program.CommandLine;
                optionalParameters = Patients.ReplacePatient(optionalParameters, patient);
                commandArgs += " \"--commandParam\" \"" + optionalParameters + "\"";
            }
            try
            {
                Process.Start(path, commandArgs);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return;
        }
    }
}








