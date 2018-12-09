using System;
using System.Web;
using System.Web.Routing;

namespace DK.MVC.Route
{
	///<summary>
	/// Retrieves the routedata as the property of the controllers
	///</summary>
	public class RouteInfo
	{
		/// <inheritdoc />
		/// <summary>
		///  RouteData of the current Url
		/// </summary>
		public RouteInfo() : this(HttpContext.Current.Request.Url) { }


		/// <inheritdoc />
		/// <summary>
		///  RouteData of a specific Url
		/// </summary>
		public RouteInfo(Uri uri) : this(uri, null) { }


		///<summary>
		/// RouteData of a specific Url, cutting the subfolder
		///</summary>
		public RouteInfo(Uri uri, String applicationPath)
		{
			var context = new internalHttpContext(uri, applicationPath);
			RouteData = RouteTable.Routes.GetRouteData(context);
		}


		///<summary>
		/// Encapsulates information about the Url
		///</summary>
		public RouteData RouteData { get; }


		///<summary>
		/// RouteData of the current Url
		///</summary>
		public static RouteInfo Current => new RouteInfo();


		/// <summary>
		/// Values dictionary
		/// </summary>
		public String this[String key] => RouteData?.Values?[key]?.ToString();


		private sealed class internalHttpContext : HttpContextBase
		{
			public internalHttpContext(Uri uri, String applicationPath)
			{
				Request = new internalRequestContext(uri, applicationPath);
			}

			public override HttpRequestBase Request { get; }
		}

		private sealed class internalRequestContext : HttpRequestBase
		{
			private readonly String appRelativePath;

			public internalRequestContext(Uri uri, String applicationPath)
			{
				PathInfo = uri.Query;


				var noApplicationPath = String.IsNullOrEmpty(applicationPath);

				var wrongApplicationPath =
					!uri.AbsolutePath.StartsWith(applicationPath ?? String.Empty, StringComparison.OrdinalIgnoreCase);


				appRelativePath = noApplicationPath || wrongApplicationPath
					? uri.AbsolutePath
					: uri.AbsolutePath.Substring(applicationPath.Length);
			}

			public override String AppRelativeCurrentExecutionFilePath => String.Concat("~", appRelativePath);
			public override String PathInfo { get; }
		}
	}
}