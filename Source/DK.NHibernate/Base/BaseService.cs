using System;
using DK.Generic.Exceptions;

namespace DK.NHibernate.Base
{
    /// <summary>
    /// Base for services, to have transaction
    /// </summary>
    public class BaseService
    {
        /// <summary></summary>
        public BaseService()
        {
            transactionController = new TransactionController();
        }

        private TransactionController transactionController { get; }

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

				commitTransaction();

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
			transactionController.Begin();

			try
			{
				action();

				commitTransaction();
			}
			catch (Exception)
			{
				transactionController.Rollback();

				onError?.Invoke();

				throw;
			}
		}

		private void commitTransaction()
		{
			try
			{
				transactionController.Commit();
			}
			catch (Exception e)
			{
				DKException.TestOtherIfTooLarge(e);
			}
		}




    }
}
