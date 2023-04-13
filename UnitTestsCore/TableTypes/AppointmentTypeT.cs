﻿using OpenDentBusiness;
using System.Drawing;

namespace UnitTestsCore
{
    public class AppointmentTypeT
    {

        public static AppointmentType CreateAppointmentType(string appointmentTypeName, Color appointmentTypeColor = new Color(), string codeStr = ""
            , bool isHidden = false, int itemOrder = 0, string pattern = "")
        {
            AppointmentType appointmentType = new AppointmentType()
            {
                AppointmentTypeName = appointmentTypeName,
                AppointmentTypeColor = appointmentTypeColor,
                CodeStr = codeStr,
                IsHidden = isHidden,
                ItemOrder = itemOrder,
                Pattern = pattern,
            };
            AppointmentTypes.Insert(appointmentType);
            AppointmentTypes.RefreshCache();
            return appointmentType;
        }

        public static void ClearAppointmentTypeTableAdditions()
        {
            string command = "DELETE FROM appointmenttype WHERE AppointmentTypeNum > 2";
            DataCore.NonQ(command);
        }

    }
}
