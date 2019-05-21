using System;

namespace DK.Util.Enums
{
    ///<summary>
    /// Convert String to Enum
    ///</summary>
    public static class Str2Enum
    {
        ///<summary>
        ///</summary>
        ///<typeparam name="T">Enum type</typeparam>
        public static T Cast<T>(String value)
            where T : struct
        {
            return (T)Enum.Parse(typeof (T), value);
        }
    }
}
