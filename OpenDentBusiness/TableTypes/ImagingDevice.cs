using System;
using System.ComponentModel;

namespace OpenDentBusiness
{
    /// <summary>Xray sensor, camera, etc. Depending on the hardware, this can either be one physical device or a set of similar devices.</summary>
    [Serializable()]
    public class ImagingDevice : TableBase
    {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey = true)]
        public long ImagingDeviceNum;
        /// <summary>Any description of the device.</summary>
        public string Description;
        /// <summary>Name of the computer where this device is available.  Optional.  If blank, then this device will be available to all computers.</summary>
        public string ComputerName;
        ///<summary>Enum:EnumImgDeviceType </summary>
        public EnumImgDeviceType DeviceType;
        ///<summary>The name of the twain device as in Windows.</summary>
        public string TwainName;
        ///<summary></summary>
        public int ItemOrder;
        ///<summary></summary>
        public bool ShowTwainUI;

        ///<summary></summary>
        public ImagingDevice Copy()
        {
            return (ImagingDevice)this.MemberwiseClone();
        }



    }

    ///<summary>Order cannot change, since we store in db as enum number.  But we show the list in the UI in a different order or our choosing.</summary>
    public enum EnumImgDeviceType
    {
        ///<summary>0</summary>
        TwainRadiograph,
        ///<summary>1</summary>
        [Description("XDR (not functional)")]
        XDR,
        ///<summary>2</summary>
        TwainMulti
    }
}
