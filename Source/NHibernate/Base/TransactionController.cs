using System;
using System.Data;
using Keon.NHibernate.Helpers;
using Keon.Util.Exceptions;
using NHibernate;

namespace Keon.NHibernate.Base
{
    internal class TransactionController : ITransactionController
	{
		private static ISession session => SessionManager.GetCurrent();
		private ITransaction transaction;

		public void Begin()
	    {
		    if (session == null) return;

			if (transaction != null
                    && transaction.IsActive)
                throw new DKException("There's a Transaction opened already, cannot begin a new one.");

            transaction = session.BeginTransaction();

            if (transaction == null
                    || !transaction.IsActive)
                throw new DKException("Transaction not opened.");

        }

		public void Commit()
        {
	        if (session == null) return;

            testTransaction("commit");

            transaction.Commit();

            session.Flush();
        }

		public void Rollback()
        {
	        if (session == null) return;

			if (session.Connection.State == ConnectionState.Closed)
			{
				session.Connection.Open();
			}

			if (session.Connection.State != ConnectionState.Closed)
			{
				if (transaction.IsActive)
				{
					testTransaction("rollback");
					transaction.Rollback();
				}
			}

			session.Refresh();

			SessionManager.Failed = true;
        }

		private void testTransaction(String action)
        {
			if (session == null) return;

            if (transaction.WasCommitted || transaction.WasRolledBack)
                throw new DKException($"There's a Transaction opened already, cannot {action}.");
        }
    }
}
