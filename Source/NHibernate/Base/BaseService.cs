using System;
using Keon.NHibernate.Fakes;

namespace Keon.NHibernate.Base
{
	/// <summary>
	/// Base for services, to have transaction
	/// </summary>
	/// <typeparam name="I">Integer ID type</typeparam>
    public class BaseService<I>
		where I: struct
    {
        /// <summary></summary>
        public BaseService()
        {
            transactionController = getTransactionController();
        }

	    private ITransactionController getTransactionController()
	    {
		    if (FakeHelper.IsFake)
			    return new FakeTransaction<I>();

		    return new TransactionController();
	    }

		private ITransactionController transactionController { get; }

		/// <summary>
		/// Execute commands inside a transaction.
		/// Anything executed without it will not work
		/// </summary>
		/// <param name="action">() => { return [your-code-here] }</param>
		/// <param name="onError">() => { [your-ON-ERROR-code-here] }</param>
		protected T InTransaction<T>(Func<T> action, Action onError = null)
		{
			transactionController.Begin();

			try
			{
				var result = action();

				transactionController.Commit();

				return result;
			}
			catch (Exception)
			{
				transactionController.Rollback();

				onError?.Invoke();

				throw;
			}
		}

		/// <summary>
		/// Execute commands inside a transaction.
		/// Anything executed without it will not work
		/// </summary>
		/// <param name="action">() => { [your-code-here] }</param>
		/// <param name="onError">() => { [your-ON-ERROR-code-here] }</param>
		protected void InTransaction(Action action, Action onError = null)
		{
			InTransaction(() =>
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
