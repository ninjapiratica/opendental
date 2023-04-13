using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class MapAreas
    {
        ///<summary>Pass in a MapAreaContainerNum to limit the list to a single room.  Otherwise all cubicles from every map will be returned.</summary>
        public static List<MapArea> Refresh(long mapAreaContainerNum = 0)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<MapArea>>(MethodBase.GetCurrentMethod(), mapAreaContainerNum);
            }
            string command = "SELECT * FROM maparea";
            if (mapAreaContainerNum > 0)
            {
                command += $" WHERE MapAreaContainerNum={POut.Long(mapAreaContainerNum)}";
            }
            return Crud.MapAreaCrud.SelectMany(command);
        }
        /*		
		///<summary>Gets one MapArea from the db.</summary>
		public static MapArea GetOne(long mapAreaNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<MapArea>(MethodBase.GetCurrentMethod(),mapAreaNum);
			}
			return Crud.MapAreaCrud.SelectOne(mapAreaNum);
		}
		*/
        ///<summary></summary>
        public static long Insert(MapArea mapArea)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                mapArea.MapAreaNum = Meth.GetLong(MethodBase.GetCurrentMethod(), mapArea);
                return mapArea.MapAreaNum;
            }
            return Crud.MapAreaCrud.Insert(mapArea);
        }

        ///<summary></summary>
        public static void Update(MapArea mapArea)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), mapArea);
                return;
            }
            Crud.MapAreaCrud.Update(mapArea);
        }

        ///<summary></summary>
        public static void Delete(long mapAreaNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), mapAreaNum);
                return;
            }
            string command = "DELETE FROM maparea WHERE MapAreaNum = " + POut.Long(mapAreaNum);
            Db.NonQ(command);
        }

        ///<summary></summary>
        public static void DeleteAll(long mapAreaContainerNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), mapAreaContainerNum);
                return;
            }
            string command = "DELETE FROM maparea WHERE MapAreaContainerNum = " + POut.Long(mapAreaContainerNum);
            Db.NonQ(command);
        }
    }
}