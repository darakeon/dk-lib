using System;
using System.Linq;

namespace Ak.Generic.Collection
{
    ///<summary>
    ///</summary>
    public static class IsInExtension
    {
        ///<summary>
        /// Whether an object is in a list
        ///</summary>
        public static Boolean IsIn<T>(this T obj, params T[] possibilities)
        {
            return possibilities != null && possibilities.Contains(obj);
        }
    }
}