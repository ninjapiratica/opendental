﻿using System;

namespace OpenDentBusiness
{
    ///<summary>This table is not part of the general release.  User would have to add it manually.
    ///Most schema changes are done directly on our live database as needed.  Used to link Jobs to Sprints</summary>
    [Serializable]
    [CrudTable(IsMissingInGeneral = true)]
    public class JobSprintLink : TableBase
    {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey = true)]
        public long JobSprintLinkNum;
        ///<summary>FK to Job.</summary>
        public long JobNum;
        ///<summary>FK to JobSprint.</summary>
        public long JobSprintNum;
    }
}


