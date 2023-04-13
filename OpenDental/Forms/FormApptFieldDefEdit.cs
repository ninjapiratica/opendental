using OpenDentBusiness;
using System;
using System.Windows.Forms;

namespace OpenDental
{
    /// <summary>
    /// </summary>
    public partial class FormApptFieldDefEdit : FormODBase
    {
        ///<summary></summary>
        public bool IsNew;
        public ApptFieldDef ApptFieldDef;
        private string _fieldNameOld;

        ///<summary></summary>
        public FormApptFieldDefEdit()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormApptFieldDefEdit_Load(object sender, System.EventArgs e)
        {
            textName.Text = ApptFieldDef.FieldName;
            textPickList.Visible = false;
            comboFieldType.Items.Clear();
            comboFieldType.Items.AddList<string>(Enum.GetNames(typeof(ApptFieldType)));
            comboFieldType.SelectedIndex = (int)ApptFieldDef.FieldType;
            if (!IsNew)
            {
                _fieldNameOld = ApptFieldDef.FieldName;
            }
            if (comboFieldType.SelectedIndex == (int)ApptFieldType.PickList)
            {
                textPickList.Visible = true;
                labelWarning.Visible = true;
                textPickList.Text = ApptFieldDef.PickList;
            }
        }

        private void comboFieldType_SelectedIndexChanged(object sender, EventArgs e)
        {
            textPickList.Visible = false;
            labelWarning.Visible = false;
            if (comboFieldType.SelectedIndex == (int)ApptFieldType.PickList)
            {
                textPickList.Visible = true;
                labelWarning.Visible = true;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (IsNew)
            {
                DialogResult = DialogResult.Cancel;
                return;
            }
            try
            {
                ApptFieldDefs.Delete(ApptFieldDef);//Throws if in use.
                FieldDefLinks.DeleteForFieldDefNum(ApptFieldDef.ApptFieldDefNum, FieldDefTypes.Appointment);//Delete any FieldDefLinks to this ApptFieldDef
                ApptFieldDef = null;
                ApptFieldDefs.RefreshCache();
                DialogResult = DialogResult.OK;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butOK_Click(object sender, System.EventArgs e)
        {
            if (_fieldNameOld != textName.Text)
            {
                if (ApptFieldDefs.GetExists(x => x.FieldName == textName.Text))
                {
                    MsgBox.Show(this, "Field name currently being used.");
                    return;
                }
            }
            ApptFieldDef.FieldName = textName.Text;
            ApptFieldDef.FieldType = (ApptFieldType)comboFieldType.SelectedIndex;
            if (ApptFieldDef.FieldType == ApptFieldType.PickList)
            {
                if (textPickList.Text == "")
                {
                    MsgBox.Show(this, "List cannot be blank.");
                    return;
                }
                ApptFieldDef.PickList = textPickList.Text;
            }
            if (IsNew)
            {
                ApptFieldDefs.Insert(ApptFieldDef);
            }
            else
            {
                ApptFieldDefs.Update(ApptFieldDef, _fieldNameOld);
            }
            ApptFieldDefs.RefreshCache();
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}





















