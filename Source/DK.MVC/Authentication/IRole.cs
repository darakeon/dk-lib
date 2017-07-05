using System;

namespace DK.MVC.Authentication
{
    ///<summary>
    /// The Interface to paas Roles (permissions) to Authenticate
    ///</summary>
    public interface IRole
    {
        ///<summary>
        /// The Name of Permission that will be verified by Authorize decorator (MVC)
        ///</summary>
        String Name { get; set; }
    }
}
