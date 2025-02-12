using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness
{
    ///<summary></summary>
    public class JobProjects
    {

        ///<summary>Gets all projects, specify true to get all Done projects as well.</summary>
        public static List<JobProject> GetAll(bool isDone)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<JobProject>>(MethodBase.GetCurrentMethod(), isDone);
            }
            string command = "SELECT * FROM jobproject";
            if (!isDone)
            {
                command += " WHERE ProjectStatus != " + POut.Int((int)JobProjectStatus.Done);
            }
            return Crud.JobProjectCrud.SelectMany(command);
        }

        ///<summary></summary>
        public static List<JobProject> GetByRootProject(long projectNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<JobProject>>(MethodBase.GetCurrentMethod(), projectNum);
            }
            string command = "SELECT * FROM jobproject WHERE RootProjectNum = " + POut.Long(projectNum);
            return Crud.JobProjectCrud.SelectMany(command);
        }

        ///<summary></summary>
        public static List<JobProject> GetByParentProject(long projectNum, bool showFinished)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<List<JobProject>>(MethodBase.GetCurrentMethod(), projectNum, showFinished);
            }
            string command = "SELECT * FROM jobproject WHERE ParentProjectNum = " + POut.Long(projectNum);
            if (!showFinished)
            {
                command += " AND ProjectStatus != " + POut.Int((int)JobProjectStatus.Done);
            }
            return Crud.JobProjectCrud.SelectMany(command);
        }

        ///<summary>Gets one JobProject from the db.</summary>
        public static JobProject GetOne(long jobProjectNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                return Meth.GetObject<JobProject>(MethodBase.GetCurrentMethod(), jobProjectNum);
            }
            return Crud.JobProjectCrud.SelectOne(jobProjectNum);
        }

        ///<summary></summary>
        public static long Insert(JobProject jobProject)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                jobProject.JobProjectNum = Meth.GetLong(MethodBase.GetCurrentMethod(), jobProject);
                return jobProject.JobProjectNum;
            }
            return Crud.JobProjectCrud.Insert(jobProject);
        }

        ///<summary></summary>
        public static void Update(JobProject jobProject)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), jobProject);
                return;
            }
            Crud.JobProjectCrud.Update(jobProject);
        }

        ///<summary></summary>
        public static void Delete(long jobProjectNum)
        {
            if (RemotingClient.MiddleTierRole == MiddleTierRole.ClientMT)
            {
                Meth.GetVoid(MethodBase.GetCurrentMethod(), jobProjectNum);
                return;
            }
            Crud.JobProjectCrud.Delete(jobProjectNum);
        }



    }
}