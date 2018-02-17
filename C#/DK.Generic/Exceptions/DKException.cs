using System;

namespace DK.Generic.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///  Any exception thrown by DK library
    /// </summary>
    public class DKException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///  Any exception thrown by DK library
        /// </summary>
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
