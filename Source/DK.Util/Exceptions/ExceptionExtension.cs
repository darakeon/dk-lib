using System;

namespace DK.Util.Exceptions
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
	        return exception.InnerException == null 
		        ? exception 
		        : exception.InnerException.MostInner();
        }
    }
}
