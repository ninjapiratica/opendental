﻿namespace OpenDentBusiness
{
    public static class ODSftp
    {

        private static long _progNum = 0;

        public static long ProgramNum
        {
            get
            {
                if (_progNum == 0)
                {
                    _progNum = Programs.GetProgramNum(ProgramName.SFTP);
                }
                return _progNum;
            }
        }

        public class PropertyDescs
        {
            public static string UserName = "FTP User Name";
            public static string Password = "FTP Password";
            public static string SftpHostname = "FTP Hostname";
            public static string AtoZPath = "FTP AtoZ Path";
        }

        public static string UserName
        {
            get { return ProgramProperties.GetPropVal(ProgramNum, PropertyDescs.UserName); }
        }

        public static string Password
        {
            get
            {
                string passEncrypted = ProgramProperties.GetPropVal(ProgramNum, PropertyDescs.Password);
                string passDecrypted = "";
                CDT.Class1.DecryptSftp(passEncrypted, out passDecrypted);
                return passDecrypted;
            }
        }

        public static string Hostname
        {
            get { return ProgramProperties.GetPropVal(ProgramNum, PropertyDescs.SftpHostname); }
        }

        public static string AtoZPath
        {
            get { return ProgramProperties.GetPropVal(ProgramNum, PropertyDescs.AtoZPath); }
        }

    }
}
