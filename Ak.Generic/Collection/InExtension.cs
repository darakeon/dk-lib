using System;
using System.Linq;

namespace Ak.Generic.Collection
{
    ///<summary>
    ///</summary>
    public static class InExtension
    {
        ///<summary>
        /// Whether an object is in a list
        ///</summary>
        public static Boolean In<T>(this T obj, params T[] possibilities)
        {
            return possibilities != null && possibilities.Contains(obj);
        }
    }
}