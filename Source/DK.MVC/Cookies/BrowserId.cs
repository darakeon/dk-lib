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
			return Get(null);
		}

		/// <summary>
		/// Get (if needed, create) ticket
		/// </summary>
		public static String Get(Boolean? remember)
		{
			if (context == null)
				throw new DKException("No http context");

			if (get() == null)
			{
				add(Token.New(), remember);
			}
			else if (remember.HasValue)
			{
				setCookie(get(), remember.Value);
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
			if (session != null)
			{
				var value = session[name];

				if (value != null)
					return value.ToString();
			}

			var cookie = getCookies[name] ?? setCookies[name];
			var cookieText = cookie?.Value ?? String.Empty;

			if (!cookieText.EndsWith("_1"))
				return null;

			var ticket = cookieText.Substring(0, cookieText.Length - 2);

			return ticket;
		}

		private static void add(String value, Boolean? remember)
		{
			if (session != null)
			{
				session.Timeout = month;
				session.Add(name, value);
			}

			if (remember.HasValue)
			{
				setCookie(value, remember.Value);
			}
		}

		private static void setCookie(String value, Boolean remember)
		{
			var cookie = new HttpCookie(name, value + "_" + (remember ? 1 : 0));

			getCookies.Add(cookie);
			setCookies.Add(cookie);
		}
	}
}