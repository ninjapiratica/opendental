using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenDental
{
    ///<summary>Can also be used to pick Date.</summary>
    public partial class FormTimePick : FormODBase
    {
        ///<summary>The selected time. Used to set the initial value of the date and time displayed in this form. Defaults to today.
        ///Also stores the resulting DateTime that the user specifies upon clicking OK to this form.
        ///The date portion of this will be MinValue if the date picker is not enabled.</summary>
        public DateTime SelectedDTime;
        private bool _pickDate;

        ///<summary>If the user clicks OK on this form, the selected time can be accessed through the SelectedTime public variable.
        ///Pass in true to enable the Date picker as well.</summary>
        public FormTimePick(bool enableDatePicker)
        {
            InitializeComponent();
            InitializeLayoutManager();
            Lan.F(this);
            _pickDate = enableDatePicker;
            SelectedDTime = DateTime.Now;
        }

        private void FormTimePick_Load(object sender, EventArgs e)
        {
            List<string> listMinuteOptions = new List<string>() {
                "00","01","02","03","04","05","06","07","08","09",
                "10","11","12","13","14","15","16","17","18","19",
                "20","21","22","23","24","25","26","27","28","29",
                "30","31","32","33","34","35","36","37","38","39",
                "40","41","42","43","44","45","46","47","48","49",
                "50","51","52","53","54","55","56","57","58","59"
            };
            comboMinute.Items.AddList(listMinuteOptions);
            List<string> listHourOptions = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            comboHour.Items.AddList(listHourOptions);
            if (!_pickDate)
            {
                groupDate.Visible = false;
            }
            if (SelectedDTime.Hour >= 12)
            {
                radioPM.Checked = true;
            }
            else
            {
                radioAM.Checked = true;
            }
            if (SelectedDTime.Hour > 12)
            {
                int index = SelectedDTime.Hour - 12;
                comboHour.SetSelected(index - 1);//Hour is 1-12 so index 0 is hour 1.
            }
            else
            {
                if (SelectedDTime.Hour == 0)
                {
                    comboHour.SetSelected(11);//12
                }
                else
                {
                    comboHour.SetSelected(SelectedDTime.Hour - 1);
                }
            }
            comboMinute.SetSelected(SelectedDTime.Minute);//Minute is 0-59 so index 0 is minute 0.
            dateTimePicker.Value = SelectedDTime.Date;
        }


        private void butOK_Click(object sender, EventArgs e)
        {
            try
            {
                int hour = comboHour.SelectedIndex + 1;
                int minute = comboMinute.SelectedIndex;
                if (radioPM.Checked)
                {
                    if (hour < 12)
                    {
                        hour += 12;
                    }
                }
                else if (radioAM.Checked && hour == 12)
                {
                    hour = 0;
                }
                if (_pickDate)
                {
                    SelectedDTime = new DateTime(dateTimePicker.Value.Year, dateTimePicker.Value.Month, dateTimePicker.Value.Day, hour, minute, 0);
                }
                else
                {
                    SelectedDTime = new DateTime(1, 1, 1, hour, minute, 0);
                }
                DialogResult = DialogResult.OK;
            }
            catch
            {
                MsgBox.Show(this, "Please enter or select a valid time.");
            }
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

    }
}