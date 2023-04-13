using System;

namespace OpenDentalGraph
{
    public partial class HqMessagesRealTimeOptionsCtrl : BaseGraphOptionsCtrl
    {
        public enum HQGrouping { country, msgtype, customer };

        public HQGrouping CurHQGroup
        {
            get
            {
                if (radioCountryCode.Checked)
                {
                    return HQGrouping.country;
                }
                else if (radioMsgType.Checked)
                {
                    return HQGrouping.msgtype;
                }
                else
                {
                    return HQGrouping.customer;
                }
            }
            set
            {
                switch (value)
                {
                    case HQGrouping.country:
                        radioCountryCode.Checked = true;
                        break;
                    case HQGrouping.msgtype:
                        radioMsgType.Checked = true;
                        break;
                    case HQGrouping.customer:
                    default:
                        radioCustomer.Checked = true;
                        break;
                }
            }
        }


        public HqMessagesRealTimeOptionsCtrl()
        {
            InitializeComponent();
        }

        public override int GetPanelHeight()
        {
            return this.Height;
        }

        public override bool HasGroupOptions
        {
            get
            {
                return false;
            }
        }

        private void OnBrokenApptGraphOptionsChanged(object sender, EventArgs e)
        {
            OnBaseInputsChanged(sender, e);
        }
    }
}
