using System;
using DK.Generic.Exceptions;
using NHibernate;

namespace DK.NHibernate.Base
{
    internal class TransactionController
    {
        protected static ISession Session
        {
            get { return NHManager.Session; }
        }

        internal void Begin()
        {
            if (Session.Transaction != null
                    && Session.Transaction.IsActive)
                throw new AkException("There's a Transaction opened already, cannot begin a new one.");

            Session.BeginTransaction();

            if (Session.Transaction == null
                    || !Session.Transaction.IsActive)
                throw new AkException("Transaction not opened.");

        }

        internal void Commit()
        {
            testTransaction("commit");

            Session.Transaction.Commit();

            Session.Flush();
        }

        internal void Rollback()
        {
            if (Session.Transaction.IsActive)
            {
                testTransaction("rollback");
                Session.Transaction.Rollback();
            }

            Session.Clear();
        }

        private static void testTransaction(String action)
        {
            if (Session.Transaction.WasCommitted || Session.Transaction.WasRolledBack)
                throw new AkException("There's a Transaction opened already, cannot " + action + ".");
        }


    }
}
