using CodeBase;
using OpenDentBusiness;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormSheetFieldStatic : FormODBase
    {
        ///<summary>This is the object we are editing.</summary>
        public SheetFieldDef SheetFieldDefCur;
        ///<summary>We need access to a few other fields of the sheetDef.</summary>
        public SheetDef SheetDefCur;
        public bool IsEditMobile;
        public bool IsReadOnly;
        private int _indexTextSelectionStart;

        public FormSheetFieldStatic()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormSheetFieldStatic_Load(object sender, EventArgs e)
        {
            textYPos.MaxVal = SheetDefCur.HeightTotal - 1;//The maximum y-value of the sheet field must be within the page vertically.
            if (IsEditMobile)
            { //When we open this form from the mobile layout window
                groupBox1.Enabled = false;
                comboGrowthBehavior.Enabled = false;
                checkPmtOpt.Enabled = false;
            }
            if (IsReadOnly)
            {
                butOK.Enabled = false;
                butDelete.Enabled = false;
            }
            if (SheetDefCur.SheetType != SheetTypeEnum.Statement)
            {
                checkPmtOpt.Visible = false;
            }
            if (SheetDefCur.SheetType == SheetTypeEnum.PatientLetter)
            {
                butExamSheet.Visible = true;
            }
            else
            {
                butExamSheet.Visible = false;
            }
            if (SheetDefs.IsDashboardType(SheetDefCur))
            {
                comboGrowthBehavior.Enabled = false;
            }
            checkIncludeInMobile.Visible = SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
            //Show/hide in mobile editor depending on if TabOrderMobile has been previously set. This is how we will selectively include only desireable StaticText fields.
            checkIncludeInMobile.Checked = SheetFieldDefCur.TabOrderMobile >= 1;
            if (SheetFieldDefCur.IsNew)
            {
                checkIsLocked.Checked = true;
            }
            else
            {
                checkIsLocked.Checked = SheetFieldDefCur.IsLocked;
            }
            textFieldValue.Text = SheetFieldDefCur.FieldValue;
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            for (int i = 0; i < installedFontCollection.Families.Length; i++)
            {
                comboFontName.Items.Add(installedFontCollection.Families[i].Name);
                if (!SheetFieldDefCur.FontName.IsNullOrEmpty() && SheetFieldDefCur.FontName == installedFontCollection.Families[i].Name)
                {
                    comboFontName.SetSelected(i);
                }
            }
            if (comboFontName.SelectedIndex == -1)
            { //The sheetfield's current font is not in the list of installed fonts on this machine
              //Add the font to the combobox and mark it as missing. That way office can decided to either keep or change the missing font used for this field
                comboFontName.Items.Add(SheetFieldDefCur.FontName + " (missing)", SheetFieldDefCur.FontName);
                comboFontName.SetSelected(comboFontName.Items.Count - 1);
            }
            numFontSize.Value = (decimal)SheetFieldDefCur.FontSize;
            checkFontIsBold.Checked = SheetFieldDefCur.FontIsBold;
            SheetUtilL.FillComboGrowthBehavior(comboGrowthBehavior, SheetFieldDefCur.GrowthBehavior);
            for (int i = 0; i < Enum.GetNames(typeof(System.Windows.Forms.HorizontalAlignment)).Length; i++)
            {
                comboTextAlign.Items.Add(Enum.GetNames(typeof(System.Windows.Forms.HorizontalAlignment))[i]);
                if ((int)SheetFieldDefCur.TextAlign == i)
                {
                    comboTextAlign.SelectedIndex = i;
                }
            }
            textXPos.Text = SheetFieldDefCur.XPos.ToString();
            textYPos.Text = SheetFieldDefCur.YPos.ToString();
            textWidth.Text = SheetFieldDefCur.Width.ToString();
            textHeight.Text = SheetFieldDefCur.Height.ToString();
            checkPmtOpt.Checked = SheetFieldDefCur.IsPaymentOption;
            butColor.BackColor = SheetFieldDefCur.ItemColor;
            FillFields();
        }

        private void FillFields()
        {
            List<StaticTextField> listStaticTextFields = StaticTextFields.GetForType(SheetDefCur.SheetType);
            this.listFields.Items.Clear();
            for (int i = 0; i < listStaticTextFields.Count; i++)
            {
                this.listFields.Items.Add(listStaticTextFields[i].GetDescription());
            }
        }

        private void ListFields_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            //SelectedItem will be null if the user clicks inside the ListBox but not on an item in the list.
            if (sender.GetType() != typeof(ListBox) || ((ListBox)sender).SelectedItem == null)
            {
                return;
            }
            string fieldStr = ((ListBox)sender).SelectedItem.ToString();
            if (_indexTextSelectionStart < textFieldValue.Text.Length - 1)
            {
                textFieldValue.Text = textFieldValue.Text.Substring(0, _indexTextSelectionStart)
                    + "[" + fieldStr + "]"
                    + textFieldValue.Text.Substring(_indexTextSelectionStart);
            }
            else
            {//otherwise, just tack it on the end
                textFieldValue.Text += "[" + fieldStr + "]";
            }
            textFieldValue.Select(_indexTextSelectionStart + fieldStr.Length + 2, 0);
            textFieldValue.Focus();
            listFields.ClearSelected();
            //if(!textFieldValue.Focused){
            //	textFieldValue.Text+="["+fieldStr+"]";
            //	return;
            //}
            //MessageBox.Show(textFieldValue.SelectionStart.ToString());
        }

        private void textFieldValue_Leave(object sender, EventArgs e)
        {
            _indexTextSelectionStart = textFieldValue.SelectionStart;
        }

        /// <summary>This method is tied to any event that could change text size, such as font size, text, or the Bold checkbox.</summary>
        private void UpdateTextSizeLabels(object sender, EventArgs e)
        {
            float fontSize = (float)numFontSize.Value;
            FontStyle fontStyle = FontStyle.Regular;
            if (checkFontIsBold.Checked)
            {
                fontStyle = FontStyle.Bold;
            }
            Size sizeText;
            using Font font = new Font(comboFontName.Text, fontSize, fontStyle);
            //If we measure using a graphics object, it will report the size of the text if we drew it with a graphics object.
            //This correctly reports the text size for how we are drawing the text.
            sizeText = TextRenderer.MeasureText(textFieldValue.Text, font);
            labelTextW.Text = Lan.g(this, "TextW:") + " " + sizeText.Width.ToString();
            labelTextH.Text = Lan.g(this, "TextH:") + " " + sizeText.Height.ToString();
        }

        private void butExamSheet_Click(object sender, EventArgs e)
        {
            using FormSheetFieldExam formSheetFieldExam = new FormSheetFieldExam();
            formSheetFieldExam.ShowDialog();
            if (formSheetFieldExam.DialogResult != DialogResult.OK)
            {
                return;
            }
            if (_indexTextSelectionStart < textFieldValue.Text.Length - 1)
            {//if cursor is not at the end of the text in textFieldValue, insert into text beginning at cursor
                textFieldValue.Text = textFieldValue.Text.Substring(0, _indexTextSelectionStart)
                + "[" + formSheetFieldExam.ExamFieldSelected + "]"
                + textFieldValue.Text.Substring(_indexTextSelectionStart);
            }
            else
            {//otherwise, just tack it on the end
                textFieldValue.Text += "[" + formSheetFieldExam.ExamFieldSelected + "]";
            }
            textFieldValue.Select(_indexTextSelectionStart + formSheetFieldExam.ExamFieldSelected.Length + 2, 0);
            textFieldValue.Focus();
        }

        private void butColor_Click(object sender, EventArgs e)
        {
            using ColorDialog colorDialog1 = new ColorDialog();
            colorDialog1.Color = butColor.BackColor;
            colorDialog1.ShowDialog();
            butColor.BackColor = colorDialog1.Color;
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            SheetFieldDefCur = null;
            DialogResult = DialogResult.OK;
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (!textXPos.IsValid()
                || !textYPos.IsValid()
                || !textWidth.IsValid()
                || !textHeight.IsValid())
            {
                MsgBox.Show(this, "Please fix data entry errors first.");
                return;
            }
            if (textFieldValue.Text == "")
            {
                MsgBox.Show(this, "Please set a field value first.");
                return;
            }
            List<string> listStrsInvalid = XmlConverter.XmlFindAllInvalidChars(textFieldValue.Text);
            if (listStrsInvalid.Count > 0)
            {
                MsgBox.Show(this, "Invalid characters found. Please remove or replace the following: " + string.Join(", ", listStrsInvalid));
                return;
            }
            if (comboFontName.GetSelected<string>() == "" || comboFontName.GetSelected<string>() == null)
            {
                //not going to bother testing for validity unless it will cause a crash.
                MsgBox.Show(this, "Please select a font name first.");
                return;
            }
            float fontSize = (float)numFontSize.Value;
            if (fontSize < 2)
            {
                MsgBox.Show(this, "Font size is invalid.");
                return;
            }
            if (SheetDefs.IsDashboardType(SheetDefCur)
                && textFieldValue.Text.ToLower().Contains($"[{StaticTextField.patientPortalCredentials.ToString().ToLower()}]"))
            {
                MsgBox.Show(this, "The [patientPortalCredentials] tag is not allowed in Dashboards.");
                return;
            }
            try
            {// check if font is compatible with PDFSharp by running it through XFont, if it suceeds, add to the list, otherwise throw error.
                XFont _ = new XFont(comboFontName.GetSelected<string>(), fontSize, XFontStyle.Regular);
            }
            catch
            {
                MsgBox.Show(Lan.g(this, $"Unsupported font: {comboFontName.GetSelected<string>()}. Please choose another font."));
                return;
            }
            SheetFieldDefCur.FieldValue = textFieldValue.Text;
            SheetFieldDefCur.FontName = comboFontName.GetSelected<string>();
            SheetFieldDefCur.FontSize = fontSize;
            SheetFieldDefCur.FontIsBold = checkFontIsBold.Checked;
            SheetFieldDefCur.XPos = PIn.Int(textXPos.Text);
            SheetFieldDefCur.YPos = PIn.Int(textYPos.Text);
            SheetFieldDefCur.Width = PIn.Int(textWidth.Text);
            SheetFieldDefCur.Height = PIn.Int(textHeight.Text);
            SheetFieldDefCur.GrowthBehavior = comboGrowthBehavior.GetSelected<GrowthBehaviorEnum>();
            SheetFieldDefCur.TextAlign = (System.Windows.Forms.HorizontalAlignment)comboTextAlign.SelectedIndex;
            SheetFieldDefCur.IsPaymentOption = checkPmtOpt.Checked;
            SheetFieldDefCur.ItemColor = butColor.BackColor;
            SheetFieldDefCur.IsLocked = checkIsLocked.Checked;
            if (checkIncludeInMobile.Checked && SheetDefs.IsMobileAllowed(SheetDefCur.SheetType))
            { //Had previously been hidden from mobile layout so show and set to top. User can re-order using the mobile editor.
                SheetFieldDefCur.TabOrderMobile = 1;
            }
            else
            {
                SheetFieldDefCur.TabOrderMobile = 0;
            }
            //don't save to database here.
            SheetFieldDefCur.IsNew = false;
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}