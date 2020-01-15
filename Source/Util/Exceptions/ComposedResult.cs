namespace Keon.Util.Exceptions
{
    /// <summary>
    /// Class to return an object composed by the return error and the success return
    /// </summary>
    /// <typeparam name="TS">Success return Type</typeparam>
    /// <typeparam name="TE">Error return Type</typeparam>
    public class ComposedResult<TS, TE>
        where TS : new()
        where TE : new()
    {
        /// <inheritdoc />
        /// <summary>
        /// Class to return an object composed by the return error and the success return
        /// </summary>
        public ComposedResult() : this(new TS(), new TE()) { }

        /// <inheritdoc />
        /// <summary>
        /// Class to return an object composed by the return error and the success return
        /// </summary>
        public ComposedResult(TS success) : this(success, new TE()) { }

        /// <inheritdoc />
        /// <summary>
        /// Class to return an object composed by the return error and the success return
        /// </summary>
        public ComposedResult(TE error) : this(new TS(), error) { }

        /// <summary>
        /// Class to return an object composed by the return error and the success return
        /// </summary>
        public ComposedResult(TS success, TE error)
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