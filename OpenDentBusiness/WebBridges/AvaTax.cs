﻿using Avalara.AvaTax.RestClient;
using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenDentBusiness
{

    ///<summary>This is an unusual bridge, currently available for HQ only that will handle sales tax calls to the Avalara API. 
    ///This is a wrapper class for calling the Avalara SDK, as we are not implementing our own interface for the API. 
    ///SDK Documentation: https://github.com/avadev/AvaTax-REST-V2-DotNet-SDK API Documentation: https://developer.avalara.com/avatax/dev-guide/. 
    ///</summary>
    public class AvaTax
    {

        #region Fields
        private static LogWriter _logWriter = new LogWriter(GetLogLevelDetail(), "AvaTax");
        #endregion Fields

        #region Constructor
        public AvaTax()
        {
        }
        #endregion Constructor

        #region Methods - Public
        ///<summary>True if we are in HQ and AvaTax is enabled.</summary>
        public static bool IsEnabled()
        {
            return PrefC.IsODHQ && Programs.IsEnabled(ProgramName.AvaTax);
        }

        ///<summary>True if we are in HQ, AvaTax is enabled, we tax the customer's state, and either the customer's tax exempt field is not defined or 
        ///they are explicitly not tax exempt.  Executes a small query.</summary>
        public static bool IsTaxable(long patNum)
        {
            if (!IsEnabled())
            {
                return false;//Save a few db calls
            }
            Patient patient = Patients.GetPat(patNum);
            if (patient == null)
            {
                return false;
            }
            PatField patFieldTaxExempt = null;
            if (GetTaxExemptPatField() != null)
            {
                patFieldTaxExempt = PatFields.Refresh(patNum).FirstOrDefault(x => x.FieldName == GetTaxExemptPatField().FieldName);
            }
            if (GetListTaxableStates().Count == 0)
            {
                return false;
            }
            if (!HasTaxableState(patient))
            { //if they aren't taxable, skip them
                return false;
            }
            if (patFieldTaxExempt != null && PIn.Bool(patFieldTaxExempt.FieldValue))
            {
                return false;
            }
            return true;
        }

        ///<summary>True if we are in HQ, AvaTax is enabled, we tax the customer's state, and the procedure has a taxable proccode. Note that if the 
        ///customer has an invalid zip but all other conditions are met, we will still return true because this procedure is still taxable by our rules
        ///we just know we would get an error back from Avalara. This method returning false will result in the procedure being created as before
        ///but without tax, so we need to return true for an invalid zip and let Avalara produce the error. Can execute two queries.</summary>
        public static bool DoSendProcToAvalara(Procedure procedure, bool isSilent = false)
        {
            ProcedureCode procedureCode = ProcedureCodes.GetProcCode(procedure.CodeNum);
            if (!IsEnabled())
            {
                return false;
            }
            if (procedure.ProcFee == 0)
            {//repeat charges for prepay use ProcFee=0
                return false;
            }
            Patient patient = Patients.GetPat(procedure.PatNum);
            //don't call IsTaxable() here, because that would get pat twice, so duplicating some if its functionality:
            if (patient == null)
            {
                return false;
            }
            if (GetListTaxableStates().Count == 0)
            {
                return false;//no taxable states
            }
            if (!HasTaxableState(patient))
            {//if this patient is not in a taxable state
                return false;
            }
            if (GetTaxExemptPatField() == null)
            {//no tax exempt pat field entered in setup
                return false;
            }
            PatField patFieldTaxExempt = PatFields.Refresh(patient.PatNum).FirstOrDefault(x => x.FieldName == GetTaxExemptPatField().FieldName);
            if (patFieldTaxExempt != null && PIn.Bool(patFieldTaxExempt.FieldValue))
            {//patient field exists and is true
                return false;//so not taxable
            }
            //end of the duplicated functionality from IsTaxable().	
            string procTaxCode = GetTaxOverrideIfNeeded(patient, procedureCode);//could be an avalara code, an override, or a percent
            if (string.IsNullOrWhiteSpace(procTaxCode))
            {//if there is no avalara code or percent for this proc/state
                return false;
            }
            if (!Patients.HasValidUSZipCode(patient))
            {
                if (isSilent)
                {
                    _logWriter.WriteLine($"Invalid ZipCode for PatNum {procedure.PatNum} while running Repeat Charge Tool on {DateTime.Today}", LogLevel.Error);
                }
                else
                {
                    //Remove the message box for now to avoid it from popping up on the server, stopping anyone using middletier to continue
                    //forward, because they can't click OK in the message box.
                    //MessageBox.Show("A valid zip code is required to process sales tax on procedures in this patient's state. "
                    //+"Please update the patient information with a valid zip code before continuing.");
                }
            }
            return true;
        }

        ///<summary>Checks to see if we need to replace the tax code for this state and procedure code with a different tax code.
        ///Returns the override, or if no override is found, returns the original tax code for the procedure.
        ///If there is a formatting error in the preference value, logs the offending section of the preference string,
        ///and attempts to find any other matching entries.  If no matching entries are found after an error
        ///occurs, throws an exception to prevent sending incorrect Avalara transactions.
        ///Exceptions thrown here will bubble up and be recorded into the adjustment note, so the user can see the error message.</summary>
        public static string GetTaxOverrideIfNeeded(Patient patient, ProcedureCode procedureCode)
        {
            string overrides = ProgramProperties.GetPropVal(ProgramName.AvaTax, "Tax Code Overrides");
            if (string.IsNullOrWhiteSpace(overrides))
            {
                return procedureCode.TaxCode;
            }
            string patStateLower = patient.State.ToLower();
            Address addressTax = Addresses.GetOneByPatNum(patient.PatNum);
            if (addressTax != null)
            {
                patStateLower = addressTax.State.ToLower();
            }
            bool stateHasPercentage = false;
            bool hasFormatError = false;
            string[] stringArrayOverrides = overrides.Split(',');
            foreach (string entry in stringArrayOverrides)
            {
                string[] stringArrayParts = entry.Split('-');
                if (stringArrayParts.Count() != 3)
                {
                    _logWriter.WriteLine("Tax Code Override entry is incorrect: " + entry + ".  "
                        + "Fix this entry in AvaTax setup to resume processing overrides.", LogLevel.Error);
                    hasFormatError = true;
                    continue;
                }
                string stateCode = stringArrayParts[0].Trim();
                string procCodeCur = stringArrayParts[1].Trim();
                string taxCodeCur = stringArrayParts[2].Trim();
                if (stateCode.ToLower() == patStateLower)
                {
                    if (taxCodeCur != "" && taxCodeCur.EndsWith("%"))
                    {
                        stateHasPercentage = true;
                    }
                }
                if (stateCode.ToLower() == patStateLower//Allowing state code to be case insensitive, since our database data is really inconsistent.
                    && procCodeCur == procedureCode.ProcCode)//We must use case sensitive proc codes, because OD allows (ex D0120 to exist with d0120).
                {
                    return taxCodeCur;
                }
            }
            if (stateHasPercentage)
            {//but matching code was not found.
                return "0%";//we don't want any other codes to be sent to Avalara
            }
            if (hasFormatError)
            {
                throw new ODException("Unable to parse tax code overrides due to formatting error.");
            }
            return procedureCode.TaxCode;
        }

        ///<summary>Pings the API service with the provided test settings to check if the settings will work and the API is available.</summary>
        public static bool IsApiAvailable(bool isProduction, string username, string password)
        {
            AvaTaxClient avaTaxClientTest = new AvaTaxClient("TestClient", "1.0", Environment.MachineName, isProduction ? AvaTaxEnvironment.Production : AvaTaxEnvironment.Sandbox)
                .WithSecurity(username, password);
            return avaTaxClientTest.Ping().authenticated ?? false;
        }

        ///<summary>Tests our connection to the API client using the stored credentials.  Returns true if we can connect.</summary>
        public static bool PingAvaTax()
        {
            return GetAvaTaxClient().Ping().authenticated ?? false;
        }

        ///<summary>Returns the tax estimate for this specific patient, procCode, and feeAmts.
        ///Calls AvaTax/CreateTransaction: https://developer.avalara.com/api-reference/avatax/rest/v2/methods/Transactions/CreateTransaction/ but does
        ///not save the transaction in the Avalara DB.</summary>
        public static decimal GetEstimate(long codeNum, long patNum, double procFee, bool hasExceptions = false)
        {
            if (!IsTaxable(patNum))
            {
                return 0;
            }
            ProcedureCode procedureCode = ProcedureCodes.GetProcCode(codeNum);
            string strTaxOverride = GetTaxOverrideIfNeeded(Patients.GetPat(patNum), procedureCode);
            if (strTaxOverride != "" && strTaxOverride.EndsWith("%"))
            {
                double percent = PIn.Double(strTaxOverride.TrimEnd('%'));
                return (decimal)(procFee * percent / 100d);
            }
            try
            {
                TransactionBuilder transactionBuilder = SetUpTransaction(DocumentType.SalesOrder, patNum);//Sales Order is AvaTax's way of getting an estimate
                transactionBuilder.WithLine((decimal)procFee, 1, strTaxOverride, procedureCode.Descript, procedureCode.ProcCode);
                TransactionModel transactionModel = GetAvaTaxClient().CreateTransaction("Lines", transactionBuilder.GetCreateTransactionModel());
                return transactionModel.totalTax.Value;
            }
            catch (Exception ex)
            {
                _logWriter.WriteLine($"Error getting estimate from Avatax for PatNum: {patNum}", LogLevel.Error);
                if (hasExceptions)
                {
                    throw ex;//Loses call stack, but everywhere that catches this only cares about the message.
                }
                ex.DoNothing();
                //For now we just enter $0 because we don't have any proc or adjustment to attach this to, and we already have logging for errors
                return 0;
            }
        }

        ///<summary>Returns true if the passed in adjustment needs to be subsequently saved to the database. Returns false otherwise. 
        ///Takes in a procedure and a potential sales tax adjustment and gets the expected tax amount for the procedure. If there is not tax
        ///and the adjustment has not been inserted into the database, then we do not create an adjustment. Otherwise, we save the sax and insert the 
        ///adjustment (even if there is an existing tax, and we are setting adjustment amount to 0). Finally all errors encountered are logged in the
        ///adjustment note and on the machine's local logs.</summary>
        public static bool DidUpdateAdjustment(Procedure procedure, Adjustment adjustment)
        {
            string message = "";
            try
            {
                //Get the new sum of all adjustments attached to the proc, excluding sales tax and including the new adjustment amount if applicable
                double procTotal = procedure.ProcFeeTotal + Adjustments.GetTotForProc(procedure.ProcNum, canIncludeTax: false);
                decimal taxAmt = GetEstimate(procedure.CodeNum, procedure.PatNum, procTotal, hasExceptions: true);
                if (taxAmt == 0 && adjustment.AdjNum == 0)
                { //We could be modifying an existing adjustment, in which case we would want to set the 0 value
                    return false;
                }
                adjustment.AdjAmt = (double)taxAmt;
                return true;
            }
            catch (AvaTaxError avaTaxError)
            {
                _logWriter.WriteLine("Encountered an Avatax error: " + JsonConvert.SerializeObject(avaTaxError.error.error), LogLevel.Error);
                message = avaTaxError.error.error.message;
            }
            catch (Exception ex)
            {
                _logWriter.WriteLine("Unable to send or receive transaction: " + JsonConvert.SerializeObject(ex), LogLevel.Error);
                message = ex.Message;
            }
            adjustment.AdjNote = AddNote(adjustment.AdjNote, "An error occurred processing the transaction: " + message + " See local logs for more details.");
            adjustment.AdjAmt = 0;
            return true;
        }

        public static bool DoCreateReturnAdjustment(Procedure procedure, Adjustment adjustmentLocked, Adjustment adjustmentReturn)
        {
            try
            {
                double procTotal = procedure.ProcFeeTotal + Adjustments.GetTotForProc(procedure.ProcNum, canIncludeTax: false);
                decimal taxEstNew = AvaTax.GetEstimate(procedure.CodeNum, procedure.PatNum, procTotal, hasExceptions: true);
                adjustmentReturn.AdjAmt = (Adjustments.GetTotTaxForProc(procedure) - (double)taxEstNew) * (-1);
                if (adjustmentReturn.AdjAmt == 0)
                {
                    return false; //no error and we would be refunding $0 to the customer, so no need to create a return adjustment
                }
            }
            catch (Exception e)
            {
                adjustmentReturn.AdjNote = AvaTax.AddNote(adjustmentReturn.AdjNote, "An error occurred processing the transaction: " + e.Message + " See local logs for more details.");
            }
            return true;
        }

        ///<summary>Creates a bulk estimate currently specifically for the pre-payment tool.  Takes in a special dictionary of procedurecodes mapped
        ///to a list of tuples containing the qty and single item prices for the provided procedure codes, then builds a single transaction to
        ///hold the data.  This method creates its own procedures and adjustments.</summary>
        public static void CreatePrepaymentTransaction(Dictionary<ProcedureCode, List<TransQtyAmt>> dictionaryProcsToTransQtyAmts, Patient patient, List<Procedure> listCompletedProcs)
        {
            Dictionary<Procedure, int> dictionaryProcsToCreate = new Dictionary<Procedure, int>();
            //Build the transaction for all of the items in a single transaction.
            TransactionBuilder transactionBuilder = SetUpTransaction(DocumentType.SalesOrder, patient.PatNum);//Sales Order is AvaTax's way of getting an estimate
            foreach (ProcedureCode procCode in dictionaryProcsToTransQtyAmts.Keys)
            {
                Procedure procedure = new Procedure()
                {
                    PatNum = patient.PatNum,
                    ProvNum = 7,//HQ only this is jordan's provNum.
                    ProcDate = DateTime.Today,
                    DateEntryC = DateTime.Now,
                    DateComplete = DateTime.Now,
                    CodeNum = procCode.CodeNum,
                    ProcStatus = ProcStat.C
                };
                int totalCount = 0;
                foreach (TransQtyAmt pair in dictionaryProcsToTransQtyAmts[procCode])
                {
                    totalCount += pair.Qty;
                    procedure.BillingNote += "Rate: $" + POut.Double(pair.Rate) + " Months: " + POut.Int(pair.Qty) + "\r\n";
                    transactionBuilder.WithLine(pair.Qty * (decimal)pair.Rate, (decimal)pair.Qty,
                        GetTaxOverrideIfNeeded(patient, procCode), procCode.Descript, procCode.ProcCode);
                }
                //Add note for what day the customer has prepaid through.
                DateTime datePrepaidThrough = DateTimeOD.GetMostRecentValidDate(DateTime.Today.Year, DateTime.Today.Month, patient.BillingCycleDay)
                    .AddMonths(totalCount).AddDays(-1);
                if (DateTime.Today.Day >= patient.BillingCycleDay)
                {
                    datePrepaidThrough = datePrepaidThrough.AddMonths(1);
                }
                procedure.BillingNote += $"Prepaid through: {datePrepaidThrough.Date:MM/dd/yyyy}";
                dictionaryProcsToCreate.Add(procedure, totalCount);
            }
            TransactionModel transactionModelResult = GetAvaTaxClient().CreateTransaction("Lines", transactionBuilder.GetCreateTransactionModel());
            List<ProcedureCode> listProcedureCodesDiscount = AvaTax.GetListProcedureCodesDiscount();
            //Create a single procedure and adjustment for each procCode.
            foreach (KeyValuePair<Procedure, int> entry in dictionaryProcsToCreate)
            {
                //look for previously completed procedures that need to be included on the adjustment calculation.
                List<Procedure> listProceduresMatchingCompleted = listCompletedProcs.FindAll(x => x.CodeNum == entry.Key.CodeNum);
                Procedure procedure = entry.Key;
                int count = entry.Value + listProceduresMatchingCompleted.Count();
                List<TransactionLineModel> listTransactionLineModels = transactionModelResult.lines.FindAll(x => x.itemCode.Equals(ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode));
                procedure.ProcFee = (double)listTransactionLineModels.Sum(x => x.lineAmount.Value);
                procedure.TaxAmt = (double)listTransactionLineModels.Sum(x => x.tax.Value);
                procedure.ProcNum = Procedures.Insert(procedure, doCalcTax: false);
                if (count > 5 && listProcedureCodesDiscount.Exists(x => x.ProcCode == ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode))
                {
                    //Create a discount adjustment.
                    if (count >= 6 && count <= 11)
                    {
                        CreateDiscountAdjustment(procedure, .05, 255);//5% discount.  Hard coded ODHQ defnum.
                    }
                    else if (count >= 12 && count <= 23)
                    {
                        CreateDiscountAdjustment(procedure, .10, 206);//10% discount.  Hard coded ODHQ defnum.
                    }
                    else if (count >= 24)
                    {
                        CreateDiscountAdjustment(procedure, .15, 229);//15% discount.  Hard coded ODHQ defnum.
                    }
                    //Create adjustments for the previously completed procedures.
                    foreach (Procedure proc in listProceduresMatchingCompleted)
                    {
                        if (count >= 6 && count <= 11)
                        {
                            CreateDiscountAdjustment(proc, .05, 255);//5% discount.  Hard coded ODHQ defnum.
                        }
                        else if (count >= 12 && count <= 23)
                        {
                            CreateDiscountAdjustment(proc, .10, 206);//10% discount.  Hard coded ODHQ defnum.
                        }
                        else if (count >= 24)
                        {
                            CreateDiscountAdjustment(proc, .15, 229);//15% discount.  Hard coded ODHQ defnum.
                        }
                    }
                }
            }
        }

        ///<summary>A program property.  The adjustment type defnum to be associated with SalesTax adjustments.</summary>
        public static long GetSalesTaxAdjType()
        {
            return PIn.Long(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.AvaTax), "Sales Tax Adjustment Type"));
        }

        ///<summary>A program property.  The adjustment type defnum to be associated with SalesTax return adjustments.</summary>
        public static long GetSalesTaxReturnAdjType()
        {
            return PIn.Long(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.AvaTax), "Sales Tax Return Adjustment Type"));
        }

        ///<summary>A program property.
        ///Required by the API when sending transactions to determine the origination company with which to associate the transaction.</summary>
        public static string GetCompanyCode()
        {
            return PIn.String(ProgramProperties.GetPropVal(ProgramName.AvaTax, "Company Code"));
        }

        ///<summary>A program property. The list of two-letter state codes where sales tax will be collected. </summary>
        public static List<string> GetListTaxableStates()
        {
            return ProgramProperties.GetPropVal(ProgramName.AvaTax, "Taxable States").Split(',').ToList();
        }

        ///<summary>A program property. Indicates if we are currently using the sandbox or production API.</summary>
        public static bool IsProduction()
        {
            return PIn.String(ProgramProperties.GetPropVal(ProgramName.AvaTax, "Test (T) or Production (P)")) == "P";
        }

        ///<summary>A program property. Indicates the level of detail to log about calls made to the Avalara API.
        ///Defaults to Error if the program property is not found or if there was an error parsing the value.</summary>
        public static LogLevel GetLogLevelDetail()
        {
            LogLevel logLevel = LogLevel.Error;
            ODException.SwallowAnyException(() =>
            {
                logLevel = PIn.Enum<LogLevel>(ProgramProperties.GetPropVal(ProgramName.AvaTax, "Log Level"));
            });
            return logLevel;
        }

        ///<summary>A program property.  The list of procedure codes we will allow users to pre-pay for in the Pre-payment tool.</summary>
        public static List<ProcedureCode> GetListProcedureCodesPrePay()
        {
            List<ProcedureCode> listProcedureCodes = new List<ProcedureCode>();
            foreach (string procCode in ProgramProperties.GetPropVal(ProgramName.AvaTax, "Prepay Proc Codes").Split(','))
            {
                listProcedureCodes.Add(ProcedureCodes.GetProcCode(procCode));
            }
            return listProcedureCodes;
        }

        ///<summary>A program property.  The list of procedure codes we will allow users to pre-pay for in the Pre-payment tool.</summary>
        public static List<ProcedureCode> GetListProcedureCodesDiscount()
        {
            List<ProcedureCode> listProcedureCodes = new List<ProcedureCode>();
            foreach (string procCode in ProgramProperties.GetPropVal(ProgramName.AvaTax, "Discount Proc Codes").Split(','))
            {
                listProcedureCodes.Add(ProcedureCodes.GetProcCode(procCode));
            }
            return listProcedureCodes;
        }

        ///<summary>A program property.  Returns the Pat Field Def Num to be associated with marking patients sales tax exempt.</summary>
        public static PatFieldDef GetTaxExemptPatField()
        {
            return PatFieldDefs.GetFirstOrDefault(x => x.PatFieldDefNum == PIn.Long(
                ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.AvaTax), "Tax Exempt Pat Field Def")
            ));
        }

        ///<summary>A program property.  Returns the date after which we will consider tax adjustments "un-locked" and directly editable.
        ///Example: lock date of 3/31/2019, so locked on that date, and unlocked if on 4/1/2019.</summary>
        public static DateTime GetTaxLockDate()
        {
            return PIn.Date(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.AvaTax), "Tax Lock Date"));
        }

        ///<summary>Creates and inserts a discount adjustment for the passed in procedure.  Used by prepayment tool.</summary>
        public static void CreateDiscountAdjustment(Procedure procedure, double discountPercentage, long adjType)
        {
            Adjustment adjustment = new Adjustment
            {
                DateEntry = DateTime.Today,
                AdjDate = DateTime.Today,
                ProcDate = procedure.ProcDate,
                ProvNum = procedure.ProvNum,
                ProcNum = procedure.ProcNum,
                ClinicNum = procedure.ClinicNum,
                PatNum = procedure.PatNum,
                AdjType = adjType,
                AdjAmt = -(procedure.ProcFee * discountPercentage)//Flip the sign to make it a negative adjustment.
            };
            Adjustments.Insert(adjustment);
            TsiTransLogs.CheckAndInsertLogsIfAdjTypeExcluded(adjustment);
        }
        #endregion Methods - Public

        #region Methods - Private
        ///<summary>The API client based on the current environment and credentials provided by the program properties.</summary>
        private static AvaTaxClient GetAvaTaxClient()
        {
            //Decrypt password for API.
            string password = CDT.Class1.TryDecrypt(ProgramProperties.GetPropVal(ProgramName.AvaTax, ProgramProperties.PropertyDescs.Password));
            return new AvaTaxClient("AvaTaxClient", "1.0", Environment.MachineName, IsProduction() ? AvaTaxEnvironment.Production : AvaTaxEnvironment.Sandbox)
                .WithSecurity(ProgramProperties.GetPropVal(ProgramName.AvaTax, ProgramProperties.PropertyDescs.Username), password);
        }

        ///<summary>If there is already text in the previous note, adds a newline before adding the next note.</summary>
        private static string AddNote(string prevNote, string nextNote)
        {
            if (!string.IsNullOrEmpty(prevNote))
            {
                prevNote += Environment.NewLine;
            }
            prevNote += DateTime.Now + " " + nextNote;
            return prevNote;
        }

        ///<summary>Sets up a transaction builder to which we can attach line items later, using the base office and patient address info.
        ///Also specifies the transaction DocumentType: https://developer.avalara.com/api-reference/avatax/rest/v2/models/enums/DocumentType/. </summary>
        private static TransactionBuilder SetUpTransaction(DocumentType documentType, long patNum)
        {
            Patient patient = Patients.GetPat(patNum);
            Address addressTax = Addresses.GetOneByPatNum(patNum);
            TransactionBuilder transactionBuilder = new TransactionBuilder(
                GetAvaTaxClient(),//AvaTaxClient
                AvaTax.GetCompanyCode(),
                documentType,
                POut.Long(patient.PatNum));//We will use the customer's PatNum as the "Customer Code"
                                           //Add main office address.  In the future, if we make this available to customers, then we might need to implement clinics here.
            transactionBuilder.WithAddress(
                TransactionAddressType.ShipFrom,
                PrefC.GetString(PrefName.PracticeAddress),
                PrefC.GetString(PrefName.PracticeAddress2),
                "",
                PrefC.GetString(PrefName.PracticeCity),
                PrefC.GetString(PrefName.PracticeST),
                PrefC.GetString(PrefName.PracticeZip),
                "US"//US only.  Otherwise, in future we might use System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName
            );
            //Add customer address. If the customer has a physical tax address override, use that instead.
            if (addressTax == null)
            {
                transactionBuilder.WithAddress(
                    TransactionAddressType.ShipTo,
                    patient.Address,
                    patient.Address2,
                    "",
                    patient.City,
                    patient.State,
                    patient.Zip,
                    "US"//US only.  Otherwise, in future we might use patient.Country
                );
            }
            else
            {
                transactionBuilder.WithAddress(
                    TransactionAddressType.ShipTo,
                    addressTax.Address1,
                    addressTax.Address2,
                    "",
                    addressTax.City,
                    addressTax.State,
                    addressTax.Zip,
                    "US"//US only.  Otherwise, in future we might use patient.Country
                );
            }
            //Record the user who was logged in when this transaction occurred for later reference
            transactionBuilder.WithSalespersonCode(Security.CurUser.UserName);
            return transactionBuilder;
        }

        ///<summary>Makes call to db</summary>
        private static bool HasTaxableState(Patient patient)
        {
            Address addressTax = Addresses.GetOneByPatNum(patient.PatNum);
            //Use the physical tax address instead of the pat address, if they have one
            bool hasTaxedState = GetListTaxableStates().Any(x => x == patient.State);
            if (addressTax != null)
            {
                hasTaxedState = GetListTaxableStates().Any(x => x == addressTax.State);
            }
            return hasTaxedState;
        }
        #endregion Methods - Private

        #region Classes - Nested
        public class TransQtyAmt
        {
            public int Qty;
            public double Rate;
        }
        #endregion Classes - Nested
    }
}
