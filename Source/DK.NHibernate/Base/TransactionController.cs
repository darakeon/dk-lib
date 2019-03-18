using System;
using System.Data;
using DK.Generic.Exceptions;
using DK.NHibernate.Helpers;
using NHibernate;

namespace DK.NHibernate.Base
{
    internal class TransactionController
    {
		protected static ISession Session => SessionManager.GetCurrent();

	    internal void Begin()
	    {
		    if (Session == null) return;

			if (Session.Transaction != null
                    && Session.Transaction.IsActive)
                throw new DKException("There's a Transaction opened already, cannot begin a new one.");

            Session.BeginTransaction();

            if (Session.Transaction == null
                    || !Session.Transaction.IsActive)
                throw new DKException("Transaction not opened.");

        }

        internal void Commit()
        {
	        if (Session == null) return;

            testTransaction("commit");

            Session.Transaction.Commit();

            Session.Flush();
        }

        internal void Rollback()
        {
	        if (Session == null) return;

			if (Session.Connection.State == ConnectionState.Closed)
			{
				Session.Connection.Open();
			}

			if (Session.Connection.State != ConnectionState.Closed)
			{
				if (Session.Transaction.IsActive)
				{
					testTransaction("rollback");
					Session.Transaction.Rollback();
				}
			}

			Session.Refresh();

			SessionManager.Failed = true;
        }

		private static void testTransaction(String action)
        {
			if (Session == null) return;

            if (Session.Transaction.WasCommitted || Session.Transaction.WasRolledBack)
                throw new DKException("There's a Transaction opened already, cannot " + action + ".");
        }
    }
}
