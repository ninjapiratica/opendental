using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormJobContainer : FormODBase
    {
        public FormJobContainer(Control control, string title)
        {
            InitializeComponent();
            InitializeLayoutManager();
            LayoutManager.Add(control, this);
            LayoutManager.MoveWidth(this, control.Width + 10);
            LayoutManager.MoveHeight(this, control.Height + 10);
            control.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);//Allows resizing the control in this window.
            this.Text = title;
        }
    }
}
