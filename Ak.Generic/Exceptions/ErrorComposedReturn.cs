namespace Ak.Generic.Exceptions
{
    /// <summary>
    /// Class to return an object composed by the return error and the success return
    /// </summary>
    /// <typeparam name="TS">Success return Type</typeparam>
    /// <typeparam name="TE">Error return Type</typeparam>
    public class ErrorComposedReturn<TS, TE>
        where TS : new()
        where TE : new()
    {
        /// <summary>
        /// Class to return an object composed by the return error and the success return
        /// </summary>
        public ErrorComposedReturn() : this(new TS(), new TE()) { }

        /// <summary>
        /// Class to return an object composed by the return error and the success return
        /// </summary>
        public ErrorComposedReturn(TS success) : this(success, new TE()) { }

        /// <summary>
        /// Class to return an object composed by the return error and the success return
        /// </summary>
        public ErrorComposedReturn(TE error) : this(new TS(), error) { }

        /// <summary>
        /// Class to return an object composed by the return error and the success return
        /// </summary>
        public ErrorComposedReturn(TS success, TE error)
        {
            Success = success;
            Error = error;
        }

        ///<summary>
        ///</summary>
        public TS Success;

        ///<summary>
        ///</summary>
        public TE Error;
    }
}