using System.Text;
using Keon.Util.Crypto;
using Org.BouncyCastle.Utilities.Encoders;

namespace Keon.Util.Tests
{
	public class CryptTest
	{
		[SetUp]
		public void Setup()
		{
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", null);
			Environment.SetEnvironmentVariable("KEON_CRYPT_IV", null);
		}

		[Test]
		public void EncryptDecryptKey16IV16()
		{
			var key = "ywdu36872648qhy3";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);
			var iv = "cwuahy4i734cin3n";
			iv = Base64.ToBase64String(Encoding.UTF8.GetBytes(iv));
			Environment.SetEnvironmentVariable("KEON_CRYPT_IV", iv);

			const String originalText = "Hey, listen!";

			var encrypted = Crypt.Encrypt(originalText);

			Assert.That(encrypted, Is.Not.EqualTo(originalText));

			var decrypted = Crypt.Decrypt(encrypted);

			Assert.That(decrypted, Is.EqualTo(originalText));
		}

		[Test]
		public void EncryptDecryptKey24IV16()
		{
			var key = "sdywdu36872648qhy3gjsiw8";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);
			var iv = "cwuahy4i734cin3n";
			iv = Base64.ToBase64String(Encoding.UTF8.GetBytes(iv));
			Environment.SetEnvironmentVariable("KEON_CRYPT_IV", iv);

			const String originalText = "Hey, listen!";

			var encrypted = Crypt.Encrypt(originalText);

			Assert.That(encrypted, Is.Not.EqualTo(originalText));

			var decrypted = Crypt.Decrypt(encrypted);

			Assert.That(decrypted, Is.EqualTo(originalText));
		}

		[Test]
		public void EncryptDecryptKey32IV16()
		{
			var key = "sdywdu36872648qhy3gjsiw8gakicme9";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);
			var iv = "cwuahy4i734cin3n";
			iv = Base64.ToBase64String(Encoding.UTF8.GetBytes(iv));
			Environment.SetEnvironmentVariable("KEON_CRYPT_IV", iv);

			const String originalText = "Hey, listen!";

			var encrypted = Crypt.Encrypt(originalText);

			Assert.That(encrypted, Is.Not.EqualTo(originalText));

			var decrypted = Crypt.Decrypt(encrypted);

			Assert.That(decrypted, Is.EqualTo(originalText));
		}

		[Test]
		public void EncryptDecryptKey16NoIV()
		{
			var key = "ywdu36872648qhy3";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);

			const String originalText = "Hey, listen!";

			var encrypted = Crypt.Encrypt(originalText);

			Assert.That(encrypted, Is.Not.EqualTo(originalText));

			var decrypted = Crypt.Decrypt(encrypted);

			Assert.That(decrypted, Is.EqualTo(originalText));
		}

		[Test]
		public void EncryptDecryptKey24NoIV()
		{
			var key = "sdywdu36872648qhy3gjsiw8";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);

			const String originalText = "Hey, listen!";

			var encrypted = Crypt.Encrypt(originalText);

			Assert.That(encrypted, Is.Not.EqualTo(originalText));

			var decrypted = Crypt.Decrypt(encrypted);

			Assert.That(decrypted, Is.EqualTo(originalText));
		}

		[Test]
		public void EncryptDecryptKey32NoIV()
		{
			var key = "sdywdu36872648qhy3gjsiw8gakicme9";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);

			const String originalText = "Hey, listen!";

			var encrypted = Crypt.Encrypt(originalText);

			Assert.That(encrypted, Is.Not.EqualTo(originalText));

			var decrypted = Crypt.Decrypt(encrypted);

			Assert.That(decrypted, Is.EqualTo(originalText));
		}

		[Test]
		public void EncryptDecryptKey48IV16()
		{
			var key = "ywdu36872648qhy3";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);
			var iv = "cwuahy4i734cin3njsuen263licysdfgmxi246kspxme8dlt";
			iv = Base64.ToBase64String(Encoding.UTF8.GetBytes(iv));
			Environment.SetEnvironmentVariable("KEON_CRYPT_IV", iv);

			const String originalText = "Hey, listen!";

			Assert.Throws(typeof(FormatException), () =>
			{
				Crypt.Encrypt(originalText);
			});

			Assert.Throws(typeof(FormatException), () =>
			{
				Crypt.Decrypt(originalText);
			});
		}

		[Test]
		public void EncryptDecryptKey16IV48()
		{
			var key = "ywdu36872648qhy3jsuen263licskjwqe343987ewrlkjdo8";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);
			var iv = "cwuahy4i734cin3n";
			iv = Base64.ToBase64String(Encoding.UTF8.GetBytes(iv));
			Environment.SetEnvironmentVariable("KEON_CRYPT_IV", iv);

			const String originalText = "Hey, listen!";

			Assert.Throws(typeof(FormatException), () =>
			{
				Crypt.Encrypt(originalText);
			});

			Assert.Throws(typeof(FormatException), () =>
			{
				Crypt.Decrypt(originalText);
			});
		}

		[Test]
		public void EncryptDecryptKeyNoBase64()
		{
			var key = "ywdu36&%$48qhy3d8";
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);
			var iv = "cwuahy4i734cin3n";
			iv = Base64.ToBase64String(Encoding.UTF8.GetBytes(iv));
			Environment.SetEnvironmentVariable("KEON_CRYPT_IV", iv);

			const String originalText = "Hey, listen!";

			Assert.Throws(typeof(FormatException), () =>
			{
				Crypt.Encrypt(originalText);
			});

			Assert.Throws(typeof(FormatException), () =>
			{
				Crypt.Decrypt(originalText);
			});
		}

		[Test]
		public void EncryptDecryptIVNoBase64()
		{
			var key = "ywdu36872648qhy3";
			key = Base64.ToBase64String(Encoding.UTF8.GetBytes(key));
			Environment.SetEnvironmentVariable("KEON_CRYPT_KEY", key);
			var iv = "cwuahy&#%34cin3n5";
			Environment.SetEnvironmentVariable("KEON_CRYPT_IV", iv);

			const String originalText = "Hey, listen!";

			Assert.Throws(typeof(FormatException), () =>
			{
				Crypt.Encrypt(originalText);
			});

			Assert.Throws(typeof(FormatException), () =>
			{
				Crypt.Decrypt(originalText);
			});
		}
	}
}
