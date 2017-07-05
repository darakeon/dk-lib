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

        public static void TestOtherIfTooLarge(Exception e)
        {
            if (e.InnerException != null && e.InnerException.Message.StartsWith("Data too long for column"))
                throw new AkException("TooLargeData");

            throw e;
        }
    }
}
