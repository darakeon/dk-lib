using System;
using System.Collections.Generic;
using System.Linq;
using Keon.NHibernate.Base;
using Keon.Util.DB;

namespace Keon.NHibernate.Fakes
{
	class FakeTransaction<I> : ITransactionController
		where I : struct
	{
		public void Begin()
		{
			backups = dbs
				.ToDictionary(
					e => e.Key,
					e => e.Value.Clone()
				);
		}

		public void Commit()
		{
			backups = null;
		}

		public void Rollback()
		{
			foreach (var entityName in backups.Keys)
			{
				var entityDb = dbs[entityName];
				var entityBk = backups[entityName];

				var allIds =
					entityBk.Keys
						.Union(entityDb.Keys)
						.Distinct();

				foreach (var entityId in allIds)
				{
					if (!entityBk.Keys.Contains(entityId))
						entityDb.Remove(entityId);
					else if (!entityDb.Keys.Contains(entityId))
						entityDb.Add(entityBk[entityId]);
					else
						entityDb.Replace(entityBk[entityId]);
				}
			}
		}

		private static readonly IDictionary<String, IDbBackup<I>> dbs
			= new Dictionary<String, IDbBackup<I>>();

		private static IDictionary<String, IDictionary<I, IEntity<I>>> backups;

		public static void AddDB(String name, IDbBackup<I> db)
		{
			dbs.Add(name, db);
		}
	}
}
