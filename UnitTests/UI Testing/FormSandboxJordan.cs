﻿using OpenDental;
using System.Drawing;

namespace UnitTests
{
    public partial class FormSandboxJordan : FormODBase
    {
        public FormSandboxJordan()
        {
            InitializeComponent();
            InitializeLayoutManager();
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            //MsgBox.Show(DeviceDpi.ToString());
            Form4kTests form4kTests = new Form4kTests();
            form4kTests.Show();
        }

        private void FormSandboxJordan_Load(object sender, System.EventArgs e)
        {
            //this.FormBorderStyle=FormBorderStyle.None;
            Rectangle rectangle = new Rectangle(596, 427, 5, 5);
            int x = 600;
            int y = 432;
            bool doesContain = rectangle.Contains(x, y);
        }
    }
}
