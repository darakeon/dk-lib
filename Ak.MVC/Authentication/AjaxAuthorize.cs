using System;
using System.Web.Mvc;

namespace Ak.MVC.Authentication
{
    ///<summary>
    /// To Force error when the user is not logged-in
    ///</summary>
    public class AjaxAuthorizeAttribute : AuthorizeAttribute
    {
        ///<summary>
        /// Message of expiration of session
        ///</summary>
        public const String Message = "Session expired.";

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result =
                new JsonResult
                {
                    Data = Message,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
        }

    }
}