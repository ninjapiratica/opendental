using CodeBase;
using OpenDentBusiness;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class FormSheetDef : FormODBase
    {
        ///<summary>This holds the default values of the sheet dimensions, but once we click ok the users settings will override.</summary>
        public SheetDef SheetDefCur;
        //private List<SheetFieldDef> AvailFields;
        public bool IsReadOnly;
        ///<summary>On creation of a new sheetdef, the user must pick a description and a sheettype before allowing to start editing the sheet.  After the initial sheettype selection, this will be false, indicating that the user may not change the type.</summary>
        public bool IsInitial;
        ///<summary>The Autosave feature needs to be considered when there is at least one image category flagged to Autosave Forms.</summary>
        private bool _doConsiderAutoSave;

        public FormSheetDef()
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
        }

        private void FormSheetDef_Load(object sender, EventArgs e)
        {
            SetHeightWidthMin();
            if (IsReadOnly)
            {
                butOK.Enabled = false;
            }
            if (!IsInitial)
            {
                listSheetType.Enabled = false;
                checkHasMobileLayout.Enabled = SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
            }
            _doConsiderAutoSave = Defs.GetDefsForCategory(DefCat.ImageCats).Any(x => x.ItemValue.Contains("U"));//Any are checked to auto save
            textDescription.Text = SheetDefCur.Description;
            Func<SheetTypeEnum, string> funcItemToString = new Func<SheetTypeEnum, string>((sheetType) => { return Lan.g("enumSheetTypeEnum", sheetType.GetDescription()); });
            //not allowed to change sheettype once created.
            List<SheetTypeEnum> listSheetTypeEnums = Enum.GetValues(typeof(SheetTypeEnum)).Cast<SheetTypeEnum>().ToList();
            listSheetTypeEnums = listSheetTypeEnums.FindAll(x => !x.In(SheetTypeEnum.None, SheetTypeEnum.MedLabResults) && !SheetDefs.IsDashboardType(x));
            listSheetTypeEnums = listSheetTypeEnums.OrderBy(x => funcItemToString(x)).ToList(); //Order alphabetical.
            listSheetType.Items.AddList(listSheetTypeEnums, funcItemToString); //funcItemToString contains the text displayed for each item
            if (!IsInitial)
            {
                for (int i = 0; i < listSheetType.Items.Count; i++)
                {
                    if ((SheetTypeEnum)listSheetType.Items.GetObjectAt(i) == SheetDefCur.SheetType)
                    {
                        listSheetType.SetSelected(i);
                    }
                }
            }
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            for (int i = 0; i < installedFontCollection.Families.Length; i++)
            {
                comboFontName.Items.Add(installedFontCollection.Families[i].Name);
                if (installedFontCollection.Families[i].Name.ToLower() == SheetDefCur.FontName.ToLower())
                {
                    comboFontName.SelectedIndex = i;
                }
            }
            if (comboFontName.SelectedIndex == -1)
            { //The sheetfield's current font is not in the list of installed fonts on this machine
              //Add the font to the combobox and mark it as missing. That way office can decided to either keep or change the missing font used for this field
                comboFontName.Items.Add(SheetDefCur.FontName + " (missing)", SheetDefCur.FontName);
                comboFontName.SetSelected(comboFontName.Items.Count - 1);
            }
            checkBypassLockDate.Checked = (SheetDefCur.BypassGlobalLock == BypassLockStatus.BypassAlways);
            textFontSize.Text = SheetDefCur.FontSize.ToString();
            textWidth.Text = SheetDefCur.Width.ToString();
            textHeight.Text = SheetDefCur.Height.ToString();
            checkIsLandscape.Checked = SheetDefCur.IsLandscape;
            checkHasMobileLayout.Checked = SheetDefCur.HasMobileLayout;
            checkAutoSaveCheck.Checked = SheetDefCur.AutoCheckSaveImage;
            checkAutoSaveCheck.Enabled = SetAutoCheckEnabled(SheetDefCur.SheetType, isLoading: true);
            //Load is done. It is now safe to register for the selection change event.
            listSheetType.SelectedIndexChanged += new EventHandler(listSheetType_SelectedIndexChanged);
            if (SheetDefs.IsDashboardType(SheetDefCur))
            {
                labelSheetType.Visible = false;
                listSheetType.Visible = false;
                checkBypassLockDate.Visible = false;
                checkIsLandscape.Visible = false;
                checkHasMobileLayout.Visible = false;
            }
        }

        private bool SetAutoCheckEnabled(SheetTypeEnum sheetType, bool isLoading = false)
        {
            if (isLoading && _doConsiderAutoSave)
            {
                checkAutoSaveCheck.Visible = true;
            }
            return _doConsiderAutoSave && EnumTools.GetAttributeOrDefault<SheetTypeAttribute>(sheetType).CanAutoSave;
        }

        ///<summary>Sets the minimum valid value (used for validation only) of the appropriate Height or Width field based on the bottom of the lowest field. Max values are set in the designer.</summary>
        private void SetHeightWidthMin()
        {
            textHeight.MinVal = 10;//default values
            textWidth.MinVal = 10;//default values
            if (SheetDefCur.SheetFieldDefs == null)
            {
                //New sheet
                return;
            }
            int minVal = int.MaxValue;
            for (int i = 0; i < SheetDefCur.SheetFieldDefs.Count; i++)
            {
                minVal = Math.Min(minVal, SheetDefCur.SheetFieldDefs[i].Bounds.Bottom / SheetDefCur.PageCount);
            }
            if (minVal == int.MaxValue)
            {
                //Sheet has no sheet fields.
                return;
            }
            if (checkIsLandscape.Checked)
            {
                //Because Width is used to measure vertical sheet size.
                textWidth.MinVal = minVal;
            }
            else
            {
                //Because Height is used to measure vertical sheet size.
                textHeight.MinVal = minVal;
            }
        }

        private void checkIsLandscape_Click(object sender, EventArgs e)
        {
            SetHeightWidthMin();
        }

        private void listSheetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsInitial)
            {
                return;
            }
            if (listSheetType.SelectedIndex == -1)
            {
                return;
            }
            SheetDef sheetDef = null;
            checkHasMobileLayout.Enabled = false;
            SheetTypeEnum sheetTypeEnumSelected = listSheetType.GetSelected<SheetTypeEnum>();
            if (SheetDefs.IsMobileAllowed(sheetTypeEnumSelected))
            {
                checkHasMobileLayout.Enabled = true;
            }
            checkAutoSaveCheck.Enabled = SetAutoCheckEnabled(sheetTypeEnumSelected);
            if (!checkAutoSaveCheck.Enabled)
            {
                checkAutoSaveCheck.Checked = false;//if the checkbox is disabled then showing it as checked would be confusing
            }
            switch (sheetTypeEnumSelected)
            {
                case SheetTypeEnum.LabelCarrier:
                case SheetTypeEnum.LabelPatient:
                case SheetTypeEnum.LabelReferral:
                    sheetDef = SheetsInternal.GetSheetDef(SheetInternalType.LabelPatientMail);
                    if (textDescription.Text == "")
                    {
                        textDescription.Text = sheetTypeEnumSelected.GetDescription();
                    }
                    textFontSize.Text = sheetDef.FontSize.ToString();
                    textWidth.Text = sheetDef.Width.ToString();
                    textHeight.Text = sheetDef.Height.ToString();
                    checkIsLandscape.Checked = sheetDef.IsLandscape;
                    break;
                case SheetTypeEnum.ReferralSlip:
                    sheetDef = SheetsInternal.GetSheetDef(SheetInternalType.ReferralSlip);
                    if (textDescription.Text == "")
                    {
                        textDescription.Text = sheetTypeEnumSelected.GetDescription();
                    }
                    textFontSize.Text = sheetDef.FontSize.ToString();
                    textWidth.Text = sheetDef.Width.ToString();
                    textHeight.Text = sheetDef.Height.ToString();
                    checkIsLandscape.Checked = sheetDef.IsLandscape;
                    break;
                default://All other sheet types use default values
                    ReloadDefaultValues();
                    break;
            }
            if (!checkHasMobileLayout.Enabled)
            { //Only change the check state if the selected form does not allow mobile sheet layout
                checkHasMobileLayout.Checked = false;
            }
        }

        private void CheckHasMobileLayout_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkHasMobileLayout.Checked && SheetDefCur.SheetDefNum > 0 && EClipboardSheetDefs.IsSheetDefInUse(SheetDefCur.SheetDefNum))
            {
                MsgBox.Show("This sheet is currently being used by eClipboard, which requires sheets to have a mobile layout. " +
                    "You must remove this form from eClipboard rules before you can remove the mobile layout for this sheet.");
                checkHasMobileLayout.Checked = true;
            }
        }

        private void ReloadDefaultValues()
        {
            if (textDescription.Text == "")
            {
                textDescription.Text = listSheetType.GetSelected<SheetTypeEnum>().GetDescription();
            }
            textFontSize.Text = SheetDefCur.FontSize.ToString();
            textWidth.Text = SheetDefCur.Width.ToString();
            textHeight.Text = SheetDefCur.Height.ToString();
            checkIsLandscape.Checked = SheetDefCur.IsLandscape;
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            if (!textWidth.IsValid() || !textHeight.IsValid())
            {
                MsgBox.Show(this, "Please fix data entry errors first.");
                return;
            }
            if (listSheetType.SelectedIndex == -1 && !SheetDefs.IsDashboardType(SheetDefCur))
            {
                MsgBox.Show(this, "Please select a sheet type first.");
                return;
            }
            if (textDescription.Text == "")
            {
                MsgBox.Show(this, "Description may not be blank.");
                return;
            }
            SheetTypeEnum sheetTypeEnumSelected = listSheetType.GetSelected<SheetTypeEnum>();
            if (sheetTypeEnumSelected == SheetTypeEnum.ExamSheet)
            {
                //make sure description for exam sheet does not contain a ':' or a ';' because this interferes with pulling the exam sheet fields to fill a patient letter
                if (textDescription.Text.Contains(":") || textDescription.Text.Contains(";"))
                {
                    MsgBox.Show(this, "Description for an Exam Sheet may not contain a ':' or a ';'.");
                    return;
                }
            }
            if (comboFontName.GetSelected<string>() == "" || comboFontName.GetSelected<string>() == null)
            {
                //not going to bother testing for validity unless it will cause a crash.
                MsgBox.Show(this, "Please select a font name first.");
                return;
            }
            float fontSize;
            try
            {
                fontSize = float.Parse(textFontSize.Text);
                if (fontSize < 2)
                {
                    MsgBox.Show(this, "Font size is invalid.");
                    return;
                }
            }
            catch
            {
                MsgBox.Show(this, "Font size is invalid.");
                return;
            }
            SheetDefCur.Description = textDescription.Text;
            if (!SheetDefs.IsDashboardType(SheetDefCur))
            {
                SheetDefCur.SheetType = sheetTypeEnumSelected;
            }
            if (checkBypassLockDate.Checked)
            {
                SheetDefCur.BypassGlobalLock = BypassLockStatus.BypassAlways;
            }
            else
            {
                SheetDefCur.BypassGlobalLock = BypassLockStatus.NeverBypass;
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
            SheetDefCur.FontName = comboFontName.GetSelected<string>();
            SheetDefCur.FontSize = fontSize;
            SheetDefCur.Width = PIn.Int(textWidth.Text);
            SheetDefCur.Height = PIn.Int(textHeight.Text);
            SheetDefCur.IsLandscape = checkIsLandscape.Checked;
            SheetDefCur.HasMobileLayout = checkHasMobileLayout.Checked;
            SheetDefCur.AutoCheckSaveImage = SetAutoCheckEnabled(SheetDefCur.SheetType) && checkAutoSaveCheck.Checked;
            //don't save to database here.
            DialogResult = DialogResult.OK;
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }






    }
}