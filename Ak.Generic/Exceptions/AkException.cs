using System;

namespace Ak.Generic.Exceptions
{
    ///<summary>
    /// Any exception thrown by Ak library
    ///</summary>
    public class AkException : Exception
    {
        ///<summary>
        /// Any exception thrown by Ak library
        ///</summary>
        public AkException(String message) : base(message) { }

    }
}
