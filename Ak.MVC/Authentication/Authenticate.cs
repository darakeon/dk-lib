using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;

namespace Ak.MVC.Authentication
{
    ///<summary>
    /// To make authentication in MVC easier
    ///</summary>
    public static class Authenticate
    {
        ///<summary>
        /// Authenticate the User
        ///</summary>
        ///<param name="username">the Username or whatever unique String to autheticate the logged-in individual</param>
        ///<param name="response">Current Response of the Controller</param>
        ///<param name="isPersistent">Whether, when browser reopen, the login will be keeped</param>
        public static void Set(String username, HttpResponseBase response, Boolean isPersistent = false)
        {
            Set(username, response, (IList<IRole>)null, isPersistent);
        }

        ///<summary>
        /// Authenticate the User
        ///</summary>
        ///<param name="username">the Username or whatever unique String to autheticate the logged-in individual</param>
        ///<param name="response">Current Response of the Controller</param>
        ///<param name="role">The permission of the individual</param>
        ///<param name="isPersistent">Whether, when browser reopen, the login will be keeped</param>
        public static void Set(String username, HttpResponseBase response, IRole role, Boolean isPersistent = false)
        {
            var roleList = new List<IRole> { role };

            Set(username, response, roleList, isPersistent);
        }

        ///<summary>
        /// Authenticate the User
        ///</summary>
        ///<param name="username">the Username or whatever unique String to autheticate the logged-in individual</param>
        ///<param name="response">Current Response of the Controller</param>
        ///<param name="roleList">The list of permissions of the individual</param>
        ///<param name="isPersistent">Whether, when browser reopen, the login will be keeped</param>
        public static void Set(String username, HttpResponseBase response, IList<IRole> roleList, Boolean isPersistent = false)
        {
            if (roleList == null)
                roleList = new List<IRole>();

            var arrRoles = roleList
                    .Select(r => r.Name)
                    .ToArray();

            var identity = new GenericIdentity(username);
            var principal = new GenericPrincipal(identity, arrRoles);

            HttpContext.Current.User = principal;

            var ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddHours(2), isPersistent, String.Join(",", arrRoles));
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            response.Cookies.Add(authCookie);
        }

        ///<summary>
        /// Log-out the individual, cleaning the cookies
        ///</summary>
        ///<param name="request">Current Request of the Controller</param>
        public static void Clean(HttpRequestBase request)
        {
            FormsAuthentication.SignOut();

            if (request != null)
            {
                request.Cookies.Clear();
            }
        }



        ///<summary>
        /// Whether there is user logged-in at the site
        ///</summary>
        public static Boolean IsAuthenticated()
        {
            return HttpContext.Current.User != null
                && HttpContext.Current.User.Identity.IsAuthenticated;
        }



        ///<summary>
        /// Return the username that is logged-in
        ///</summary>
        public static String GetUsername()
        {
            return IsAuthenticated()
                ? HttpContext.Current.User.Identity.Name
                : null;
        }



        ///<summary>
        /// Encrypt the password to write to the Database
        ///</summary>
        ///<param name="password"></param>
        ///<returns></returns>
        public static String EncryptPassword(this String password)
        {
            return FormsAuthentication
                .HashPasswordForStoringInConfigFile(
                    password,
                    FormsAuthPasswordFormat.SHA1.ToString()
                );
        }
    }
}