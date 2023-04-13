﻿using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace OpenDental.UI.Design
{
    //[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")] 
    public class TabControlDesigner : ParentControlDesigner
    {

        public TabControlDesigner()
        {
            //Trace.WriteLine("TabControlDesigner ctor");
            EnableDragDrop(true);
        }

        protected override bool GetHitTest(Point pointScreen)
        {
            Point point = Control.PointToClient(pointScreen);
            TabControl tabControl = (TabControl)Control;
            for (int i = 0; i < tabControl.ListRectanglesTabs.Count; i++)
            {
                if (tabControl.ListRectanglesTabs[i].Contains(point))
                {
                    return true;
                }
            }
            return false;
        }

        //We are not using Behavior:
        //https://stackoverflow.com/questions/8537023/enable-a-button-to-be-clicked-at-design-time-in-visual-studio


    }
}
