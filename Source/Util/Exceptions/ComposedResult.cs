namespace Keon.Util.Exceptions
{
	/// <summary>
	/// Class to return an object composed by the return error and the success return
	/// </summary>
	/// <typeparam name="S">Success return Type</typeparam>
	/// <typeparam name="E">Error return Type</typeparam>
	public class ComposedResult<S, E>
		where S : new()
		where E : new()
	{
		/// <inheritdoc />
		/// <summary>
		/// Class to return an object composed by the return error and the success return
		/// </summary>
		public ComposedResult() : this(new S(), new E()) { }

		/// <inheritdoc />
		/// <summary>
		/// Class to return an object composed by the return error and the success return
		/// </summary>
		public ComposedResult(S success) : this(success, new E()) { }

		/// <inheritdoc />
		/// <summary>
		/// Class to return an object composed by the return error and the success return
		/// </summary>
		public ComposedResult(E error) : this(new S(), error) { }

		/// <summary>
		/// Class to return an object composed by the return error and the success return
		/// </summary>
		public ComposedResult(S success, E error)
		{
			Success = success;
			Error = error;
		}

		///<summary>
		///</summary>
		public S Success;

		///<summary>
		///</summary>
		public E Error;
	}
}
