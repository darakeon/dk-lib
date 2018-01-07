using System;
using System.Web.Mvc;

namespace DK.MVC.Authentication
{
    ///<summary>
    /// To Force error when the user is not logged-in
    ///</summary>
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        ///<summary>
        /// Message of expiration of session
        ///</summary>
        private const String message = "Session expired.";

	    /// <inheritdoc />
	    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result =
                new JsonResult
                {
                    Data = message,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
        }

    }
}