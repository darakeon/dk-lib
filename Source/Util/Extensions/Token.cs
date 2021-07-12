using System;

namespace Keon.Util.Extensions
{
	/// <summary>
	/// String Guid
	/// </summary>
	public static class Token
	{
		/// <summary>
		/// Generates new Guid
		/// </summary>
		/// <returns>Guid without dashes ("-")</returns>
		public static String New()
		{
			return Guid.NewGuid()
				.ToString()
				.ToUpper()
				.Replace("-", "");
		}

	}
}
