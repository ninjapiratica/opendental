﻿using OpenDental;
using System;
using System.Windows.Forms;

namespace UnitTests
{
    public partial class FormMonthCalendarTests : FormODBase
    {
        public FormMonthCalendarTests()
        {
            InitializeComponent();
            InitializeLayoutManager();
            //LayoutManager.ZoomTest=40;
        }

        private void FormMonthCalendarTests_Load(object sender, EventArgs e)
        {
            DateTime date = DateTime.Today.AddDays(1);
            monthCalendar.SetDate(date);
            monthCalendarOD1.SetDateSelected(date);
            monthCalendarOD1.AllowClickingTopText = false;
            monthCalendarOD2.SetDateSelected(date);
            //float scale=2;
            //monthCalendarOD2.LayoutManager.ZoomTest=100;//additional
            //monthCalendarOD2.Font=new Font(FontFamily.GenericSansSerif,8.5f*scale);
            //monthCalendarOD2.Width=(int)(monthCalendarOD2.GetDefaultSize().Width*scale);
            //monthCalendarOD2.Height=(int)(monthCalendarOD2.GetDefaultSize().Height*scale);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MsgBox.Show(monthCalendar.SelectionStart.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MsgBox.Show(monthCalendarOD.GetDateSelected().ToString());
        }

        private void monthCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            MsgBox.Show(e.Start.ToShortDateString());
        }
    }
}
