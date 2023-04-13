using System.Collections;
using System.Collections.Generic;

namespace CodeBase
{
    ///<summary>Working on deprecating</summary>
    public static class ODClassExtensions
    {

        ///<summary>Deprecated. Use Cast instead.</summary>
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable enumerable)
        {
            foreach (object obj in enumerable)
            {
                yield return (T)obj;
            }
        }
    }
}
