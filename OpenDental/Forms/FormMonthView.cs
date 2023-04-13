using System;

namespace OpenDental
{
    public partial class FormMonthView : FormODBase
    {
        public FormMonthView()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormMonthView_Load(object sender, EventArgs e)
        {

        }

        private void butClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}