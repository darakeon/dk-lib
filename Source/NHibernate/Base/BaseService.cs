using System;

namespace Keon.NHibernate.Base
{
	/// <summary>
	/// Base for services, to have transaction
	/// </summary>
	/// <typeparam name="I">Integer ID type</typeparam>
    public class BaseService<I>
		where I: struct
    {
	    private ITransactionController getTransactionController(String caller)
	    {
		    return new TransactionController(caller);
	    }

		/// <summary>
		/// Execute commands inside a transaction.
		/// Anything executed without it will not work
		/// </summary>
		/// <param name="caller">Caller name, to print states in case of errors</param>
		/// <param name="action">() => { return [your-code-here] }</param>
		/// <param name="onError">() => { [your-ON-ERROR-code-here] }</param>
		protected T inTransaction<T>(String caller, Func<T> action, Action onError = null)
		{
			var controller = getTransactionController(caller);

			controller.Begin();

			try
			{
				var result = action();

				controller.Commit();

				return result;
			}
			catch (Exception)
			{
				controller.Rollback();

				onError?.Invoke();

				throw;
			}
		}

		/// <summary>
		/// Execute commands inside a transaction.
		/// Anything executed without it will not work
		/// </summary>
		/// <param name="caller">Caller name, to print states in case of errors</param>
		/// <param name="action">() => { [your-code-here] }</param>
		/// <param name="onError">() => { [your-ON-ERROR-code-here] }</param>
		protected void inTransaction(String caller, Action action, Action onError = null)
		{
			inTransaction(caller, () =>
			{
				action();
				return 0;
			}, onError);
		}
    }

	/// <inheritdoc />
	public class BaseService : BaseService<Int32> { }

	/// <inheritdoc />
	public class BaseServiceShort : BaseService<Int16> { }

	/// <inheritdoc />
	public class BaseServiceLong : BaseService<Int64> { }
}
