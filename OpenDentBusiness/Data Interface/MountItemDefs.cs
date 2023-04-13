using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class MountItemDefs
    {
        public static List<MountItemDef> GetForMountDef(long mountDefNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<MountItemDef>>(MethodBase.GetCurrentMethod(), mountDefNum);
            }
            string command = "SELECT * FROM mountitemdef WHERE MountDefNum='" + POut.Long(mountDefNum) + "' ORDER BY ItemOrder";
            return Crud.MountItemDefCrud.SelectMany(command);
        }

        public static List<MountItemDef> GetAll()
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<MountItemDef>>(MethodBase.GetCurrentMethod());
            }
            string command = "SELECT * FROM mountitemdef ORDER BY ItemOrder";
            return Crud.MountItemDefCrud.SelectMany(command);
        }

        ///<summary></summary>
        public static void Update(MountItemDef mountItemDef)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), mountItemDef);
                return;
            }
            Crud.MountItemDefCrud.Update(mountItemDef);
        }

        ///<summary></summary>
        public static long Insert(MountItemDef mountItemDef)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                mountItemDef.MountItemDefNum = Meth.GetLong(MethodBase.GetCurrentMethod(), mountItemDef);
                return mountItemDef.MountItemDefNum;
            }
            return Crud.MountItemDefCrud.Insert(mountItemDef);
        }

        ///<summary>No need to surround with try/catch, because all deletions are allowed.</summary>
        public static void Delete(long mountItemDefNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), mountItemDefNum);
                return;
            }
            string command = "DELETE FROM mountitemdef WHERE MountItemDefNum=" + POut.Long(mountItemDefNum);
            Db.NonQ(command);
        }

        ///<summary></summary>
        public static void DeleteForMount(long mountDefNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), mountDefNum);
                return;
            }
            string command = "DELETE FROM mountitemdef WHERE MountDefNum=" + POut.Long(mountDefNum);
            Db.NonQ(command);
        }
    }













}










