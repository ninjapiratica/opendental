﻿using System;

namespace OpenDental.User_Controls.SetupWizard
{
    public partial class UserControlSetupWizComplete : SetupWizControl
    {
        public UserControlSetupWizComplete(string name)
        {
            InitializeComponent();
            labelDone.Text = Lan.g(this, "Congratulations! You have finished setting up your " + name + "!");
            labelEnd.Text = "\r\n" + Lan.g(this, "You can always go back through this setup wizard if you need to make any modifications to your " + name + ".");
        }

        private void UserControlSetupWizComplete_Load(object sender, EventArgs e)
        {
            IsDone = true;
            //OnControlDone?.Invoke(sender,e);
        }
    }
}
