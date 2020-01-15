using System;
using System.Web;
using System.Web.SessionState;
using Keon.Util.Exceptions;
using Keon.Util.Extensions;

namespace Keon.MVC.Cookies
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
			return Get(false);
		}

		/// <summary>
		/// Get (if needed, create) ticket
		/// </summary>
		public static String Get(Boolean remember)
		{
			if (context == null)
				throw new DKException("No http context");

			if (String.IsNullOrEmpty(get()))
			{
				add(Token.New(), remember);
			}
			else if (remember)
			{
				setCookie(get());
			}

			return get();
		}



		private const String name = "DFM";

		private const Int32 hour = 60;
		private const Int32 day = 24 * hour;
		private const Int32 month = 30 * day;

		private static HttpContext context => HttpContext.Current;
		private static HttpSessionState session => context.Session;
		private static HttpCookieCollection getCookies => context.Request.Cookies;
		private static HttpCookieCollection setCookies => context.Response.Cookies;


		private static String get()
		{
			var value = session?[name];

			if (value != null)
				return value.ToString();

			var cookie = getCookies[name] ?? setCookies[name];
			return cookie?.Value ?? String.Empty;
		}

		private static void add(String value, Boolean remember)
		{
			if (session != null)
			{
				session.Timeout = month;
				session.Add(name, value);
			}

			if (remember)
			{
				setCookie(value);
			}
		}

		private static void setCookie(String value)
		{
			var cookie = new HttpCookie(name, value);

			getCookies.Add(cookie);
			setCookies.Add(cookie);
		}
	}
}
