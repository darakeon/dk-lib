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
		/// Get (if needed, create) ticket
		/// </summary>
		public static String Get()
		{
			if (context == null)
			{
				throw new DKException("No http context");
			}

			if (get() == null)
				add(Token.New());

			return get();
		}



		private const String name = "DFM";

		private static HttpContext context => HttpContext.Current;
		private static HttpSessionState session => context.Session;


		private static String get()
		{
			var cookie = session?[name];

			if (cookie == null)
				remove();

			return cookie?.ToString();
		}



		private static void add(String value)
		{
			remove();
			session?.Add(name, value);
		}

		private static void remove()
		{
			session?.Remove(name);
		}


	}
}