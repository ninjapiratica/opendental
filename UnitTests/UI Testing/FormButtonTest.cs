using OpenDental;
using System;
using System.Drawing;

namespace UnitTests
{
    public partial class FormButtonTest : FormODBase
    {
        public FormButtonTest()
        {
            InitializeComponent();
            InitializeLayoutManager();
        }

        private void FormButtonTest_Load(object sender, EventArgs e)
        {
            //CodeBase.OdThemeModernGrey.SetTheme(CodeBase.OdTheme.MonoFlatBlue);
            Size sizeCanvas = new Size(1000, 1000);
            Size sizeImage = new Size(500, 800);
            zoomSlider1.SetValueInitialFit(sizeCanvas, sizeImage, 0);
        }

        private void ButDeleteProc_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            button2.Text = "Longer text result";
        }
    }
}
