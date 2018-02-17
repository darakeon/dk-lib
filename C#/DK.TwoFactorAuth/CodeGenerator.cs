using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DK.TwoFactorAuth
{
	/// <summary>
	/// https://www.codementor.io/slavko/google-two-step-authentication-otp-generation-du1082vho
	/// </summary>
	public class CodeGenerator
	{
		/// <summary>
		/// Generate codes for present
		/// </summary>
		/// <param name="key">secret key to generate code</param>
		public static String Generate(String key)
		{
			return Generate(key, 0)[0];
		}

		/// <summary>
		/// Generate codes for presente, past and future,
		/// to compensate clock not synced
		/// </summary>
		/// <param name="key">secret key to generate code</param>
		/// <param name="range">number of past/future generations</param>
		public static IList<String> Generate(String key, Int32 range)
		{
			var result = new List<String>();

			for (var r = -range; r <= range; r++)
			{
				var hexTimestamp = getHexTimestamp(r);
				var message = getBytes(hexTimestamp);
				var code = getCode(message, key);
				
				result.Add(code);
			}

			return result;
		}

		private const Int32 size = 6;

		private static String getCode(Byte[] message, String key)
		{
			var bytes = Encoding.UTF8.GetBytes(key);
			var hmac = new HMACSHA1(bytes);

			var hash = hmac.ComputeHash(message);
			var position = hash.Last() & 0xf;

			var digit =
				(hash[position] & 0x7f) << 24 |
				(hash[++position] & 0xff) << 16 |
				(hash[++position] & 0xff) << 8 |
				(hash[++position] & 0xff);

			var text = digit.ToString();

			return text.Substring(text.Length - size);
		}

		private static Byte[] getBytes(String hexadecimal)
		{
			return hexadecimal.BreakByRegex(@"[\dA-F]{2}")
				.Select(getByte).ToArray();
		}

		private static Byte getByte(String text)
		{
			return Byte.Parse(text, NumberStyles.HexNumber);
		}

		private static String getHexTimestamp(Int32 add)
		{
			var now = DateTime.UtcNow;
			var start = new DateTime(1970, 1, 1);
			var timestamp = (now - start).TotalSeconds / 30;
			var integer = (Int32)Math.Floor(timestamp);

			return (integer + add).ToString("X").PadLeft(16, '0');
		}
	}
}
