﻿using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDental
{
    public class ProcedureL
    {
        ///<summary>Sets all procedures for apt complete.  Flags procedures as CPOE as needed (when prov logged in).  Makes a log
        ///entry for each completed proc.  Then fires the CompleteProcedure automation trigger.</summary>
        public static List<Procedure> SetCompleteInAppt(Appointment apt, List<InsPlan> PlanList, List<PatPlan> patPlans, Patient patient,
            List<InsSub> subList, bool removeCompletedProcs)
        {
            List<Procedure> listProcsInAppt = Procedures.SetCompleteInAppt(apt, PlanList, patPlans, patient, subList, removeCompletedProcs);
            AutomationL.Trigger(AutomationTrigger.CompleteProcedure, listProcsInAppt.Select(x => ProcedureCodes.GetStringProcCode(x.CodeNum)).ToList(), apt.PatNum);
            Procedures.AfterProcsSetComplete(listProcsInAppt);
            return listProcsInAppt;
        }

        ///<summary>Returns empty string if no duplicates, otherwise returns duplicate procedure information.  In all places where this is called, we are guaranteed to have the eCW bridge turned on.  So this is an eCW peculiarity rather than an HL7 restriction.  Other HL7 interfaces will not be checking for duplicate procedures unless we intentionally add that as a feature later.</summary>
        public static string ProcsContainDuplicates(List<Procedure> procs)
        {
            bool hasLongDCodes = false;
            HL7Def defCur = HL7Defs.GetOneDeepEnabled();
            if (defCur != null)
            {
                hasLongDCodes = defCur.HasLongDCodes;
            }
            string info = "";
            List<Procedure> procsChecked = new List<Procedure>();
            for (int i = 0; i < procs.Count; i++)
            {
                Procedure proc = procs[i];
                ProcedureCode procCode = ProcedureCodes.GetProcCode(procs[i].CodeNum);
                string procCodeStr = procCode.ProcCode;
                if (procCodeStr.Length > 5
                    && procCodeStr.StartsWith("D")
                    && !hasLongDCodes)
                {
                    procCodeStr = procCodeStr.Substring(0, 5);
                }
                for (int j = 0; j < procsChecked.Count; j++)
                {
                    Procedure procDup = procsChecked[j];
                    ProcedureCode procCodeDup = ProcedureCodes.GetProcCode(procsChecked[j].CodeNum);
                    string procCodeDupStr = procCodeDup.ProcCode;
                    if (procCodeDupStr.Length > 5
                        && procCodeDupStr.StartsWith("D")
                        && !hasLongDCodes)
                    {
                        procCodeDupStr = procCodeDupStr.Substring(0, 5);
                    }
                    if (procCodeDupStr != procCodeStr)
                    {
                        continue;
                    }
                    if (procDup.ToothNum != proc.ToothNum)
                    {
                        continue;
                    }
                    if (procDup.ToothRange != proc.ToothRange)
                    {
                        continue;
                    }
                    if (procDup.ProcFee != proc.ProcFee)
                    {
                        continue;
                    }
                    if (procDup.Surf != proc.Surf)
                    {
                        continue;
                    }
                    if (info != "")
                    {
                        info += ", ";
                    }
                    info += procCodeDupStr;
                }
                procsChecked.Add(proc);
            }
            if (info != "")
            {
                info = Lan.g("ProcedureL", "Duplicate procedures") + ": " + info;
            }
            return info;
        }

        ///<summary>Checks to see if the appointments provider has at least one mismatch provider on all the completed procedures attached to the appointment.
        ///If so, checks to see if the user has permission to edit a completed procedure. If the user does, then the user has the option to change the provider to match.</summary>
        public static bool DoRemoveCompletedProcs(Appointment apt, List<Procedure> listProcsForAppt, bool checkForAllProcCompl = false)
        {
            if (listProcsForAppt.Count == 0)
            {
                return false;
            }
            if (checkForAllProcCompl && (apt.AptStatus != ApptStatus.Complete || listProcsForAppt.All(x => x.ProcStatus == ProcStat.C)))
            {
                return false;
            }
            List<Procedure> listCompletedProcWithDifferentProv = new List<Procedure>();
            foreach (Procedure proc in listProcsForAppt)
            {
                if (proc.ProcStatus != ProcStat.C || proc.AptNum != apt.AptNum)
                {//should all be complete already. 
                    continue;
                }
                ProcedureCode procCode = ProcedureCodes.GetProcCode(proc.CodeNum);
                long provNum = Procedures.GetProvNumFromAppointment(apt, proc, procCode);
                if (provNum != proc.ProvNum)
                {
                    listCompletedProcWithDifferentProv.Add(proc);
                }
            }
            if (listCompletedProcWithDifferentProv.Count == 0)
            {
                return true;//All completed procs match appt, so ignore completed procedures when running Procedures.UpdateProcsInApptHelper()
            }
            if (PrefC.GetBool(PrefName.ProcProvChangesClaimProcWithClaim))
            {
                List<ClaimProc> listClaimProcs = ClaimProcs.RefreshForProcs(listCompletedProcWithDifferentProv.Select(x => x.ProcNum).ToList());
                if (listClaimProcs.Any(x => x.Status == ClaimProcStatus.Received
                    || x.Status == ClaimProcStatus.Supplemental
                    || x.Status == ClaimProcStatus.CapClaim))
                {
                    MsgBox.Show("Procedures", "The appointment provider does not match the provider on at least one procedure that is attached "
                        + "to a claim.\r\nThe provider on the procedure(s) cannot be changed.");
                    return true;
                }
            }
            List<PaySplit> listPaySplit = PaySplits.GetPaySplitsFromProcs(listCompletedProcWithDifferentProv.Select(x => x.ProcNum).ToList());
            if (listPaySplit.Count > 0)
            {
                MsgBox.Show("Procedures", "The appointment provider does not match the provider on at least one completed procedure.\r\n"
                    + "The procedure provider cannot be changed to match the appointment provider because the paysplit provider would no longer match.  "
                    + "Any change to the provider on the completed procedure(s) or paysplit(s) will have to be made manually.");
                return true;//paysplits exist on one of the completed procedures. Per Nathan, don't change the provider. User will need to change manually.
            }
            foreach (Procedure proc in listCompletedProcWithDifferentProv)
            {
                Permissions perm = GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteEdit, proc);
                DateTime dateForPerm = Procedures.GetDateForPermCheck(proc);
                if (Security.IsGlobalDateLock(perm, dateForPerm))
                {
                    return true;
                }
                if (!Security.IsAuthorized(perm, dateForPerm, true, true))
                {
                    MessageBox.Show(Lan.g("Procedures", "The appointment provider does not match the provider on at least one completed procedure.") + "\r\n"
                        + Lans.g("Procedures", "Not authorized for") + ": " + GroupPermissions.GetDesc(perm) + "\r\n"
                        + Lan.g("Procedures", "Any change to the provider on the completed procedure(s) will have to be made manually."));
                    return true;//user does not have permission to change the provider. Don't change provider.
                }
            }
            //The appointment is set complete, completed procedures exist, and provider does not match appointment.
            //Ask if they would like to change the providers on the completed procedure to match the appointments provider
            if (!MsgBox.Show("Procedures", MsgBoxButtons.YesNo, "The appointment provider does not match the provider on at least one completed procedure.\r\n"
                + "Change the provider on the completed procedure(s) to match the provider on the appointment?"))
            {
                return true;//user does not want to change the providers
            }
            //user wants to change the provider on the completed procedure
            return false;
        }

        ///<summary>Checks if proc is Complete and has ClaimProcs attached to a Claim, and if so, prompts the user that this is invalid.</summary>
        public static bool IsProcCompleteAttachedToClaim(Procedure proc, List<ClaimProc> listClaimProcs, bool isSilent = false)
        {
            if (proc.ProcStatus == ProcStat.C && Procedures.IsAttachedToClaim(proc, listClaimProcs.FindAll(x => x.ProcNum == proc.ProcNum), false))
            {
                //status cannot be changed for completed procedures attached to a claim, except we allow changing status for preauths.
                if (!isSilent)
                {
                    MsgBox.Show("Procedures", "This is a completed procedure that is attached to a claim.  You must remove the procedure from the claim" +
                        " or delete the claim before editing the status.");
                }
                return true;
            }
            return false;
        }

        ///<summary></summary>
        public static bool ValidateProvider(List<ClaimProc> listClaimProcsForProc, long selectedProvNum, long provNumForProc)
        {
            Action<string> actionOnFailure = (msg) =>
            {
                MsgBox.Show(msg);
            };
            return Procedures.ValidateProvider(listClaimProcsForProc, selectedProvNum, provNumForProc, actionOnFailure);
        }

        ///<summary>Only needs to be called when procOld.ProcStatus is C, EO or EC.</summary>
        public static bool CheckPermissionsAndGlobalLockDate(Procedure procOld, Procedure procNew, DateTime procDate, double procFeeOverride = double.MinValue)
        {
            Action<string> actionOnFailure = (msg) =>
            {
                MsgBox.Show(msg);
            };
            return Procedures.CheckPermissionsAndGlobalLockDate(procOld, procNew, procDate, procFeeOverride, actionOnFailure);
        }

    }
}
