using System;
using System.Web;
using System.Web.SessionState;
using DK.Generic.Exceptions;
using DK.Generic.Extensions;

namespace DK.MVC.Cookies
{
	/// <summary>
	/// Identification for Browser
	/// </summary>
	public static class BrowserId
	{
		/// <summary>
		/// Time in minutes until session expire
		/// </summary>
		public static Int32 TimeoutMinutes = month;

		/// <summary>
		/// Get (if needed, create) ticket
		/// </summary>
		public static String Get()
		{
			if (context == null)
				throw new DKException("No http context");

			if (get() == null)
				add(Token.New());

			return get();
		}



		private const String name = "DFM";

		private const Int32 hour = 60;
		private const Int32 day = 24 * hour;
		private const Int32 month = 30 * day;

		private static HttpContext context => HttpContext.Current;
		private static HttpSessionState session => context.Session;


		private static String get()
		{
			return session?[name]?.ToString();
		}

		private static void add(String value)
		{
			if (session == null)
				return;

			session.Timeout = TimeoutMinutes;
			session.Add(name, value);
		}


	}
}