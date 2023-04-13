﻿using OpenDentBusiness;
using System.Drawing;

namespace UnitTestsCore
{
    public class ToothInitialT
    {

        ///<summary>Creates an entry for the tooth intial table. Set isValid to false to generate one with an invalid DrawingSegment for testing DBM</summary>
        public static ToothInitial CreateToothInitial(Patient pat, string drawingSegment, string toothNum = "32", ToothInitialType toothInitialType = ToothInitialType.Drawing,
            Color colorDraw = default, float movement = 0)
        {
            ToothInitial newToothInit = new ToothInitial()
            {
                PatNum = pat.PatNum,
                ToothNum = toothNum,
                InitialType = toothInitialType,
                DrawingSegment = drawingSegment,
                ColorDraw = colorDraw,
                Movement = movement
            };
            ToothInitials.Insert(newToothInit);
            return newToothInit;
        }

        public static void ClearTable()
        {
            string command = "DELETE FROM toothinitial";
            DataCore.NonQ(command);
        }
    }
}
