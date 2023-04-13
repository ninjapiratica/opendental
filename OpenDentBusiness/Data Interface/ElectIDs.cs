using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness
{

    public class ElectIDs
    {
        #region CachePattern

        private class ElectIDCache : CacheListAbs<ElectID>
        {
            protected override List<ElectID> GetCacheFromDb()
            {
                string command = "SELECT * from electid ORDER BY CarrierName";
                return Crud.ElectIDCrud.SelectMany(command);
            }
            protected override List<ElectID> TableToList(DataTable table)
            {
                return Crud.ElectIDCrud.TableToList(table);
            }
            protected override ElectID Copy(ElectID electID)
            {
                return electID.Copy();
            }
            protected override DataTable ListToTable(List<ElectID> listElectIDs)
            {
                return Crud.ElectIDCrud.ListToTable(listElectIDs, "ElectID");
            }
            protected override void FillCacheIfNeeded()
            {
                ElectIDs.GetTableFromCache(false);
            }
        }

        ///<summary>The object that accesses the cache in a thread-safe manner.</summary>
        private static ElectIDCache _electIDCache = new ElectIDCache();

        public static List<ElectID> GetDeepCopy(bool isShort = false)
        {
            return _electIDCache.GetDeepCopy(isShort);
        }

        private static ElectID GetFirstOrDefault(Func<ElectID, bool> match, bool isShort = false)
        {
            return _electIDCache.GetFirstOrDefault(match, isShort);
        }

        public static List<ElectID> GetWhere(Predicate<ElectID> match, bool isShort = false)
        {
            return _electIDCache.GetWhere(match, isShort);
        }

        ///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
        public static DataTable RefreshCache()
        {
            return GetTableFromCache(true);
        }

        ///<summary>Fills the local cache with the passed in DataTable.</summary>
        public static void FillCacheFromTable(DataTable table)
        {
            _electIDCache.FillCacheFromTable(table);
        }

        ///<summary>Always refreshes the ClientWeb's cache.</summary>
        public static DataTable GetTableFromCache(bool doRefreshCache)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                DataTable table = Meth.GetTable(MethodBase.GetCurrentMethod(), doRefreshCache);
                _electIDCache.FillCacheFromTable(table);
                return table;
            }
            return _electIDCache.GetTableFromCache(doRefreshCache);
        }

        #endregion

        private static ElectID[] list;

        public static long Insert(ElectID electID)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                electID.ElectIDNum = Meth.GetLong(MethodBase.GetCurrentMethod(), electID);
                return electID.ElectIDNum;
            }
            return Crud.ElectIDCrud.Insert(electID);
        }

        public static void Update(ElectID electID)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), electID);
                return;
            }
            Crud.ElectIDCrud.Update(electID);
        }

        public static bool Update(ElectID electID, ElectID oldElectID)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetBool(MethodBase.GetCurrentMethod(), electID, oldElectID);
            }
            return Crud.ElectIDCrud.Update(electID, oldElectID);
        }

        /// <summary>Takes a list of PayorIDs from DxC's getPayerListService API method. Inserts/updates new or existing electids.</summary>
        public static void UpsertFromDentalXChange(Dentalxchange2016.supportedTransPayer[] arraySupportedTransPayers)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), arraySupportedTransPayers);
                return;
            }
            bool hasChanged = false;
            for (int i = 0; i < arraySupportedTransPayers.Length; i++)
            {
                Dentalxchange2016.supportedTransPayer supportedTransPayer = arraySupportedTransPayers[i];
                ElectID electID = GetFirstOrDefault(x => x.PayorID == supportedTransPayer.PayerIDCode && x.CarrierName == supportedTransPayer.Name);
                if (electID is null)
                {
                    electID = new ElectID();
                    electID.CarrierName = supportedTransPayer.Name;
                    electID.PayorID = supportedTransPayer.PayerIDCode;
                    Insert(electID);
                    hasChanged = true;
                }
                else
                {
                    ElectID oldElectID = electID.Copy();
                    electID.CarrierName = supportedTransPayer.Name;
                    electID.PayorID = supportedTransPayer.PayerIDCode;
                    hasChanged |= Update(electID, oldElectID);
                }
            }
            if (hasChanged)
            {
                Signalods.SetInvalid(InvalidType.ElectIDs);
            }
        }

        ///<summary></summary>
        public static ProviderSupplementalID[] GetRequiredIdents(string payorID)
        {
            //No need to check MiddleTierRole; no call to db.
            ElectID electID = GetID(payorID);
            if (electID == null)
            {
                return new ProviderSupplementalID[0];
            }
            if (electID.ProviderTypes == "")
            {
                return new ProviderSupplementalID[0];
            }
            string[] provTypes = electID.ProviderTypes.Split(',');
            if (provTypes.Length == 0)
            {
                return new ProviderSupplementalID[0];
            }
            ProviderSupplementalID[] retVal = new ProviderSupplementalID[provTypes.Length];
            for (int i = 0; i < provTypes.Length; i++)
            {
                retVal[i] = (ProviderSupplementalID)(Convert.ToInt32(provTypes[i]));
            }
            /*
			if(electID=="SB601"){//BCBS of GA
				retVal=new ProviderSupplementalID[2];
				retVal[0]=ProviderSupplementalID.BlueShield;
				retVal[1]=ProviderSupplementalID.SiteNumber;
			}*/
            return retVal;
        }

        ///<summary>Gets ONE ElectID that uses the supplied payorID. Even if there are multiple payors using that ID.  So use this carefully.</summary>
        public static ElectID GetID(string payorID)
        {
            //No need to check MiddleTierRole; no call to db.
            return GetFirstOrDefault(x => x.PayorID == payorID);
        }

        ///<summary>Gets an arrayList of ElectID objects based on a supplied payorID. If no matches found, then returns array of 0 length. Used to display payors in FormInsPlan and also to get required idents.  This means that all payors with the same ID should have the same required idents and notes.</summary>
        public static List<ElectID> GetIDs(string payorID)
        {
            //No need to check MiddleTierRole; no call to db.
            return GetWhere(x => x.PayorID == payorID);
        }

        ///<summary>Gets the names of the payors to display based on the payorID.  Since carriers sometimes share payorIDs, there will often be multiple payor names returned.</summary>
        public static string[] GetDescripts(string payorID)
        {
            //No need to check MiddleTierRole; no call to db.
            if (payorID == "")
            {
                return new string[] { };
            }
            return GetIDs(payorID).Select(x => x.CarrierName).ToArray();
        }

        public static bool IsMedicaid(string payorID)
        {
            //No need to check MiddleTierRole; no call to db.
            ElectID electID = GetID(payorID);
            if (electID == null)
            {
                return false;
            }
            return electID.IsMedicaid;
        }
    }



}










