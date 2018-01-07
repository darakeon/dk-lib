using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace DK.MVC.Authentication
{
    ///<summary>
    /// To make authentication in MVC easier
    ///</summary>
    public static class Authenticate
    {
        private static IPrincipal user
        {
            get => HttpContext.Current.User;
	        set => HttpContext.Current.User = value;
        }

        
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

            user = principal;

            var ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, DateTime.Now.AddDays(1), isPersistent, String.Join(",", arrRoles));
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
            if (request != null)
            {
                var authCookie =
                    new HttpCookie(FormsAuthentication.FormsCookieName) { Expires = DateTime.Now.AddMilliseconds(-1) };

                request.Cookies.Add(authCookie);
            }

            FormsAuthentication.SignOut();
        }



        ///<summary>
        /// Whether there is user logged-in at the site
        ///</summary>
        public static Boolean IsAuthenticated => user != null && user.Identity.IsAuthenticated;


	    ///<summary>
        /// Return the username that is logged-in
        ///</summary>
        public static String Username => IsAuthenticated ? user.Identity.Name : null;


	    /// <summary>
        /// Clean the session if the login is out on core, 
        /// but there is authentication on browser
        /// </summary>
        /// <param name="request">Request context</param>
        /// <returns>If the login was cleaned</returns>
        public static Boolean CleanIfDead(HttpRequestBase request)
        {
            if (!IsAuthenticated)
                Clean(request);

            return !IsAuthenticated;
        }



        ///<summary>
        /// Encrypt the password to write to the Database
        ///</summary>
        ///<param name="password"></param>
        ///<returns></returns>
        public static String EncryptPassword(this String password)
        {
			#pragma warning disable 618
            return FormsAuthentication
                .HashPasswordForStoringInConfigFile(
                    password,
                    FormsAuthPasswordFormat.SHA1.ToString()
                );
			#pragma warning restore 618
        }


	}
}