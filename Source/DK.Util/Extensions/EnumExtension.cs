using System;

namespace DK.Util.Extensions
{
    ///<summary>
    /// Enum extensions to String
    ///</summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Transform enum value in CSS class (lower case, dash separating words)
        /// </summary>
        public static String ToCss(this Enum permission)
        {
            return permission
                .ToReadableString()
                .ToLower()
                .Replace(" ", "-");
        }

        /// <summary>
        /// Transform enum value em text, separating words by space
        /// </summary>
        public static String ToReadableString(this Enum permission)
        {
            var name = permission.ToString();
            var readableName = name[0].ToString();

            foreach (var letter in name.Substring(1))
            {
                if (letter >= 'A' && letter <= 'Z')
                {
                    readableName += " ";
                }

                readableName += letter;
            }

            return readableName;
        }

    }

}
