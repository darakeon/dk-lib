using System;
using Keon.MVC.Cookies;
using Microsoft.AspNetCore.Routing;

namespace Keon.MVC.Route
{
	///<summary>
	/// Retrieves the route data as the property of the controllers
	///</summary>
	public class RouteInfo
	{
		/// <summary>
		///  RouteData of the current Url
		/// </summary>
		public RouteInfo(GetContext getContext)
		{
			RouteData = getContext().GetRouteData();
		}


		///<summary>
		/// Encapsulates information about the Url
		///</summary>
		public RouteData RouteData { get; }

		/// <summary>
		/// Values dictionary
		/// </summary>
		public String this[String key] => RouteData?.Values?[key]?.ToString();
	}
}
