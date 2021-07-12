using System;
using System.Collections.Generic;
using System.Linq;

namespace Keon.Util.Extensions
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
						 + str[1..].ToLower();
		}

		///<summary>
		/// String format with words instead of number (e.g. {data} instead {0})
		///</summary>
		public static String Format(this String str, IDictionary<String, String> replaces)
		{
			return replaces.Aggregate(str, (current, replace) =>
						current.Replace("{{" + replace.Key + "}}", replace.Value));
		}



		/// <summary>
		/// Cast to Enum
		/// </summary>
		public static T Cast<T>(this String text)
			where T : struct
		{
			var type = typeof (T);

			if (!type.IsEnum)
				throw new ArgumentException("The type " + type + " is not an enum");

			return (T) Enum.Parse(typeof (T), text);
		}

	}
}
