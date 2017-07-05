using System;
using System.Web;
using System.Web.Mvc;
using Ak.Generic.Exceptions;

namespace Ak.MVC.Authentication
{
    ///<summary>
    /// To Force error when the user is not logged-in
    ///</summary>
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        ///<summary>
        /// The message to return as exception
        ///</summary>
        public String Message { get; set; }

        /// <summary>
        /// When overridden, provides an entry point for custom authorization checks.
        /// </summary>
        /// <param name="httpContext">The HTTP context, which encapsulates all HTTP-specific information about an individual HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="httpContext"/> parameter is null.</exception>
        /// <returns>Whether the user is authorized</returns>
        protected override Boolean AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
                return true;

            if (String.IsNullOrEmpty(Message))
                Message = "Session expired.";

            throw new AkException(Message);
        }
    }
}