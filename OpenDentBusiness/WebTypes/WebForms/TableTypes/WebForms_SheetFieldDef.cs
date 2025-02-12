﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OpenDentBusiness.WebTypes.WebForms
{
    [Serializable]
    [CrudTable(IsMissingInGeneral = true, HasBatchWriteMethods = true, CrudLocationOverride = @"..\..\..\OpenDentBusiness\WebTypes\WebForms\Crud",
        NamespaceOverride = "OpenDentBusiness.WebTypes.WebForms.Crud", CrudExcludePrefC = true)]
    public class WebForms_SheetFieldDef : TableBase
    {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey = true)]
        public long WebSheetFieldDefID;
        ///<summary>FK to webforms_sheetdef.WebSheetDefID</summary>
        public long WebSheetDefID;
        ///<summary>Enum:SheetFieldType </summary>
        public SheetFieldType FieldType;
        ///<summary>FieldName is used differently for different FieldTypes. 
        /// <para>For OutputText, each sheet typically has a main datatable type. For example statements correspond to the statment table. See SheetFieldsAvailable.GetList() for available values.</para>
        /// <para>     If the output field exactly matches a column from the main table this will be the &lt;ColumnName>. For example, "FName" on patient Forms.</para>
        /// <para>     If the output field exactly matches a column from a different table this will be the &lt;tablename>.&lt;ColumnName>. For example, appt.Note on Routing Slips.</para>
        /// <para>     If the output field is not a database column it must start with a lowercase letter. For example, "statementReceiptInvoice" on Statements.</para>
        /// <para>For InputField, these are hardcoded to correspond to DB fields, for example "FName" corresponsds to patient.FName. See SheetFieldsAvailable.GetList() for available values.</para>
        /// <para>For Image, this file name with extention, for example "image1.jpg". Some image names are handled specially, for example "Patient Info.gif". Images are stored in &lt;imagefolder>\SheetImages\image1.jpg.</para>
        /// <para>For CheckBox, this groups checkboxes together so that only one per group can be checked.</para>
        /// <para>For PatImage, this is the name of the DocCategory.</para>
        /// <para>For Special, identifies the type of special field. Currently only ToothChart and ToothChartLegend.</para>
        /// <para>For Grid, this is the specific type of grid. See SheetUtil.GetDataTableForGridType() for values. For example "StatementPayPlan".</para>
        /// <para>For all other fieldtypes, FieldName is blank or irrelevant.</para></summary>
        public string FieldName;
        ///<summary>For StaticText, this text can include bracketed fields, like [nameLF].
        ///<para>For OutputText and InputField, this will be blank.  </para>
        ///<para>For CheckBoxes, either X or blank.  Even if the checkbox is set to behave like a radio button.  </para>
        ///<para>For Pat Images, this is blank.  The filename of a PatImage will later be stored in SheetField.FieldValue.</para>
        ///<para>For ComboBoxes, the chosen option, semicolon, then a pipe delimited list of options such as: March;January|February|March|April</para>
        ///<para>For ScreenCharts, a semicolon delimited list of comma separated surfaces.  It may look like S,P,N;S,S,S;... etc.</para></summary>
        [CrudColumn(SpecialType = CrudSpecialColType.IsText)]
        public string FieldValue;
        ///<summary>The fontSize for this field regardless of the default for the sheet.  The actual font must be saved with each sheetField.</summary>
        public float FontSize;
        ///<summary>The fontName for this field regardless of the default for the sheet.  The actual font must be saved with each sheetField.</summary>
        public string FontName;
        ///<summary>.</summary>
        public bool FontIsBold;
        ///<summary>In pixels.</summary>
        public int XPos;
        ///<summary>In pixels.</summary>
        public int YPos;
        ///<summary>The field will be constrained horizontally to this size.  Not allowed to be zero.
        ///When SheetType is associated to a dynamic layout def and GrowthBehavior is set to a dynamic value this value represents the corresponding controls minimum width.</summary>
        public int Width;
        ///<summary>The field will be constrained vertically to this size.  Not allowed to be 0.  It's not allowed to be zero so that it will be visible on the designer.
        ///When SheetType is associated to a dynamic layout def and GrowthBehavior is set to a dynamic value this value represents the corresponding controls minimum height.</summary>
        public int Height;
        ///<summary>Enum:GrowthBehaviorEnum </summary>
        public GrowthBehaviorEnum GrowthBehavior;
        ///<summary>This is only used for checkboxes that you want to behave like radiobuttons.  Set the FieldName the same for each Checkbox in the group.  The FieldValue will likely be X for one of them and empty string for the others.  Each of them will have a different RadioButtonValue.  Whichever box has X, the RadioButtonValue for that box will be used when importing.  This field is not used for "misc" radiobutton groups.</summary>
        public string RadioButtonValue;
        ///<summary>Name which identifies the group within which the radio button belongs. FieldName must be set to "misc" in order for the group to take effect.</summary>
        public string RadioButtonGroup;
        ///<summary>Set to true if this field is required to have a value before the sheet is closed.</summary>
        public bool IsRequired;
        ///<summary>The Bitmap should be converted to Base64 using POut.Bitmap() before placing in this field.  Not stored in the database.  Only used when uploading SheetDefs to the web server.</summary>
        [CrudColumn(SpecialType = CrudSpecialColType.IsText)]
        public string ImageData;
        ///<summary>Tab stop order for all fields. One-based.  Only checkboxes and input fields can have values other than 0.</summary>
        public int TabOrder;
        ///<summary>Allows reporting on misc fields.</summary>
        public string ReportableName;
        ///<summary>Text Alignment for text fields.</summary>
        public HorizontalAlignment TextAlign;
        ///<summary>Text color, line color, rectangle color.</summary>
        [XmlIgnore]
        public Color ItemColor;
        ///<summary>Tab stop order for all fields of a mobile sheet. One-based.  Only mobile fields can have values other than 0.
        ///If all SheetFieldDefs for a given SheetField are 0 then assume that this sheet has no mobile-specific view.</summary>
        public int TabOrderMobile;
        ///<summary>Each input field for a mobile will need a corresponding UI label. This is what the user sees as the label describing what this input is for. EG "First Name:, Last Name:, Address, etc."</summary>
        public string UiLabelMobile;
        ///<summary>Human readable label that will be displayed for radio button item in mobile mode. 
        ///Cannot use UiLabelMobile for this purpose as it is already dedicated to the radio group header that groups radio button items together.</summary>
        public string UiLabelMobileRadioButton;
        ///<summary>Each sheetdef can have multiple corresponding SheetFieldDefs, each representing different Languages. Will be null for default language.
        ///Language values are stored as three letter strings.</summary>
        public string Language;
        ///<Summary>Display name of culture. Added here to pass to GWT when Sheetdef retrieved by patient.</Summary>
        [CrudColumn(IsNotDbColumn = true)]
        public string LanguageName;
        ///<summary>FK to sheetfielddef.SheetFieldDefNum. The sheet field def from the dental office that was used to create this field.</summary>
        public long SheetFieldDefNum;
        ///<summary>When true, allows a user to sign a signature box electronically.</summary>
        public bool CanElectronicallySign;
        ///<summary>When true, only allows the signature box to be signed by a provider.</summary>
        public bool IsSigProvRestricted;

        ///<summary>Used only for serialization purposes</summary>
        [XmlElement("ItemColor", typeof(int))]
        public int ColorOverrideXml
        {
            get
            {
                return ItemColor.ToArgb();
            }
            set
            {
                ItemColor = Color.FromArgb(value);
            }
        }

        public WebForms_SheetFieldDef()
        {

        }

        public WebForms_SheetFieldDef Copy()
        {
            return (WebForms_SheetFieldDef)this.MemberwiseClone();
        }

    }
}