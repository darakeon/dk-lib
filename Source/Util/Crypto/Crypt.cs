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

		private static readonly String key = File.ReadAllText("key.pk");
		private static readonly String iv = File.ReadAllText("iv.pk");

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
