using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class FormPats
    {
        ///<summary></summary>
        public static long Insert(FormPat Cur)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Cur.FormPatNum = Meth.GetLong(MethodBase.GetCurrentMethod(), Cur);
                return Cur.FormPatNum;
            }
            return Crud.FormPatCrud.Insert(Cur);
        }

        public static FormPat GetOne(long formPatNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<FormPat>(MethodBase.GetCurrentMethod(), formPatNum);
            }
            string command = "SELECT * FROM formpat WHERE FormPatNum=" + POut.Long(formPatNum);
            FormPat formpat = Crud.FormPatCrud.SelectOne(formPatNum);
            if (formpat == null)
            {
                return null;//should never happen.
            }
            command = "SELECT * FROM question WHERE FormPatNum=" + POut.Long(formPatNum);
            formpat.QuestionList = Crud.QuestionCrud.SelectMany(command);
            return formpat;
        }

        ///<summary></summary>
        public static void Delete(long formPatNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), formPatNum);
                return;
            }
            string command = "DELETE FROM formpat WHERE FormPatNum=" + POut.Long(formPatNum);
            Db.NonQ(command);
            command = "DELETE FROM question WHERE FormPatNum=" + POut.Long(formPatNum);
            Db.NonQ(command);
        }
    }




}













