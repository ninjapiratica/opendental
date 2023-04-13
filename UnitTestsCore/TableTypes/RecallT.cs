﻿using OpenDentBusiness;
using System;

namespace UnitTestsCore
{
    public class RecallT
    {

        ///<summary></summary>
        public static Recall CreateRecall(long patNum, long recallTypeNum, DateTime dateDue, Interval recallInterval, long recallStatus = 0
            , DateTime dateScheduled = new DateTime(), DateTime dateDueCalc = new DateTime(), DateTime datePrevious = new DateTime(), string note = "")
        {
            Recall recall = new Recall();
            recall.DateDue = dateDue;
            recall.DateDueCalc = dateDueCalc;
            recall.DatePrevious = datePrevious;
            recall.DateScheduled = dateScheduled;
            recall.PatNum = patNum;
            recall.RecallInterval = recallInterval;
            recall.RecallStatus = recallStatus;
            recall.Note = note;
            recall.RecallTypeNum = recallTypeNum;
            Recalls.Insert(recall);
            return recall;
        }

        ///<summary>Deletes everything from the recalltype table.  Does not truncate the table so that PKs are not reused on accident.</summary>
        public static void ClearRecallTable()
        {
            string command = "DELETE FROM recall WHERE RecallNum > 0";
            DataCore.NonQ(command);
        }
    }
}
