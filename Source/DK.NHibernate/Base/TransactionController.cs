using System;
using System.Data;
using DK.NHibernate.Helpers;
using DK.Util.Exceptions;
using NHibernate;

namespace DK.NHibernate.Base
{
    internal class TransactionController : ITransactionController
	{
		private static ISession session => SessionManager.GetCurrent();

		public void Begin()
	    {
		    if (session == null) return;

			if (session.Transaction != null
                    && session.Transaction.IsActive)
                throw new DKException("There's a Transaction opened already, cannot begin a new one.");

            session.BeginTransaction();

            if (session.Transaction == null
                    || !session.Transaction.IsActive)
                throw new DKException("Transaction not opened.");

        }

		public void Commit()
        {
	        if (session == null) return;

            testTransaction("commit");

            session.Transaction.Commit();

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
				if (session.Transaction.IsActive)
				{
					testTransaction("rollback");
					session.Transaction.Rollback();
				}
			}

			session.Refresh();

			SessionManager.Failed = true;
        }

		private static void testTransaction(String action)
        {
			if (session == null) return;

            if (session.Transaction.WasCommitted || session.Transaction.WasRolledBack)
                throw new DKException("There's a Transaction opened already, cannot " + action + ".");
        }
    }
}
