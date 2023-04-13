﻿using OpenDentBusiness;
using System;

namespace UnitTestsCore
{
    public class DiscountPlanSubT
    {

        ///<summary>Deletes everything from the discountplansub table.  Does not truncate the table so that PKs are not reused on accident.</summary>
        public static void ClearDiscountPlanSubTable()
        {
            string command = "DELETE FROM discountplansub WHERE DiscountSubNum > 0";
            DataCore.NonQ(command);
        }

        ///<summary></summary>
        public static DiscountPlanSub CreateDiscountPlanSub(long patNum, long discountPlanNum, DateTime dateEffective = default(DateTime), DateTime dateTerm = default(DateTime), string subNote = default(string))
        {
            DiscountPlanSub discountPlanSub = new DiscountPlanSub()
            {
                PatNum = patNum,
                DiscountPlanNum = discountPlanNum,
                DateEffective = dateEffective,
                DateTerm = dateTerm,
                SubNote = subNote
            };
            DiscountPlanSubs.Insert(discountPlanSub);
            return discountPlanSub;
        }
    }
}