﻿using OpenDental;
using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace CentralManager
{
    public partial class FormCentralReportSetup : FormODBase
    {
        ///<summary>Either the currently logged in user or the user of a group selected in the Security window.</summary>
        private long _userGroupNum;
        private bool _isPermissionMode;

        public FormCentralReportSetup(long userGroupNum, bool isPermissionMode)
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
            _userGroupNum = userGroupNum;
            _isPermissionMode = isPermissionMode;
        }

        private void FormCentralReportSetup_Load(object sender, EventArgs e)
        {
            userControlReportSetup.InitializeOnStartup(true, _userGroupNum, _isPermissionMode, true);
            if (_isPermissionMode)
            {
                tabControl1.SelectedIndex = 1;
            }
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            GroupPermissions.Sync(userControlReportSetup.ListGroupPermissionsForReports, userControlReportSetup.ListGroupPermissionsOld);
            GroupPermissions.RefreshCache();
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}