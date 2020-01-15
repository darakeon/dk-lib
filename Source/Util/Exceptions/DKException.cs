using System;

namespace Keon.Util.Exceptions
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
    }
}
