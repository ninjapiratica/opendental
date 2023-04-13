using CodeBase;

namespace OpenDental.Bridges
{
    ///<summary>Link to CDS Backup Solutions.</summary>
    public class CDS
    {

        ///<summary></summary>
        public CDS()
        {

        }

        ///<summary></summary>
        public static void ShowPage()
        {
            try
            {
                ODFileUtils.ProcessStart("http://www.opendental.com/resources/redirects/redirectcds.html");
            }
            catch
            {
                MsgBox.Show("CDS", "Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet then try again.");
            }
        }


    }
}







