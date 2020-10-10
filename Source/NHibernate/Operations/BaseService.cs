using System;
using Keon.NHibernate.Sessions;

namespace Keon.NHibernate.Operations
{
	/// <summary>
	/// Base for services, to have transaction
	/// </summary>
    public class BaseService
    {
		/// <summary>
		/// Execute commands inside a transaction.
		/// Anything executed without it will not work
		/// </summary>
		/// <param name="caller">Caller name, to print states in case of errors</param>
		/// <param name="action">() => { return [your-code-here] }</param>
		/// <param name="onError">() => { [your-ON-ERROR-code-here] }</param>
		protected Result inTransaction<Result>(String caller, Func<Result> action, Action onError = null)
		{
			var controller = new TransactionController(caller);

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
}
