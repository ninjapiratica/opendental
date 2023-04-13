﻿using System;

namespace OpenDentBusiness
{
    ///<summary>Replication server information. Used for server specific replication settings, manually entered by the user.  Each row is one server.</summary>
    [Serializable()]
    public class ReplicationServer : TableBase
    {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey = true)]
        public long ReplicationServerNum;
        ///<summary>The description or name of the server.  Optional.</summary>
        [CrudColumn(SpecialType = CrudSpecialColType.IsText)]
        public string Descript;
        ///<summary>Db admin sets this server_id server variable on each replication server.  Allows us to know what server each workstation is connected to.  In display, it's ordered by this value.  Users are always forced to enter a value here.</summary>
        public int ServerId;
        ///<summary>Deprecated. Only used for Random Primary Keys. The start of the key range for this server.  0 if no value entered yet.</summary>
        public long RangeStart;
        ///<summary>Deprecated. Only used for Random Primary Keys. The end of the key range for this server.  0 if no value entered yet.</summary>
        public long RangeEnd;
        ///<summary>The AtoZpath for this server. Optional.</summary>
        public string AtoZpath;
        ///<summary>If true, then this server cannot initiate an update.  Typical for satellite servers.</summary>
        public bool UpdateBlocked;
        ///<summary>Deprecated. Monitoring the status of replication is now monitored by a separate service. See online manual for information on installing the new service. The description or name of the comptuer that will monitor replication for this server.</summary>
        public string SlaveMonitor;

        public ReplicationServer Copy()
        {
            return (ReplicationServer)this.MemberwiseClone();
        }
    }
}