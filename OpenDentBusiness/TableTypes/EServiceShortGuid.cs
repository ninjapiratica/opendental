﻿using CodeBase;
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace OpenDentBusiness
{
    ///<summary>Linker table to associate ShortGuids, usually generated at ODHQ, to various eService related entities.</summary>
    [Serializable, CrudTable(HasBatchWriteMethods = true)]
    public class EServiceShortGuid : TableBase
    {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey = true)]
        public long EServiceShortGuidNum;
        ///<summary>Enum:eServiceCode EService that this short GUID applies to.</summary>
        [CrudColumn(SpecialType = CrudSpecialColType.EnumAsString)]
        public eServiceCode EServiceCode;
        ///<summary>A unique alphanumeric string that identifies something.</summary>
        public string ShortGuid;
        ///<summary>URL generated by HQ.</summary>
        public string ShortURL;
        ///<summary>Usually identifies the object that is linked to ShortGUID.</summary>
        public long FKey;
        ///<summary>Describes the type of object referenced by the FKey.</summary>
        [CrudColumn(SpecialType = CrudSpecialColType.EnumAsString)]
        public EServiceShortGuidKeyType FKeyType;
        ///<summary>Timestamp at which this short GUID will expire..</summary>
        [CrudColumn(SpecialType = CrudSpecialColType.DateT)]
        public DateTime DateTimeExpiration;
        ///<summary>The exact server time when this EServiceShortGuid was entered into db.  Handled automatically.</summary>
        [CrudColumn(SpecialType = CrudSpecialColType.DateTEntry)]
        public DateTime DateTEntry;
        ///<summary>True if DateTimeExpiration is in the past. Otherwise false. Note: Time portion of DateTime IS considered, not just date.</summary>
        [XmlIgnore, JsonIgnore]
        public bool HasExpired
        {
            get
            {
                return DateTimeExpiration < DateTime_.Now;
            }
        }

        ///<summary>Returns a copy of the ShortGuidLink.</summary>
        public EServiceShortGuid Copy()
        {
            return (EServiceShortGuid)MemberwiseClone();
        }
    }

    public enum EServiceShortGuidKeyType
    {
        Undefined = 0,
        eClipboardApptByod,
    }

}
