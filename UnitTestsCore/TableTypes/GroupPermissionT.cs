﻿using OpenDentBusiness;

namespace UnitTestsCore
{
    public class GroupPermissionT
    {
        //<summary>Deletes everything from the grouppermission table.  Does not truncate the table so that PKs are not reused on accident.</summary>
        public static void ClearGroupPermissionTable()
        {
            //The first ~300 group permissions are from the original SQL dump (and initialization) and need to stick around no matter what.
            string command = "DELETE FROM grouppermission WHERE GroupPermNum >= 293";
            DataCore.NonQ(command);
        }
    }
}
