using System;
using System.Linq;
using System.Text;
using BCrypt.Net;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
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

		/// <summary>
		/// Encrypt using a key and iv inside files key.pk and iv.pk
		/// </summary>
		public static String Encrypt(String text)
		{
			var cipher = createCipher(true);
			var bytes = Encoding.UTF8.GetBytes(text);
			var encryptedBytes = cipher.DoFinal(bytes);
			return Base64.ToBase64String(encryptedBytes);
		}

		/// <summary>
		/// Decrypt using a key and iv inside files key.pk and iv.pk
		/// </summary>
		public static String Decrypt(String text)
		{
			var cipher = createCipher(false);
			var bytes = Base64.Decode(text);
			var decryptedBytes = cipher.DoFinal(bytes);
			return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
		}

		private static IBufferedCipher createCipher(Boolean isEncryption)
		{
			var padding = new ISO10126d2Padding();
			var cipher = new PaddedBufferedBlockCipher(
				new CbcBlockCipher(new RijndaelEngine()),
				padding
			);
			
			var bytesKey = getBytes("KEON_CRYPT_KEY", 16, 24, 32);

			ICipherParameters parameters = new KeyParameter(bytesKey);

			var bytesIV = getBytes("KEON_CRYPT_IV", 16);

			if (bytesIV != null)
				parameters = new ParametersWithIV(parameters, bytesIV);

			cipher.Init(isEncryption, parameters);

			return cipher;
		}

		private static Byte[] getBytes(String varName, params Int32[] allowedSizes)
		{
			try
			{
				var text = Environment.GetEnvironmentVariable(varName);

				if (String.IsNullOrEmpty(text))
					return null;

				var bytes = Base64.Decode(text);

				if (!allowedSizes.Contains(bytes.Length))
					throw new FormatException($"{varName} allowed sizes: {allowedSizes}");

				return bytes;
			}
			catch (FormatException)
			{
				throw new FormatException($"{varName} must be encoded as Base64");
			}
		}
	}
}
