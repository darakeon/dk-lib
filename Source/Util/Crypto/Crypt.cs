using System;
using System.IO;
using BCrypt.Net;
using crypto;
using crypt = BCrypt.Net.BCrypt;

namespace Keon.Util.Crypto
{
	/// <summary>
	/// Helper for cryptography
	/// </summary>
	public static class Crypt
	{
		/// <summary>
		/// Make a hash
		/// </summary>
		public static String Do(String text)
		{
			return crypt.HashPassword(text);
		}

		/// <summary>
		/// Check against a hash
		/// </summary>
		public static Boolean Check(String text, String hash)
		{
			try
			{
				return crypt.Verify(text, hash);
			}
			catch (SaltParseException)
			{
				return false;
			}
		}

		private static String key => Environment.GetEnvironmentVariable("KEON_CRYPT_KEY");
		private static String iv => Environment.GetEnvironmentVariable("KEON_CRYPT_IV");

		/// <summary>
		/// Encrypt using a key and iv inside files key.pk and iv.pk
		/// </summary>
		public static String Encrypt(String text)
		{
			return Security.Encrypt(text, key, iv);
		}

		/// <summary>
		/// Decrypt using a key and iv inside files key.pk and iv.pk
		/// </summary>
		public static String Decrypt(String text)
		{
			return Security.Decrypt(text, key, iv);
		}
	}
}
