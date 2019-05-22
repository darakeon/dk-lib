using System;
using System.Linq;
using System.Text;

namespace Keon.TwoFactorAuth
{
	/// <summary>
	/// https://code.tutsplus.com/tutorials/base-what-a-practical-introduction-to-base-encoding--net-27590
	/// </summary>
	public class Base32
	{
		/// <summary>
		/// Convert a string into Base32 code
		/// </summary>
		public static String Convert(String origin)
		{
			var bits = getBits(origin);

			var blocks = bits.BreakByRegex(".{5}");

			var translated = blocks
				.Select(b => System.Convert.ToInt32(b, 2))
				.Select(b => alphabet[b])
				.ToArray();

			var newText = new String(translated);

			return newText.FixBlockSize(8, '=');
		}

		private static string getBits(String origin)
		{
			var bytes = Encoding.UTF8.GetBytes(origin);

			var bits = string.Join("",
				bytes.Select(getBinary)
			);

			return bits.FixBlockSize(5, '0');
		}

		private static String getBinary(Byte b)
		{
			return System.Convert.ToString(b, 2).PadLeft(8, '0');
		}

		private static String alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
	}
}
