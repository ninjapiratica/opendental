﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OpenDental
{
    public partial class SheetComboBox : Control
    {
        private bool _isHovering;
        public string SelectedOption;
        private string[] _arrayComboOptions;
        ///<summary>A default option for the combo box to display.  Required to be set for screening tooth chart combo boxes.  E.g. "ling", "b".</summary>
        public string DefaultOption;
        public bool IsToothChart;
        private ContextMenu _contextMenu = new ContextMenu();

        [Category("Layout"), Description("Set true if this is a toothchart combo.")]
        public bool ToothChart { get { return IsToothChart; } set { IsToothChart = value; } }

        public string[] ComboOptions
        {
            get
            {
                return _arrayComboOptions;
            }
        }

        ///<summary>Currently fills the combo box with the default options for combo boxes on screen charts.</summary>
        public SheetComboBox() : this(";None|S|PS|C|F|NFE|NN")
        {
        }

        public SheetComboBox(string values)
        {
            InitializeComponent();
            string[] arrayValues = values.Split(';');
            if (arrayValues.Length > 1)
            {
                SelectedOption = arrayValues[0];
                _arrayComboOptions = arrayValues[1].Split('|');
            }
            else
            {//Incorrect format.
             //Default to empty string when 'values' is in format 'A|B|C', indicating only combobox options, 
             //rather than 'C;A|B|C' which indicates selection as well as options.
             //Upon Ok click this will correct the fieldvalue format.
                SelectedOption = "";
                _arrayComboOptions = arrayValues[0].Split('|');//Will be an empty string if no '|' is present.
            }
            foreach (string option in _arrayComboOptions)
            {
                //'&' is a special character in System.Windows.Forms.ContextMenu. We need to escapte all ampersands so that they are displayed correctly in the fill sheet window.
                string escapedOption = option.Replace("&", "&&");
                //'-' by itself is a special character in System.Windows.Forms.ContextMenu. We need to escapte it so that it is displayed correctly in the fill sheet window.
                if (escapedOption == "-")
                {
                    escapedOption = "&-";
                }
                _contextMenu.MenuItems.Add(new MenuItem(escapedOption, menuItemContext_Click));
            }
        }

        ///<summary>Formats the selected option and the list of options in a string that can be saved to the database.</summary>
        public string ToFieldValue()
        {
            //FieldValue will contain the selected option, followed by a semicolon, followed by a | delimited list of all options.
            return SelectedOption + ";" + string.Join("|", ComboOptions);
        }

        private void menuItemContext_Click(object sender, EventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
            {
                return;
            }
            SelectedOption = _arrayComboOptions[_contextMenu.MenuItems.IndexOf((MenuItem)sender)];
        }

        private void SheetComboBox_MouseDown(object sender, MouseEventArgs e)
        {
            _contextMenu.Show(this, new Point(0, Height));//Can't resize width, it's done according to width of items.
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Color colorSurround = Color.FromArgb(245, 234, 200);
            using Brush brushHover = new SolidBrush(colorSurround);
            using Pen penOutline = new Pen(Color.Black);
            using Pen penSurround = new Pen(colorSurround);
            float sizeFont;
            if (IsToothChart)
            {
                sizeFont = 10f;
            }
            else if (Height < 11)
            {
                sizeFont = 10f;
            }
            else
            {
                sizeFont = this.Height - 10;
            }
            using Font fontStr = new Font(FontFamily.GenericSansSerif, sizeFont);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            Graphics g = pe.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.FillRectangle(Brushes.White, 0, 0, Width, Height);//White background
            if (_isHovering)
            {
                g.FillRectangle(brushHover, 0, 0, Width - 1, Height - 1);
                g.DrawRectangle(penSurround, 0, 0, Width - 1, Height - 1);
            }
            g.DrawRectangle(penOutline, -1, -1, Width, Height);//Outline
            Color colorText = Color.Black;
            if (ToothChart)
            {
                if (SelectedOption == "buc" || SelectedOption == "ling" || SelectedOption == "d" || SelectedOption == "m" || SelectedOption == "None")
                {
                    colorText = Color.LightGray;
                }
                if (SelectedOption == "None")
                {
                    SelectedOption = DefaultOption;//Nothing has been selected so draw the "default" string in the combo box.  E.g. "b", "ling", etc.
                }
            }
            using Brush brush = new SolidBrush(colorText);
            g.DrawString(SelectedOption, fontStr, brush, new Point(this.Width / 2, this.Height / 2), stringFormat);
            stringFormat.Dispose();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_isHovering)
            {
                _isHovering = true;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovering = false;
            Invalidate();
        }

        private void SheetComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                _contextMenu.Show(this, new Point(0, Height));
            }
            Invalidate();
        }

        private void SheetComboBox_Enter(object sender, EventArgs e)
        {
            _isHovering = true;
            Invalidate();
        }

        private void SheetComboBox_Leave(object sender, EventArgs e)
        {
            _isHovering = false;
            Invalidate();
        }
    }
}
