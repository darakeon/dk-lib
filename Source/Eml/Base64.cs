using System;
using System.Text;

namespace Keon.Eml
{
	/// <summary>
	/// Base64 Helper
	/// </summary>
	public static class Base64
	{
		/// <summary>
		/// Convert Base64 text into human readable text
		/// </summary>
		/// <param name="text">Text in Base64 format</param>
		/// <returns>Decoded text</returns>
		public static String FromBase64(this String text)
		{
			var bytes = Convert.FromBase64String(text);
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
