﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnitTestsCore;

namespace UnitTests.InsPlans_Tests
{
    [TestClass]
    public class InsPlansTests : TestBase
    {

        private static List<ProcedureCode> _listProcCodes;
        private static List<ProcedureCode> _listProcCodesOrig;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            _listProcCodes = ProcedureCodes.GetAllCodes();
            _listProcCodesOrig = _listProcCodes.Select(x => x.Copy()).ToList();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            //Setting substitution codes can mess up fees for other tests.
            _listProcCodesOrig.ForEach(x => ProcedureCodes.Update(x));
        }

        ///<summary></summary>
        [TestMethod]
        public void InsPlans_ComputeEstimatesForSubscriber_CanadianLabFees()
        {
            string suffix = MethodBase.GetCurrentMethod().Name;
            CultureInfo curCulture = CultureInfo.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-CA");//Canada
            try
            {
                //Create a patient and treatment plan a procedure with a lab fee.
                Patient pat = PatientT.CreatePatient();
                ProcedureCodeT.AddIfNotPresent("14611");
                ProcedureCodeT.AddIfNotPresent("99111", isCanadianLab: true);
                Procedure proc = ProcedureT.CreateProcedure(pat, "14611", ProcStat.TP, "", 250);
                Procedure procLab = ProcedureT.CreateProcedure(pat, "99111", ProcStat.TP, "", 149, procNumLab: proc.ProcNum);
                //Create a new primary insurance plan for this patient.
                //It is important that we add the insurance plan after the procedure has already been created for this particular scenario.
                Carrier carrier = CarrierT.CreateCarrier(suffix);
                InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum);
                InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);
                PatPlan patPlan = PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
                //Invoking ComputeEstimatesForAll() will simulate the logic of adding a new insurance plan from the Family module.
                //The bug that this unit test is preventing is that a duplicate claimproc was being created for the lab fee.
                //This was causing a faux line to show up when a claim was created for the procedure in question.
                //It ironically doesn't matter if the procedures above are even covered by insurance because they'll get claimprocs created regardless.
                InsPlans.ComputeEstimatesForSubscriber(sub.Subscriber);
                //Check to see how many claimproc enteries there are for the current patient.  There should only be two.
                List<ClaimProc> listClaimProcs = ClaimProcs.Refresh(pat.PatNum);
                Assert.AreEqual(2, listClaimProcs.Count);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = curCulture;
            }
        }

        /// <summary>Get the copay value for when there is no patient copay</summary>
        [TestMethod]
        public void InsPlans_GetCopay_Blank()
        {
            ProcedureCode procCode = _listProcCodes[0];
            InsPlan plan = GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name);
            double amt = InsPlans.GetCopay(procCode.CodeNum, plan.FeeSched, plan.CopayFeeSched, plan.CodeSubstNone, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(-1, amt);
        }

        ///<summary>Get the copay amount when there is no exact fee on the copay schedule but there is a fee in the default schedule.</summary>
        [TestMethod]
        public void InsPlans_GetCopay_NoExactFeeUseDefault()
        {
            ProcedureCode procCode = _listProcCodes[1];
            InsPlan plan = GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name);
            Fee feeDefault = FeeT.GetNewFee(plan.FeeSched, procCode.CodeNum, 25);
            Prefs.UpdateBool(PrefName.CoPay_FeeSchedule_BlankLikeZero, false);
            double amt = InsPlans.GetCopay(procCode.CodeNum, plan.FeeSched, plan.CopayFeeSched, plan.CodeSubstNone, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(feeDefault.Amount, amt);
        }

        ///<summary>Get the copay amount where there is no exact fee and the Preference CoPay_FeeSchedule_BlankLikeZero is true.</summary>
        [TestMethod]
        public void InsPlans_GetCopay_NoExactFeeUseZero()
        {
            ProcedureCode procCode = _listProcCodes[2];
            InsPlan plan = GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name);
            Fee feeDefault = FeeT.GetNewFee(plan.FeeSched, procCode.CodeNum, 35);
            Prefs.UpdateBool(PrefName.CoPay_FeeSchedule_BlankLikeZero, true);
            double amt = InsPlans.GetCopay(procCode.CodeNum, plan.FeeSched, plan.CopayFeeSched, plan.CodeSubstNone, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(-1, amt);
            Prefs.UpdateBool(PrefName.CoPay_FeeSchedule_BlankLikeZero, false);
        }

        ///<summary>Get the copay value for when there is no substitute fee and the exact copay fee exists.</summary>
        [TestMethod]
        public void InsPlans_GetCopay_ExactFee()
        {
            ProcedureCode procCode = _listProcCodes[3];
            InsPlan plan = GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name);
            Fee feeDefault = FeeT.GetNewFee(plan.FeeSched, procCode.CodeNum, 50);
            Fee feeCopay = FeeT.GetNewFee(plan.CopayFeeSched, procCode.CodeNum, 15);
            double amt = InsPlans.GetCopay(procCode.CodeNum, plan.FeeSched, plan.CopayFeeSched, plan.CodeSubstNone, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(feeCopay.Amount, amt);
        }

        ///<summary>Get the copay value for when there is a substitute fee.</summary>
        [TestMethod]
        public void InsPlans_GetCopay_SubstituteFee()
        {
            ProcedureCode procCode = _listProcCodes[4];
            procCode.SubstitutionCode = _listProcCodes[5].ProcCode;
            ProcedureCodes.Update(procCode);
            InsPlan plan = GenerateMediFlatInsPlan(MethodBase.GetCurrentMethod().Name, false);
            Fee feeDefault = FeeT.GetNewFee(plan.FeeSched, procCode.CodeNum, 100);
            Fee feeSubstitute = FeeT.GetNewFee(plan.CopayFeeSched, ProcedureCodes.GetSubstituteCodeNum(procCode.ProcCode, "", plan.PlanNum), 45);
            double amt = InsPlans.GetCopay(procCode.CodeNum, plan.FeeSched, plan.CopayFeeSched, plan.CodeSubstNone, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(feeSubstitute.Amount, amt);
        }

        ///<summary>Get the allowed amount for the procedure code for the PPO plan.</summary>
        [TestMethod]
        public void InsPlans_GetAllowed_PPOExact()
        {
            InsPlan plan = GeneratePPOPlan(MethodBase.GetCurrentMethod().Name);
            ProcedureCode procCode = _listProcCodes[6];
            Fee fee = FeeT.GetNewFee(plan.FeeSched, procCode.CodeNum, 65);
            double allowed = InsPlans.GetAllowed(procCode.ProcCode, plan.FeeSched, plan.AllowedFeeSched, plan.CodeSubstNone, plan.PlanType, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(fee.Amount, allowed);
        }

        ///<summary>Get the allowed amount when there is a substitution code for the PPO plan.</summary>
        [TestMethod]
        public void InsPlans_GetAllowed_PPOSubstitute()
        {
            InsPlan plan = GeneratePPOPlan(MethodBase.GetCurrentMethod().Name, false);
            ProcedureCode procCode = _listProcCodes[7];
            procCode.SubstitutionCode = _listProcCodes[8].ProcCode;
            ProcedureCodes.Update(procCode);
            ProcedureCodes.RefreshCache();
            Fee feeOrig = FeeT.GetNewFee(plan.FeeSched, procCode.CodeNum, 85);
            Fee feeSubs = FeeT.GetNewFee(plan.FeeSched, ProcedureCodes.GetSubstituteCodeNum(procCode.ProcCode, "", plan.PlanNum), 20);
            double allowed = InsPlans.GetAllowed(procCode.ProcCode, plan.FeeSched, plan.AllowedFeeSched, plan.CodeSubstNone, plan.PlanType, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(feeSubs.Amount, allowed);
        }

        ///<summary>Get the allowed amount where there is a substitution code that is more expensive than the original code for the PPO plan.</summary>
        [TestMethod]
        public void InsPlans_GetAllowed_PPOSubstituteMoreExpensive()
        {
            InsPlan plan = GeneratePPOPlan(MethodBase.GetCurrentMethod().Name, false);
            ProcedureCode procCode = _listProcCodes[9];
            procCode.SubstitutionCode = _listProcCodes[10].ProcCode;
            ProcedureCodes.Update(procCode);
            Fee feeOrig = FeeT.GetNewFee(plan.FeeSched, procCode.CodeNum, 85);
            Fee feeSubs = FeeT.GetNewFee(plan.FeeSched, ProcedureCodes.GetSubstituteCodeNum(procCode.SubstitutionCode, "", plan.PlanNum), 200);
            double allowed = InsPlans.GetAllowed(procCode.ProcCode, plan.FeeSched, plan.AllowedFeeSched, plan.CodeSubstNone, plan.PlanType, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(feeOrig.Amount, allowed);
        }

        ///<summary>Get the allowed amount for a capitation plan that has an allowed fee schedule.</summary>
        [TestMethod]
        public void InsPlans_GetAllowed_CapAllowedFeeSched()
        {
            InsPlan plan = GenerateCapPlan(MethodBase.GetCurrentMethod().Name);
            ProcedureCode procCode = _listProcCodes[11];
            Fee feeAllowed = FeeT.GetNewFee(plan.AllowedFeeSched, procCode.CodeNum, 70);
            double amt = InsPlans.GetAllowed(procCode.ProcCode, plan.FeeSched, plan.AllowedFeeSched, plan.CodeSubstNone, plan.PlanType, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(feeAllowed.Amount, amt);
        }

        ///<summary>Get the allowed amount for a capitation plan where there is no allowed fee schedule and there is no substitution code.</summary>
        [TestMethod]
        public void InsPlan_GetAllowed_CapNoAllowedNoSubs()
        {
            InsPlan plan = GenerateCapPlan(MethodBase.GetCurrentMethod().Name, false);
            ProcedureCode procCode = _listProcCodes[12];
            double amt = InsPlans.GetAllowed(procCode.ProcCode, plan.FeeSched, plan.AllowedFeeSched, plan.CodeSubstNone, plan.PlanType, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(-1, amt);
        }

        ///<summary>Get the allowed amount for a capitation plan where there is no fee schedule assigned to the plan</summary>
        [TestMethod]
        public void InsPlans_GetAllowed_NoFeeSched()
        {
            Carrier carrier = CarrierT.CreateCarrier(MethodBase.GetCurrentMethod().Name);
            InsPlan plan = new InsPlan();
            plan.CarrierNum = carrier.CarrierNum;
            plan.PlanType = "";
            plan.CobRule = EnumCobRule.Basic;
            plan.PlanNum = InsPlans.Insert(plan);
            ProcedureCode procCode = _listProcCodes[13];
            procCode.SubstitutionCode = _listProcCodes[14].ProcCode;
            ProcedureCodes.Update(procCode);
            ProcedureCodes.RefreshCache();
            Provider prov = Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
            long provFeeSched = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, MethodBase.GetCurrentMethod().Name);
            prov.FeeSched = provFeeSched;
            Providers.Update(prov);
            Providers.RefreshCache();
            Fee defaultFee = FeeT.GetNewFee(Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv)).FeeSched,
                ProcedureCodes.GetSubstituteCodeNum(procCode.ProcCode, "", plan.PlanNum), 80);
            double amt = InsPlans.GetAllowed(procCode.ProcCode, plan.FeeSched, plan.AllowedFeeSched, plan.CodeSubstNone, plan.PlanType, "", 0, 0, plan.PlanNum);
            Assert.AreEqual(defaultFee.Amount, amt);
        }

        #region Factory Methods

        private InsPlan GenerateMediFlatInsPlan(string suffix, bool codeSubstNone = true)
        {
            long baseFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "Normal_" + suffix, true);
            long copayFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.CoPay, "Copay_" + suffix, true);
            Carrier carrier = CarrierT.CreateCarrier("Carrier_" + suffix);
            return InsPlanT.CreateInsPlanMediFlatCopay(carrier.CarrierNum, baseFeeSchedNum, copayFeeSchedNum, codeSubstNone);
        }

        private InsPlan GeneratePPOPlan(string suffix, bool codeSubstNone = true)
        {
            long baseFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "Normal_" + suffix, true);
            Carrier carrier = CarrierT.CreateCarrier("Carrier_" + suffix);
            return InsPlanT.CreateInsPlanPPO(carrier.CarrierNum, baseFeeSchedNum, codeSubstNone);
        }

        private InsPlan GenerateCapPlan(string suffix, bool createAllowed = true, bool codeSubstNone = true)
        {
            long baseFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "Normal_" + suffix, true);
            long allowedFeeSchedNum = 0;
            if (createAllowed)
            {
                allowedFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "Allowed_" + suffix, true);
            }
            Carrier carrier = CarrierT.CreateCarrier("Carrier_" + suffix);
            return InsPlanT.CreateInsPlanCapitation(carrier.CarrierNum, baseFeeSchedNum, allowedFeeSchedNum, codeSubstNone);
        }

        #endregion

        ///<summary>Creates a procedure on an ins plan that does not calculate PPO writeoffs for substituted codes.</summary>
        [TestMethod]
        public void InsPlan_PpoSubNoWriteoffs()
        {
            string suffix = MethodBase.GetCurrentMethod().Name;
            Patient pat = PatientT.CreatePatient(suffix);
            long ucrFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "UCR Fees" + suffix);
            long ppoFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO " + suffix);
            InsuranceInfo ins = InsuranceT.AddInsurance(pat, suffix, planType: "p", feeSchedNum: ppoFeeSchedNum);
            ins.PriInsPlan.HasPpoSubstWriteoffs = false;
            InsPlans.Update(ins.PriInsPlan);
            BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum, EbenefitCategory.Restorative, 50);
            ProcedureCode originalProcCode = ProcedureCodeT.CreateProcCode("D2330");
            ProcedureCode downgradeProcCode = ProcedureCodeT.CreateProcCode("D2140");
            originalProcCode.SubstitutionCode = "D2140";
            originalProcCode.SubstOnlyIf = SubstitutionCondition.Always;
            ProcedureCodeT.Update(originalProcCode);
            FeeT.CreateFee(ucrFeeSchedNum, originalProcCode.CodeNum, 100);
            FeeT.CreateFee(ucrFeeSchedNum, downgradeProcCode.CodeNum, 80);
            FeeT.CreateFee(ppoFeeSchedNum, originalProcCode.CodeNum, 60);
            FeeT.CreateFee(ppoFeeSchedNum, downgradeProcCode.CodeNum, 50);
            Procedure proc = ProcedureT.CreateProcedure(pat, "D2330", ProcStat.C, "9", 100);//Tooth 9
            List<ClaimProc> listClaimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<Procedure> listProcs = Procedures.Refresh(pat.PatNum);
            ins.RefreshBenefits();
            Claim claim = ClaimT.CreateClaim("P", ins.ListPatPlans, ins.ListInsPlans, listClaimProcs, listProcs, pat, listProcs, ins.ListBenefits, ins.ListInsSubs);
            ClaimProc clProc = ClaimProcs.Refresh(pat.PatNum)[0];//Should only be one
            Assert.AreEqual(50, clProc.Percentage);
            Assert.AreEqual(25, clProc.BaseEst);
            Assert.AreEqual(25, clProc.InsPayEst);
            Assert.AreEqual(-1, clProc.WriteOffEst);
        }

        ///<summary>Creates a procedure on an ins plan that does calculate PPO writeoffs for substituted codes.</summary>
        [TestMethod]
        public void InsPlan_PpoSubWriteoffs()
        {
            string suffix = MethodBase.GetCurrentMethod().Name;
            Patient pat = PatientT.CreatePatient(suffix);
            long ucrFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "UCR Fees" + suffix);
            long ppoFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO " + suffix);
            InsuranceInfo ins = InsuranceT.AddInsurance(pat, suffix, planType: "p", feeSchedNum: ppoFeeSchedNum);
            BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum, EbenefitCategory.Restorative, 50);
            ProcedureCode originalProcCode = ProcedureCodeT.CreateProcCode("D2330");
            ProcedureCode downgradeProcCode = ProcedureCodeT.CreateProcCode("D2140");
            originalProcCode.SubstitutionCode = "D2140";
            originalProcCode.SubstOnlyIf = SubstitutionCondition.Always;
            ProcedureCodeT.Update(originalProcCode);
            FeeT.CreateFee(ucrFeeSchedNum, originalProcCode.CodeNum, 100);
            FeeT.CreateFee(ucrFeeSchedNum, downgradeProcCode.CodeNum, 80);
            FeeT.CreateFee(ppoFeeSchedNum, originalProcCode.CodeNum, 60);
            FeeT.CreateFee(ppoFeeSchedNum, downgradeProcCode.CodeNum, 50);
            Procedure proc = ProcedureT.CreateProcedure(pat, "D2330", ProcStat.C, "8", 100);//Tooth 8
            List<ClaimProc> listClaimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<Procedure> listProcs = Procedures.Refresh(pat.PatNum);
            ins.RefreshBenefits();
            Claim claim = ClaimT.CreateClaim("P", ins.ListPatPlans, ins.ListInsPlans, listClaimProcs, listProcs, pat, listProcs, ins.ListBenefits, ins.ListInsSubs);
            ClaimProc clProc = ClaimProcs.Refresh(pat.PatNum)[0];//Should only be one
            Assert.AreEqual(50, clProc.Percentage);
            Assert.AreEqual(25, clProc.BaseEst);
            Assert.AreEqual(25, clProc.InsPayEst);
            Assert.AreEqual(40, clProc.WriteOffEst);
        }

        ///<summary>Creates a procedure on an ins plan that does not calculate PPO writeoffs for substituted codes where the procedure is not
        ///substitued.</summary>
        [TestMethod]
        public void InsPlan_PpoNoSubWriteoffsNoSub()
        {
            string suffix = MethodBase.GetCurrentMethod().Name;
            Patient pat = PatientT.CreatePatient(suffix);
            long ucrFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "UCR Fees" + suffix);
            long ppoFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO " + suffix);
            InsuranceInfo ins = InsuranceT.AddInsurance(pat, suffix, planType: "p", feeSchedNum: ppoFeeSchedNum);
            ins.PriInsPlan.HasPpoSubstWriteoffs = false;
            InsPlans.Update(ins.PriInsPlan);
            BenefitT.CreateCategoryPercent(ins.PriInsPlan.PlanNum, EbenefitCategory.Restorative, 50);
            ProcedureCode originalProcCode = ProcedureCodeT.CreateProcCode("D2330");
            ProcedureCode downgradeProcCode = ProcedureCodeT.CreateProcCode("D2140");
            originalProcCode.SubstitutionCode = "";//NOT substituting
            originalProcCode.SubstOnlyIf = SubstitutionCondition.Always;
            ProcedureCodeT.Update(originalProcCode);
            FeeT.CreateFee(ucrFeeSchedNum, originalProcCode.CodeNum, 100);
            FeeT.CreateFee(ucrFeeSchedNum, downgradeProcCode.CodeNum, 80);
            FeeT.CreateFee(ppoFeeSchedNum, originalProcCode.CodeNum, 60);
            FeeT.CreateFee(ppoFeeSchedNum, downgradeProcCode.CodeNum, 50);
            Procedure proc = ProcedureT.CreateProcedure(pat, "D2330", ProcStat.C, "9", 100);//Tooth 9
            List<ClaimProc> listClaimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<Procedure> listProcs = Procedures.Refresh(pat.PatNum);
            ins.RefreshBenefits();
            Claim claim = ClaimT.CreateClaim("P", ins.ListPatPlans, ins.ListInsPlans, listClaimProcs, listProcs, pat, listProcs, ins.ListBenefits, ins.ListInsSubs);
            ClaimProc clProc = ClaimProcs.Refresh(pat.PatNum)[0];//Should only be one
            Assert.AreEqual(50, clProc.Percentage);
            Assert.AreEqual(30, clProc.BaseEst);
            Assert.AreEqual(30, clProc.InsPayEst);
            Assert.AreEqual(40, clProc.WriteOffEst);
        }

        ///<summary></summary>
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlan_GetInsUsedDisplay_LimitationsOverride)]
        [Documentation.VersionAdded("7.1")]
        [Documentation.Description("Patient has one insurance plan, subscriber self. Benefits: annual max 1000, diagnostic max 1000. First completed procedure, an exam for $50, insurance paid $50.  Second procedure, a crown for $830, insurance paid $400. Ins used should show $400 and should not include the $50 because the ins used value should only be concerned with amounts that affect the annual max .")]
        [TestMethod]
        public void InsPlan_GetInsUsedDisplay_LimitationsOverride()
        {
            string suffix = "6";
            Patient pat = PatientT.CreatePatient(suffix);
            long patNum = pat.PatNum;
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum);
            long planNum = plan.PlanNum;
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, planNum);//guarantor is subscriber
            long subNum = sub.InsSubNum;
            long patPlanNum = PatPlanT.CreatePatPlan(1, pat.PatNum, subNum).PatPlanNum;
            BenefitT.CreateAnnualMax(planNum, 1000);
            BenefitT.CreateLimitation(planNum, EbenefitCategory.Diagnostic, 1000);
            Procedure proc = ProcedureT.CreateProcedure(pat, "D0120", ProcStat.C, "", 50);//An exam
            long procNum = proc.ProcNum;
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.C, "8", 830);//create a crown
            ClaimProcT.AddInsPaid(patNum, planNum, procNum, 50, subNum, 0, 0);
            ClaimProcT.AddInsPaid(patNum, planNum, proc2.ProcNum, 400, subNum, 0, 0);
            //Lists
            Family fam = Patients.GetFamily(patNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(patNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = ClaimProcs.GetHistList(patNum, benefitList, patPlans, planList, DateTime.Today, subList);
            //Validate
            double insUsed = InsPlans.GetInsUsedDisplay(histList, DateTime.Today, planNum, patPlanNum, -1, planList, benefitList, patNum, subNum);
            Assert.AreEqual(400, insUsed);
        }

        ///<summary></summary>
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlan_GetInsUsedDisplay_OrthoProcsNotAffectInsUsed)]
        [Documentation.VersionAdded("7.9.9")]
        [Documentation.Description("Patient has one insurance plan, subscriber self. Benefits: annual max $100, ortho max $500, 100% on diagnostic and ortho.  2 procs: D0140 (limEx) $59, and D8090 (comprehensive ortho) $348. Each sent to ins on separate claim, received, paid 100%.  Insurance used should show $59.")]
        [TestMethod]
        public void InsPlan_GetInsUsedDisplay_OrthoProcsNotAffectInsUsed()
        {
            string suffix = "13";
            Patient pat = PatientT.CreatePatient(suffix);
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);
            long subNum = sub.InsSubNum;
            PatPlan patPlan = PatPlanT.CreatePatPlan(1, pat.PatNum, subNum);
            BenefitT.CreateAnnualMax(plan.PlanNum, 100);
            BenefitT.CreateOrthoMax(plan.PlanNum, 500);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Diagnostic, 100);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Orthodontics, 100);
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D0140", ProcStat.C, "", 59);//limEx
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D8090", ProcStat.C, "", 348);//Comprehensive ortho
            ClaimProcT.AddInsPaid(pat.PatNum, plan.PlanNum, proc1.ProcNum, 59, subNum, 0, 0);
            ClaimProcT.AddInsPaid(pat.PatNum, plan.PlanNum, proc2.ProcNum, 348, subNum, 0, 0);
            //Lists
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = ClaimProcs.GetHistList(pat.PatNum, benefitList, patPlans, planList, DateTime.Today, subList);
            //Validate
            double insUsed = InsPlans.GetInsUsedDisplay(histList, DateTime.Today, plan.PlanNum, patPlan.PatPlanNum, -1, planList, benefitList, pat.PatNum, subNum);
            Assert.AreEqual(59, insUsed);
        }

        ///<summary></summary>
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlan_GetDedRemainDisplay_IndividualAndFamilyDeductiblesInsRemaining)]
        [Documentation.VersionAdded("12.1")]
        [Documentation.Description("Three patients, all with the same insurance plan. Guarantor is subscriber. $75 individual deductible, $150 family deductible. Patient 3 has a $75 insurance adjustment for a previously applied deductible. Patient 2 has a procedure D2750 for $1280 that has been paid, including a deductible of $50. Patient 1, the guarantor, has a procedure treatment planned, D4355 for $135. In the guarantor's TP module, at the lower right, the deductible remaining should be $25. An internal test is also performed to verify that if the family deductible were ignored, that the deductible remaining would show $75.")]
        [TestMethod]
        public void InsPlan_GetDedRemainDisplay_IndividualAndFamilyDeductiblesInsRemaining()
        {
            string suffix = "20";
            Patient pat = PatientT.CreatePatient(suffix);//guarantor
            long patNum = pat.PatNum;
            Patient pat2 = PatientT.CreatePatient(suffix);
            PatientT.SetGuarantor(pat2, pat.PatNum);
            Patient pat3 = PatientT.CreatePatient(suffix);
            PatientT.SetGuarantor(pat3, pat.PatNum);
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum);
            long planNum = plan.PlanNum;
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, planNum);//guarantor is subscriber
            long subNum = sub.InsSubNum;
            PatPlan patPlan = PatPlanT.CreatePatPlan(1, pat.PatNum, subNum);//all three patients have the same plan
            PatPlan patPlan2 = PatPlanT.CreatePatPlan(1, pat2.PatNum, subNum);//all three patients have the same plan
            PatPlan patPlan3 = PatPlanT.CreatePatPlan(1, pat3.PatNum, subNum);//all three patients have the same plan
            BenefitT.CreateDeductibleGeneral(planNum, BenefitCoverageLevel.Individual, 75);
            BenefitT.CreateDeductibleGeneral(planNum, BenefitCoverageLevel.Family, 150);
            ClaimProcT.AddInsUsedAdjustment(pat3.PatNum, planNum, 0, subNum, 75);//Adjustment goes on the third patient
            Procedure proc = ProcedureT.CreateProcedure(pat2, "D2750", ProcStat.C, "20", 1280);//proc for second patient with a deductible already applied.
            ClaimProcT.AddInsPaid(pat2.PatNum, planNum, proc.ProcNum, 304, subNum, 50, 597);
            proc = ProcedureT.CreateProcedure(pat, "D4355", ProcStat.TP, "", 135);//proc is for the first patient
            long procNum = proc.ProcNum;
            //Lists
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(patNum);
            Family fam = Patients.GetFamily(patNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(patNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = ClaimProcs.GetHistList(patNum, benefitList, patPlans, planList, DateTime.Today, subList);
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            //Validate
            List<ClaimProcHist> HistList = ClaimProcs.GetHistList(pat.PatNum, benefitList, patPlans, planList, DateTime.Today, subList);
            double dedFam = Benefits.GetDeductGeneralDisplay(benefitList, planNum, patPlan.PatPlanNum, BenefitCoverageLevel.Family);
            double ded = Benefits.GetDeductGeneralDisplay(benefitList, planNum, patPlan.PatPlanNum, BenefitCoverageLevel.Individual);
            double dedRem = InsPlans.GetDedRemainDisplay(HistList, DateTime.Today, planNum, patPlan.PatPlanNum, -1, planList, pat.PatNum, ded, dedFam);//test family and individual deductible together
            Assert.AreEqual(25, dedRem);
            dedRem = InsPlans.GetDedRemainDisplay(HistList, DateTime.Today, planNum, patPlan.PatPlanNum, -1, planList, pat.PatNum, ded, -1);//test individual deductible by itself
            Assert.AreEqual(75, dedRem);
        }

        ///<summary></summary>
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlan_GetPendingDisplay_LimitationsOverrideGeneralLimitations)]
        [Documentation.VersionAdded("12.3.45")]
        [Documentation.Description("Patient has one insurance plan, subscriber self.  Benefits: annual max $1000. Other benefit added for limitation on routine preventive of $1000. Routine preventive 100%. A prophy D1110 for $125 is complete, attached to a claim, with insurance estimate of $125 and a claimproc status of NotReceived. Pending insurance at the lower right of the TP module  should be $0 because the procedure does not count towards the regular annual max. It instead has its own annual max.")]
        [TestMethod]
        public void InsPlan_GetPendingDisplay_LimitationsOverrideGeneralLimitations()
        {
            string suffix = "31";
            Patient pat = PatientT.CreatePatient(suffix);
            long patNum = pat.PatNum;
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum);
            long planNum = plan.PlanNum;
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, planNum);//guarantor is subscriber
            long subNum = sub.InsSubNum;
            long patPlanNum = PatPlanT.CreatePatPlan(1, pat.PatNum, subNum).PatPlanNum;
            BenefitT.CreateAnnualMax(planNum, 1000);
            BenefitT.CreateCategoryPercent(planNum, EbenefitCategory.RoutinePreventive, 100);
            BenefitT.CreateLimitation(planNum, EbenefitCategory.RoutinePreventive, 1000);//Changing this amount would affect patient portion vs ins portion.  But regardless of the amount, this should prevent any pending from showing in the box, which is for general pending only.
            Procedure proc = ProcedureT.CreateProcedure(pat, "D1110", ProcStat.C, "", 125);//Prophy
                                                                                           //Lists
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            Family fam = Patients.GetFamily(patNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(patNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            Claim claim = ClaimT.CreateClaim("P", patPlans, planList, claimProcs, ProcList, pat, ProcList, benefitList, subList);//Creates the claim in the same manner as the account module, including estimates and status NotReceived.
            List<ClaimProcHist> histList = ClaimProcs.GetHistList(patNum, benefitList, patPlans, planList, DateTime.Today, subList);
            //Validate
            Assert.AreEqual(0, InsPlans.GetPendingDisplay(histList, DateTime.Today, plan, patPlanNum, -1, patNum, subNum, benefitList));
        }

        #region InsPlansZeroWriteOffsOnAnnualMaxOverride
        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_AnnualMaxSurpassedZerosWriteoff_Global)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when annual max is entirely exceeded is set to true, plan override is set to default. Benefits include annual max of 100, crowns 90%. Three procedures are treatment planned: all crowns for $110, the second procedure causes the account to go over the annual max, and the third will then completely exceed the annual max resulting in a $0 writeoff. The third TP must recieve no estimated write off.
