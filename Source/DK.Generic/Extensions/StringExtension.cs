using System;
using System.Collections.Generic;
using System.Linq;

namespace DK.Generic.Extensions
{
    ///<summary>
    ///</summary>
    public static class StringExtension
    {
        ///<summary>
        /// Puts the first letter in Upper and the rest in Lower
        ///</summary>
        public static String Capitalize(this String str)
        {
            return str == null
                       ? null
                       : str.First().ToString().ToUpper()
                         + str.Substring(1).ToLower();
        }

        ///<summary>
        /// String format with words instead of number (e.g. {data} instead {0})
        ///</summary>
        public static String Format(this String str, IDictionary<String, String> replaces)
        {
            return replaces.Aggregate(str, (current, replace) =>
                        current.Replace("{{" + replace.Key + "}}", replace.Value));
        }

    }
}