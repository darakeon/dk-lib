using System;
using Keon.Util.Exceptions;
using Keon.Util.Extensions;
using Microsoft.AspNetCore.Http;

namespace Keon.MVC.Cookies
{
	/// <summary>
	/// Identification for Browser
	/// </summary>
	public class BrowserId
	{
		/// <summary>
		/// Get (if needed, create) ticket
		/// </summary>
		public static String Get(GetContext getContext)
		{
			return Get(getContext, false);
		}

		/// <summary>
		/// Get (if needed, create) ticket
		/// </summary>
		public static String Get(GetContext getContext, Boolean remember)
		{
			return new BrowserId(getContext).get(remember);
		}

		private const String name = "DFM";
		private readonly HttpContext context;

		private BrowserId(GetContext getContext)
		{
			context = getContext();
		}

		private String get(Boolean remember)
		{
			if (context == null)
				throw new DKException("No http context");

			if (String.IsNullOrEmpty(get()))
				add(Token.New(), remember);
			else if (remember)
				setCookie(get());

			return get();
		}

		private String get()
		{
			var session = context.Session;

			var value = session?.GetString(name);

			if (value != null)
				return value;

			var request = context.Request.Cookies;

			return request[name] ?? String.Empty;
		}

		private void add(String value, Boolean remember)
		{
			var session = context.Session;

			session?.SetString(name, value);

			if (remember)
				setCookie(value);
		}

		private void setCookie(string value)
		{
			context.Response.Cookies.Append(name, value);
		}
	}
}
