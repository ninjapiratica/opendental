﻿using OpenDentBusiness;

namespace UnitTestsCore
{
    public class DefLinkT
    {

        public static DefLink CreateDefLink(long defNum, long fKey, DefLinkType linkType)
        {
            DefLink defLink = new DefLink()
            {
                DefNum = defNum,
                FKey = fKey,
                LinkType = linkType,
            };
            DefLinks.Insert(defLink);
            return defLink;
        }

        ///<summary>Deletes everything from the deflink table.  Does not truncate the table so that PKs are not reused on accident.</summary>
        public static void ClearDefLinkTable()
        {
            string command = "DELETE FROM deflink WHERE DefLinkNum > 0";
            DataCore.NonQ(command);
        }

    }
}
