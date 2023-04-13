﻿using CodeBase;
using OpenDentBusiness;
using System.Diagnostics;

namespace OpenDental.Bridges
{

    public class AiDental
    {
        ///<summary></summary>
        public AiDental()
        {

        }

        ///<summary></summary>
        public static void SendData(Program program, Patient patient)
        {
            string processPath = Programs.GetProgramPath(program);
            if (patient == null)
            {
                try
                {
                    Process.Start(processPath);
                }
                catch
                {
                    MessageBox.Show(processPath + " is not available.");
                }
                return;
            }
            string info = program.CommandLine;
            info = Patients.ReplacePatient(info, patient);
            try
            {
                ODFileUtils.ProcessStart(processPath, info);
            }
            catch
            {
                MessageBox.Show(processPath + " is not available, or there is an error in the command line options.");
            }
        }

        ///<summary>Removes semicolons and spaces.</summary>
        private static string Tidy(string input)
        {
            string retVal = input.Replace(";", "");//get rid of any semicolons.
            retVal = retVal.Replace(" ", "");
            return retVal;
        }

    }
}







