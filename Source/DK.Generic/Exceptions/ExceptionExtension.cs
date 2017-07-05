using System;

namespace DK.Generic.Exceptions
{
    ///<summary>
    /// Extension for Exception Base Class
    ///</summary>
    public static class ExceptionExtension
    {
        ///<summary>
        /// Get the most InnerException
        ///</summary>
        public static Exception MostInner(this Exception exception)
        {
            if (exception.InnerException == null)
                return exception;

            return exception.InnerException.MostInner();
        }
    }
}
