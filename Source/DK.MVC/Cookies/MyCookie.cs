using System;
using System.Web;
using DK.Generic.Exceptions;
using DK.Generic.Extensions;

namespace DK.MVC.Cookies
{
	/// <summary>
	/// "Cookie" for browser, cellphone or local
	/// </summary>
	public static class MyCookie
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
		private static HttpCookieCollection requestCookies => context.Request.Cookies;
		private static HttpCookieCollection responseCookies => context.Response.Cookies;


		private static String get()
		{
			var cookie = requestCookies[name]
			             ?? responseCookies[name];

			if (cookie == null)
				return null;

			if (cookie.Value == null)
				remove();

			return cookie.Value;
		}



		private static void add(String value)
		{
			remove();

			var cookie = new HttpCookie(name)
			{
				Value = value,
				Expires = DateTime.UtcNow.AddDays(7)
			};

			requestCookies.Add(cookie);
			responseCookies.Add(cookie);

			// ReSharper disable PossibleNullReferenceException
			requestCookies[name].Value = value;
			responseCookies[name].Value = value;
			// ReSharper enable PossibleNullReferenceException
		}

		private static void remove()
		{
			if (requestCookies[name] != null)
				requestCookies[name].Expires =
					DateTime.UtcNow.AddDays(-1);

			if (responseCookies[name] != null)
				responseCookies[name].Expires =
					DateTime.UtcNow.AddDays(-1);
		}


	}
}