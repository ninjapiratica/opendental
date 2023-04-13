using System;

namespace OpenDentalGraph
{
    public partial class IncomeGraphOptionsCtrl : BaseGraphOptionsCtrl
    {
        #region Properties
        public bool IncludePaySplits
        {
            get { return checkIncludePaySplits.Checked; }
            set { checkIncludePaySplits.Checked = value; }
        }
        public bool IncludeInsuranceClaimPayments
        {
            get { return checkIncludeInsuranceClaimPayments.Checked; }
            set { checkIncludeInsuranceClaimPayments.Checked = value; }
        }
        #endregion

        public IncomeGraphOptionsCtrl()
        {
            InitializeComponent();
        }

        public override int GetPanelHeight()
        {
            return this.Height;
        }

        private void OnIncomeGraphInputsChanged(object sender, EventArgs e)
        {
            OnBaseInputsChanged(sender, e);
        }
    }
}
