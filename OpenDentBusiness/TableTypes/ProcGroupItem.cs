﻿using System;

namespace OpenDentBusiness
{
    ///<summary>Links Procedures(groupnotes) to Procedures in a 1-n relationship.</summary>
    [Serializable]
    [CrudTable(HasBatchWriteMethods = true)]
    public class ProcGroupItem : TableBase
    {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey = true)]
        public long ProcGroupItemNum;
        ///<summary>FK to procedurelog.ProcNum.</summary>
        public long ProcNum;
        ///<summary>FK to procedurelog.ProcNum.This is the group note that the procedure is in.</summary>
        public long GroupNum;

        ///<summary></summary>
        public ProcGroupItem Clone()
        {
            return (ProcGroupItem)this.MemberwiseClone();
        }

    }
}