<table>
  <tr>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$110.00</td>
    <td>$19.00</td>
    <td>$20.00</td>
    <td>$71.00</td>
  </tr>
  <tr>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverAnnualMaxGlobalLevel()
        {
            //3 procedures, insurance coverage is $81 with a annual max of $100.
            // 1 - First proc will get full coverage for the $81, with a $20 writeoff.
            // 2 - Second proc will get partial coverage for the $81, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $0 writeoff.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix);
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateAnnualMax(plan.PlanNum, 100);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnAnnualMax, true);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc2.WriteOffEst);
            Assert.AreEqual(19, claimProc2.InsPayEst);
            Assert.AreEqual(71.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(110, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_AnnualMaxSurpassedZerosWriteoff_Plan)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when annual max is entirely exceeded is set to false, plan override is set to true. Benefits include annual max of 100, crowns 90%. Three procedures are treatment planned: all crowns for $110, the second procedure causes the account to go over the annual max, and the third will then completely exceed the annual max resulting in a $0 writeoff. The third TP must recieve no estimated write off.
<table>
  <tr>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$110.00</td>
    <td>$19.00</td>
    <td>$20.00</td>
    <td>$71.00</td>
  </tr>
  <tr>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverAnnualMaxPlanLevel()
        {
            //3 procedures, insurance coverage is $81 with a annual max of $100.
            // 1 - First proc will get full coverage for the $81, with a $20 writeoff.
            // 2 - Second proc will get partial coverage for the $81, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $0 writeoff.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix);
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            plan.InsPlansZeroWriteOffsOnAnnualMaxOverride = YN.Yes; //Set plan level override.
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateAnnualMax(plan.PlanNum, 100);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded to false.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnAnnualMax, false);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc2.WriteOffEst);
            Assert.AreEqual(19, claimProc2.InsPayEst);
            Assert.AreEqual(71.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(110, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_AnnualMaxSurpassedZerosWriteoff_Off)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when annual max is entirely exceeded is set to false, plan override is set to default. Benefits include annual max of 100, crowns 90%. Three procedures are treatment planned: all crowns for $110, the second procedure causes the account to go over the annual max, and the third will then completely exceed the annual max. The third TP must recieve the same estimated write off as the previous two TP's.
<table>
  <tr>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$110.00</td>
    <td>$19.00</td>
    <td>$20.00</td>
    <td>$71.00</td>
  </tr>
  <tr>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$20.00</td>
    <td>$90.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverAnnualMaxGlobalLevelOff()
        {
            //3 procedures, insurance coverage is $81 with a annual max of $100.
            // 1 - First proc will get full coverage for the $81, with a $20 writeoff.
            // 2 - Second proc will get partial coverage for the $81, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $20 writeoff, since our InsPlansZeroWriteOffsOnAnnualMax is set to false (globally and plan level is set to default).
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix);
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            plan.InsPlansZeroWriteOffsOnAnnualMaxOverride = YN.Unknown; //Set plan level override.
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateAnnualMax(plan.PlanNum, 100);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded to false.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnAnnualMax, false);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc2.WriteOffEst);
            Assert.AreEqual(19, claimProc2.InsPayEst);
            Assert.AreEqual(71.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(90, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_AnnualMaxSurpassedZerosWriteoff_Plan)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when annual max is entirely exceeded is set to false, plan override is set to true. Benefits include annual max of 90, crowns 100%. Three procedures are treatment planned: all crowns for $110, the second procedure causes the account to go over the annual max, and the third will then completely exceed the annual max resulting in a $0 writeoff. Also there is a deductible that will completely cover the first procedure. That procedure must not zero out the write-off.
<table>
  <tr>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
    <td>1</td>
    <td>$110.00</td>
    <td>$0</td>
    <td>$20.00</td>
    <td>$90.00</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$110.00</td>
    <td>$90</td>
    <td>$20</td>
    <td>$0</td>
  </tr>
  <tr>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverAnnualMaxPlanLevelWhenDeductibleEqualsProcFeeAllowedAndAnnualMaxMet()
        {
            //3 procedures, insurance coverage is %100 with an annual max of $90 and $100 deductible.
            // 1 - First proc will get no coverage, patportion is $90, with a $20 writeoff.
            // 2 - Second proc will get full coverage for the $90, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $0 writeoff.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix);
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            plan.InsPlansZeroWriteOffsOnAnnualMaxOverride = YN.Yes; //Set plan level override.
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateAnnualMax(plan.PlanNum, 90);
            BenefitT.CreateDeductible(plan.PlanNum, EbenefitCategory.General, 90);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 100);
            //Set pref to 0 write offs after annual max is exceeded to false.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnAnnualMax, false);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            //First claimProc: ProcFeeUCR=110 ProcFeeAllowed=90 Deductible=90 PercentageCoverage=100 DeductibleRem=90
            //Writeoff est: ProcFeeUCR - ProcFeeAllowed
            // -> 110 - 90 = 20
            //InsPay est: (ProcFeeAllowed - DeductibleRem)-(ProcFeeAllowed - DeductibleRem)*PercentageCoverage || if DeductibleRem > ProcFeeAllowed : 0
            // -> (90 - 90) - (90 - 90) * 0 = 0 || 0 
            //Pat Portion est (Annual Max Met): ProcFeeUCR
            // -> 110
            //Pat Portion est (Annual Max NOT Met): ProcFeeUCR - InsPayEst - write off || ProcFeeAllowed - InsPayEst
            // -> 110 - 0 - 20 = 90 || 90 - 0 = 90
            //DeductibleRem after this claim proc: DeductibleRem - ProcFeeAllowed
            // -> 90 - 90 = 0
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(0, claimProc1.InsPayEst);
            Assert.AreEqual(90, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            //First claimProc: ProcFeeUCR=110 ProcFeeAllowed=90 Deductible=90 PercentageCoverage=100 DeductibleRem=0
            //Writeoff est: ProcFeeUCR - ProcFeeAllowed
            // -> 110 - 90 = 20
            //InsPay est: (ProcFeeAllowed - DeductibleRem)-(ProcFeeAllowed - DeductibleRem)*PercentageCoverage || if DeductibleRem > ProcFeeAllowed : 0
            // -> (90 - 0) - (90 - 0) * 0 = 90 || 0 
            //Pat Portion est (Annual Max Met): ProcFeeUCR
            // -> 110
            //Pat Portion est (Annual Max NOT Met): ProcFeeUCR - InsPayEst - write off || ProcFeeAllowed - InsPayEst
            // -> 110 - 90 - 20 = 0 || 90 - 90 = 90
            //DeductibleRem after this claim proc: DeductibleRem - ProcFeeAllowed
            // -> 0 - 90 = 0
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc2.WriteOffEst);
            Assert.AreEqual(90, claimProc2.InsPayEst);
            Assert.AreEqual(0, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            //First claimProc: ProcFeeUCR=110 ProcFeeAllowed=90 Deductible=90 PercentageCoverage=100 DeductibleRem=0
            //Writeoff est: ProcFeeUCR - ProcFeeAllowed
            // -> 110 - 90 = 20
            //InsPay est: (ProcFeeAllowed - DeductibleRem)-(ProcFeeAllowed - DeductibleRem)*PercentageCoverage || if DeductibleRem > ProcFeeAllowed : 0
            // -> (90 - 0) - (90 - 0) * 0 = 90 || 0 
            //Pat Portion est (Annual Max Met): ProcFeeUCR
            // -> 110
            //Pat Portion est (Annual Max NOT Met): ProcFeeUCR - InsPayEst - write off || ProcFeeAllowed - InsPayEst
            // -> 110 - 90 - 20 = 90 || 90 - 0 = 90
            //DeductibleRem after this claim proc: DeductibleRem - ProcFeeAllowed
            // -> 0 - 90 = 0
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(110, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        [TestMethod]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverAnnualMaxGlobalLevelDeductableExceedsProcEntirely()
        {
            //3 procedures, insurance coverage is $81 with a annual max of $100.
            // 1 - First proc will get full coverage for the $81, with a $20 writeoff.
            // 2 - Second proc will get partial coverage for the $81, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $0 writeoff.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix);
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 1000);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 1000);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "8", 1000);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - UCR
            long ucrFeeSchedNum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "UCR Fees" + suffix);
            long feePPONum2 = FeeT.CreateFee(ucrFeeSchedNum, proc1.CodeNum, 1000);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 900);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateAnnualMax(plan.PlanNum, 99999);
            BenefitT.CreateDeductible(plan.PlanNum, "D2750", 1400.00);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnAnnualMax, true);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                        histList, loopList, false, pat.Age, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            //First claimProc: ProcFeeUCR=1000 ProcFeeAllowed=900 Deductible=500 PercentageCoverage=(100-90)/100 DeductibleRem=1400
            //Writeoff est: ProcFeeUCR - ProcFeeAllowed
            // -> 1000 - 900=100
            //InsPay est: (ProcFeeAllowed - DeductibleRem)-(ProcFeeAllowed - DeductibleRem)*PercentageCoverage || if DeductibleRem > ProcFeeAllowed : 0
            // -> (900 - 1400) - (900 - 1400) * 0.1 = || 0 
            //Pat Portion est (Annual Max Met): ProcFeeUCR
            // -> 1000
            //Pat Portion est (Annual Max NOT Met): ProcFeeUCR - InsPayEst - write off || ProcFeeAllowed - InsPayEst
            // -> 1000 - 0 - 100 = 900 || 900 - 0 = 900
            //DeductibleRem after this claim proc: DeductibleRem - ProcFeeAllowed
            // -> 1400 - 900 = 500
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(100.00, claimProc1.WriteOffEst);
            Assert.AreEqual(0.00, claimProc1.InsPayEst);
            Assert.AreEqual(900.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            //Second claimProc: ProcFeeUCR=1000 ProcFeeAllowed=900 Deductible=500 PercentageCoverage=(100-90)/100 DeductibleRem=500
            //Writeoff est: ProcFeeUCR - ProcFeeAllowed
            // -> 1000 - 900 = 100
            //InsPay est: (ProcFeeAllowed - DeductibleRem)-(ProcFeeAllowed - DeductibleRem)*PercentageCoverage || if DeductibleRem > ProcFeeAllowed : 0
            // -> (900 - 500) - (900 - 500) * 0.1 = 360.00
            //Pat Portion est (Annual Max Met): ProcFeeUCR
            // -> 1000
            //Pat Portion est (Annual Max NOT Met): ProcFeeUCR - InsPayEst - write off || ProcFeeAllowed - InsPayEst
            // -> 1000 - 360 - 100 = 90 || 900 - 360 = 540
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(100, claimProc2.WriteOffEst);
            Assert.AreEqual(360.00, claimProc2.InsPayEst);
            Assert.AreEqual(540.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            //Third claimProc: ProcFeeUCR=1000 ProcFeeAllowed=900 Deductible=500 PercentageCoverage=(100-90)/100 DeductibleRem=0
            //Writeoff est: ProcFeeUCR - ProcFeeAllowed
            // -> 1000 - 900 = 100
            //InsPay est: (ProcFeeAllowed - DeductibleRem)-(ProcFeeAllowed - DeductibleRem)*PercentageCoverage || if DeductibleRem > ProcFeeAllowed : 0
            // -> (900 - 0) - (900 - 0) * 0.1 = 810
            //Pat Portion est (Annual Max Met): ProcFeeUCR
            // -> 1000
            //Pat Portion est (Annual Max NOT Met): ProcFeeUCR - InsPayEst - write off || ProcFeeAllowed - InsPayEst
            // -> 1000 - 810 - 100 = 90 || 900 - 810 = 90
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(100.00, claimProc3.WriteOffEst);
            Assert.AreEqual(810.00, claimProc3.InsPayEst);
            Assert.AreEqual(90.00, (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        #endregion
        #region InsPlansZeroWriteOffsOnFreqOrAging
        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_FrequencySurpassedZerosWriteoff_Global)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when frequency or aging is entirely exceeded is set to true, plan override is set to default. Benefits include frequency limitation of 2 crowns, crowns 90%. Three procedures are treatment planned: all crowns for $110, the third procedure causes the account to go over the frequency limitation The third TP must recieve a $0 estimated write off.
<table>
  <tr>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverFrequencyGlobalLevel()
        {
            //3 procedures, insurance coverage is $81 with a Frequency limit of 2.
            // 1 - First proc will get full coverage for the $81, with a $20 writeoff.
            // 2 - Second proc will get partial coverage for the $81, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $0 writeoff, since our InsPlansZeroWriteOffsOnAnnualMax is set to true globally but plan level is set to default.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix);
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateFrequencyLimitation("D2750", 2, BenefitQuantity.NumberOfServices, plan.PlanNum, BenefitTimePeriod.CalendarYear);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnFreqOrAging, true);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc2.WriteOffEst);
            Assert.AreEqual(81, claimProc2.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(110, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_FrequencySurpassedZerosWriteoff_Plan)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when frequency or aging is entirely exceeded is set to false, plan override is set to true. Benefits include frequency limitation of 2 crowns, crowns 90%. Three procedures are treatment planned: all crowns for $110, the third procedure causes the account to go over the frequency limitation The third TP must recieve a $0 estimated write off.
<table>
  <tr>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverFrequencyPlanLevel()
        {
            //3 procedures, insurance coverage is $81 with a Frequency limit of 2.
            // 1 - First proc will get full coverage for the $81, with a $20 writeoff.
            // 2 - Second proc will get partial coverage for the $81, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $0 writeoff, since our InsPlansZeroWriteOffsOnAnnualMax is set to false globally but plan level is set to true.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix);
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            plan.InsPlansZeroWriteOffsOnFreqOrAgingOverride = YN.Yes; //Set plan level override.
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateFrequencyLimitation("D2750", 2, BenefitQuantity.NumberOfServices, plan.PlanNum, BenefitTimePeriod.CalendarYear);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded to false.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnFreqOrAging, false);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc2.WriteOffEst);
            Assert.AreEqual(81, claimProc2.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(110, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_FrequencySurpassedZerosWriteoff_Off)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when frequency or aging is entirely exceeded is set to false, plan override is set to default. Benefits include frequency limitation of 2 crowns, crowns 90%. Three procedures are treatment planned: all crowns for $110, the third procedure causes the account to go over the frequency limitation The third TP must recieve a $20 estimated write off.
<table>
  <tr>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>2</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$20.00</td>
    <td>$90.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverFrequencyGlobalLevelOff()
        {
            //3 procedures, insurance coverage is $81 with a Frequency limit of 2.
            // 1 - First proc will get full coverage for the $81, with a $20 writeoff.
            // 2 - Second proc will get partial coverage for the $81, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $20 writeoff, since our InsPlansZeroWriteOffsOnAnnualMax is set to false (globally and plan level is set to default).
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix);
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            plan.InsPlansZeroWriteOffsOnFreqOrAgingOverride = YN.Unknown; //Set plan level override.
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateFrequencyLimitation("D2750", 2, BenefitQuantity.NumberOfServices, plan.PlanNum, BenefitTimePeriod.CalendarYear);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded to false.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnFreqOrAging, false);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc2.WriteOffEst);
            Assert.AreEqual(81, claimProc2.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(90, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_AgingSurpassedZerosWriteoff_Global)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when frequency or aging is entirely exceeded is set to true, plan override is set to default. Benefits include aging limitation of 26 year olds, crowns 90%. Three procedures are treatment planned: all crowns for $110, for the second and third procedure the patient age will go over the aging limitation. The third and second TP must recieve a $0 estimated write off.
<table>
  <tr>
	<td><b>Patient Age</b></td>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
	<td>25</td>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
	<td>26</td>
    <td>2</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
  <tr>
	<td>27</td>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverAgingGlobalLevel()
        {
            //3 procedures, insurance coverage is $81 with a age limit of 25. Patient is 26. Write off's will be $0 due to PrefName.InsPlansZeroWriteOffsOnFreqOrAging being enabled.
            // 1 - First proc will get no coverage, with a $0 writeoff.
            // 2 - Second proc will get no coverage, with a $0 writeoff.
            // 3 - Third proc will get no coverage, with a $0 writeoff.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix, birthDate: DateTime.Today.AddDays(-1).AddYears(-26));
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateAgeLimitation(plan.PlanNum, EbenefitCategory.Crowns, 26, BenefitCoverageLevel.Individual, proc1.CodeNum);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnFreqOrAging, true);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age + i, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc2.WriteOffEst);
            Assert.AreEqual(0, claimProc2.InsPayEst);
            Assert.AreEqual(110.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(110, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_AgingSurpassedZerosWriteoff_Plan)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when frequency or aging is entirely exceeded is set to false, plan override is set to true. Benefits include aging limitation of 26 year olds, crowns 90%. Three procedures are treatment planned: all crowns for $110, for the second and third procedure the patient age will go over the aging limitation. The third and second TP must recieve a $0 estimated write off.
<table>
  <tr>
	<td><b>Patient Age</b></td>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
	<td>25</td>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
	<td>26</td>
    <td>2</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
  <tr>
	<td>27</td>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$0.00</td>
    <td>$110.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverAgingPlanLevel()
        {
            //3 procedures, insurance coverage is $81 with a age limit of 25. Patient is 26. Write off's will be $0 due to InsPlan.InsPlansZeroWriteOffsOnFreqOrAgingOverride being enabled.
            // 1 - First proc will get no coverage, with a $0 writeoff.
            // 2 - Second proc will get no coverage, with a $0 writeoff.
            // 3 - Third proc will get no coverage, with a $0 writeoff.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix, birthDate: DateTime.Today.AddYears(-26));
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            plan.InsPlansZeroWriteOffsOnFreqOrAgingOverride = YN.Yes; //Set plan level override.
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateAgeLimitation(plan.PlanNum, EbenefitCategory.Crowns, 26, BenefitCoverageLevel.Individual, proc1.CodeNum);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded to false.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnFreqOrAging, false);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age + i, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc2.WriteOffEst);
            Assert.AreEqual(0, claimProc2.InsPayEst);
            Assert.AreEqual(110.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(0, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(110, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }

        ///<summary></summary>
        [TestMethod]
        [Documentation.Numbering(Documentation.EnumTestNum.InsPlans_ComputeEstimates_AgingSurpassedZerosWriteoff_Off)]
        [Documentation.VersionAdded("22.3")]
        [Documentation.Description(@"Patient has one insurance plan, PPO, subscriber self. Global preference to zero out write-offs when frequency or aging is entirely exceeded is set to false, plan override is set to default. Benefits include aging limitation of 26 year olds, crowns 90%. Three procedures are treatment planned: all crowns for $110, for the second and third procedure the patient age will go over the aging limitation. All TP must recieve a $20 estimated write off.
<table>
  <tr>
	<td><b>Patient Age</b></td>
    <td><b>Procedure</b></td>
    <td><b>Fee</b></td>
    <td><b>Ins Pay Est.</b></td>
    <td><b>Write-off</b></td>
    <td><b>Pat portion</b></td>
  </tr>
  <tr>
	<td>25</td>
    <td>1</td>
    <td>$110.00</td>
    <td>$81.00</td>
    <td>$20.00</td>
    <td>$9.00</td>
  </tr>
  <tr>
	<td>26</td>
    <td>2</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$20.00</td>
    <td>$90.00</td>
  </tr>
  <tr>
	<td>27</td>
    <td>3</td>
    <td>$110.00</td>
    <td>$0.00</td>
    <td>$20.00</td>
    <td>$90.00</td>
  </tr>
</table>")]
        public void InsPlans_ComputeEstimates_ZeroWriteoffOverAgingGlobalLevelOff()
        {
            //3 procedures, insurance coverage is $81 with a age limit of 25. Patient is 26. Write off's will be $20 due to PrefName.InsPlansZeroWriteOffsOnFreqOrAging being disabled.
            // 1 - First proc will get no coverage, with a $20 writeoff.
            // 2 - Second proc will get no coverage, with a $20 writeoff.
            // 3 - Third proc will get no coverage, with a $20 writeoff.
            string suffix = "3";
            Patient pat = PatientT.CreatePatient(suffix, birthDate: DateTime.Today.AddYears(-26));
            //proc1 - Crown
            Procedure proc1 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc1, 0);
            //proc2 - Crown
            Procedure proc2 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc2, 1);
            //proc3 - Crown
            Procedure proc3 = ProcedureT.CreateProcedure(pat, "D2750", ProcStat.TP, "", 110);
            ProcedureT.SetPriority(proc3, 2);
            //FeeSched - PPO
            long feeSchedPPONum = FeeSchedT.CreateFeeSched(FeeScheduleType.Normal, "PPO");
            long feePPONum = FeeT.CreateFee(feeSchedPPONum, proc1.CodeNum, 90);
            //Insuarance Plan - PPO
            Carrier carrier = CarrierT.CreateCarrier(suffix);
            InsPlan plan = InsPlanT.CreateInsPlan(carrier.CarrierNum, feeSched: feeSchedPPONum);
            plan.PlanType = "p";
            plan.InsPlansZeroWriteOffsOnFreqOrAgingOverride = YN.Unknown; //Set plan level override.
            InsPlans.Update(plan);
            InsSub sub = InsSubT.CreateInsSub(pat.PatNum, plan.PlanNum);//guarantor is subscriber
            BenefitT.CreateAgeLimitation(plan.PlanNum, EbenefitCategory.Crowns, 26, BenefitCoverageLevel.Individual, proc1.CodeNum);
            BenefitT.CreateCategoryPercent(plan.PlanNum, EbenefitCategory.Crowns, 90);
            //Set pref to 0 write offs after annual max is exceeded to false.
            PrefT.UpdateBool(PrefName.InsPlansZeroWriteOffsOnFreqOrAging, false);
            //BenefitT.CreateFrequencyProc(plan.PlanNum,"D0274",BenefitQuantity.Years,1);//BW frequency every 1 year
            PatPlanT.CreatePatPlan(1, pat.PatNum, sub.InsSubNum);
            //Lists:
            List<ClaimProc> claimProcs = ClaimProcs.Refresh(pat.PatNum);
            List<ClaimProc> claimProcListOld = new List<ClaimProc>();
            Family fam = Patients.GetFamily(pat.PatNum);
            List<InsSub> subList = InsSubs.RefreshForFam(fam);
            List<InsPlan> planList = InsPlans.RefreshForSubList(subList);
            List<PatPlan> patPlans = PatPlans.Refresh(pat.PatNum);
            List<Benefit> benefitList = Benefits.Refresh(patPlans, subList);
            List<ClaimProcHist> histList = new List<ClaimProcHist>();
            List<ClaimProcHist> loopList = new List<ClaimProcHist>();
            List<Procedure> ProcList = Procedures.Refresh(pat.PatNum);
            List<Procedure> ListProceduresTPs = Procedures.GetListTPandTPi(ProcList);//sorted by priority, then toothnum
                                                                                     //Validate
            for (int i = 0; i < ListProceduresTPs.Count; i++)
            {
                Procedures.ComputeEstimates(ListProceduresTPs[i], pat.PatNum, ref claimProcs, false, planList, patPlans, benefitList,
                    histList, loopList, false, pat.Age + i, subList);
                //then, add this information to loopList so that the next procedure is aware of it.
                loopList.AddRange(ClaimProcs.GetHistForProc(claimProcs, ListProceduresTPs[i], ListProceduresTPs[i].CodeNum));
            }
            //save changes in the list to the database
            ClaimProcs.Synch(ref claimProcs, claimProcListOld);
            claimProcs = ClaimProcs.Refresh(pat.PatNum);
            long subNum = sub.InsSubNum;
            ClaimProc claimProc1 = ClaimProcs.GetEstimate(claimProcs, proc1.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc1.WriteOffEst);
            Assert.AreEqual(81, claimProc1.InsPayEst);
            Assert.AreEqual(9.00, (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            Assert.AreEqual(proc1.ProcFeeTotal, claimProc1.WriteOffEst + claimProc1.InsPayEst + (double)ClaimProcs.GetPatPortion(proc1, new List<ClaimProc>() { claimProc1 }));
            ClaimProc claimProc2 = ClaimProcs.GetEstimate(claimProcs, proc2.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc2.WriteOffEst);
            Assert.AreEqual(0, claimProc2.InsPayEst);
            Assert.AreEqual(90.00, (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            Assert.AreEqual(proc2.ProcFeeTotal, claimProc2.WriteOffEst + claimProc2.InsPayEst + (double)ClaimProcs.GetPatPortion(proc2, new List<ClaimProc>() { claimProc2 }));
            ClaimProc claimProc3 = ClaimProcs.GetEstimate(claimProcs, proc3.ProcNum, plan.PlanNum, subNum);
            Assert.AreEqual(20, claimProc3.WriteOffEst);
            Assert.AreEqual(0, claimProc3.InsPayEst);
            Assert.AreEqual(90, ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
            Assert.AreEqual(proc3.ProcFeeTotal, claimProc3.WriteOffEst + claimProc3.InsPayEst + (double)ClaimProcs.GetPatPortion(proc3, new List<ClaimProc>() { claimProc3 }));
        }
        #endregion

    }
}
