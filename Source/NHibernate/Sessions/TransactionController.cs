using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Keon.Util.Exceptions;
using NHibernate;

namespace Keon.NHibernate.Sessions
{
	internal class TransactionController
	{
		public TransactionController(String caller)
		{
			this.caller = caller;
		}

		private String caller { get; }
		private State state { get; set; }
		private Boolean completed => 
			state == State.EndCommit || state == State.EndRollback;

		private static ISession session => SessionManager.GetCurrent();
		private ITransaction transaction;

		private static readonly IList<TransactionController> states =
			new List<TransactionController>();

		public void Begin()
		{
			if (session == null) return;

			transaction = session.GetCurrentTransaction();

			if (transaction != null)
			{
				var whoIsUsing = states.Where(s => !s.completed)
					.Select(s => s.toString(transaction));

				var pending = String.Join("/", whoIsUsing);

				throw new DKException(
					$"[{caller}] There's a Transaction opened already," +
					" cannot begin a new one." +
					$" Pending: {pending}"
				);
			}

			states.Add(this);

			state = State.StartBegin;

			transaction = session.BeginTransaction();

			if (transaction == null || !transaction.IsActive)
				throw new DKException("Transaction not opened.");

			state = State.EndBegin;
		}

		public void Commit()
		{
			if (session == null) return;

			state = State.StartCommit;

			testTransaction("commit");

			transaction.Commit();
			session.Flush();

			state = State.EndCommit;
		}

		public void Rollback()
		{
			if (session == null) return;

			state = State.StartRollback;

			if (session.Connection.State == ConnectionState.Closed)
			{
				session.Connection.Open();
			}

			if (session.Connection.State != ConnectionState.Closed)
			{
				testTransaction("rollback");
				transaction.Rollback();
			}

			session.Clear();

			SessionManager.AddFailed(session);

			state = State.EndRollback;
		}

		private void testTransaction(String action)
		{
			if (session == null) return;

			if (transaction.WasCommitted || transaction.WasRolledBack)
				throw new DKException($"There's a Transaction opened already, cannot {action}.");
		}

		private String toString(ITransaction other)
		{
			var marker = transaction == other ? "*" : "";

			return $"{caller}{marker} [{state}]";
		}

		enum State
		{
			StartBegin,
			EndBegin,
			StartCommit,
			EndCommit,
			StartRollback,
			EndRollback
		}
	}
}
