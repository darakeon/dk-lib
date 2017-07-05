using System;

namespace DK.Generic.Exceptions
{
    ///<summary>
    /// Any exception thrown by DK library
    ///</summary>
    public class DKException : Exception
    {
        ///<summary>
        /// Any exception thrown by Ak library
        ///</summary>
        public DKException(String message) : base(message) { }

        /// <summary>
        /// Too Large Exception
        /// </summary>
        public static void TestOtherIfTooLarge(Exception e)
        {
            if (e.InnerException != null && e.InnerException.Message.StartsWith("Data too long for column"))
                throw new DKException("TooLargeData");

            throw e;
        }
    }
}
