using OpenDentBusiness;
using System.Collections.Generic;

namespace UnitTestsCore
{
    public class TaskAncestorT
    {
        ///<summary>Gets all taskAncestors for a supplied TaskNum.</summary>
        public static List<TaskAncestor> GetAllForTask(long taskNum)
        {
            if (taskNum == 0)
            {
                return new List<TaskAncestor>();
            }
            string command = "SELECT * FROM taskancestor WHERE TaskNum = " + POut.Long(taskNum);
            return OpenDentBusiness.Crud.TaskAncestorCrud.SelectMany(command);
        }
    }
}
