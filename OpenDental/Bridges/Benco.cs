using OpenDentBusiness;
using System;
using System.Diagnostics;

namespace OpenDental.Bridges
{
    public class Benco
    {

        ///<summary></summary>
        public Benco()
        {

        }

        ///<summary></summary>
        public static void SendData(Program ProgramCur)
        {
            string path = ProgramCur.Path;
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                FriendlyException.Show(Lans.g("Benco", "Unable to launch") + " " + ProgramCur.ProgDesc + ".", ex);
            }
        }

    }
}