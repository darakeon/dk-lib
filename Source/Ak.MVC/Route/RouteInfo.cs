using System;
using System.Web;
using System.Web.Routing;

namespace Ak.MVC.Route
{
    ///<summary>
    /// Retrieves the routedata as the property of the controllers
    ///</summary>
    public class RouteInfo
    {
        ///<summary>
        /// RouteData of the current Url
        ///</summary>
        public RouteInfo() : this(HttpContext.Current.Request.Url) { }


        ///<summary>
        /// RouteData of a specific Url
        ///</summary>
        public RouteInfo(Uri uri) : this(uri, null) { }


        ///<summary>
        /// RouteData of a specific Url, cutting the subfolder
        ///</summary>
        public RouteInfo(Uri uri, String applicationPath)
        {
            RouteData = RouteTable.Routes.GetRouteData(new InternalHttpContext(uri, applicationPath));
        }


        ///<summary>
        /// Encapsulates information about the Url
        ///</summary>
        public RouteData RouteData { get; private set; }


        ///<summary>
        /// RouteData of the current Url
        ///</summary>
        public static RouteInfo Current
        {
            get
            {
                return new RouteInfo();
            }
        }


        private class InternalHttpContext : HttpContextBase
        {
            private readonly HttpRequestBase request;

            public InternalHttpContext(Uri uri, String applicationPath)
            {
                request = new InternalRequestContext(uri, applicationPath);
            }

            public override HttpRequestBase Request { get { return request; } }
        }

        private class InternalRequestContext : HttpRequestBase
        {
            private readonly String appRelativePath;
            private readonly String pathInfo;

            public InternalRequestContext(Uri uri, String applicationPath)
            {
                pathInfo = uri.Query;

                
                var noApplicationPath = String.IsNullOrEmpty(applicationPath);
                
                var wrongApplicationPath =
                    !uri.AbsolutePath.StartsWith(applicationPath ?? String.Empty, StringComparison.OrdinalIgnoreCase);


                appRelativePath = noApplicationPath || wrongApplicationPath
                    ? uri.AbsolutePath
                    : uri.AbsolutePath.Substring(applicationPath.Length);
            }

            public override String AppRelativeCurrentExecutionFilePath { get { return String.Concat("~", appRelativePath); } }
            public override String PathInfo { get { return pathInfo; } }
        }
    }
}