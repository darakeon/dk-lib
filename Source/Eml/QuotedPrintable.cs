using System;

namespace Keon.Eml
{
	/// <summary>
	/// QuotedPrintable Helper
	/// </summary>
	public static class QuotedPrintable
	{
		/// <summary>
		/// Convert Base64 text into human readable text
		/// </summary>
		/// <param name="text">Text in Base64 format</param>
		/// <returns>Decoded text</returns>
		public static String FromQuotedPrintable(this String text)
		{
			return text
				.Replace("\"\"", "\"")
				.Replace("=\n", "")
				.Replace("=3D", "=")
				.Replace("=0A", "\n");
		}

		/// <summary>
		/// Convert Base64 text into human readable text
		/// </summary>
		/// <param name="lines">Text in Base64 format</param>
		/// <returns>Decoded text</returns>
		public static String[] FromQuotedPrintable(this String[] lines)
		{
			return String.Join("\n", lines)
				.FromQuotedPrintable()
				.Split("\n");
		}
	}
}
