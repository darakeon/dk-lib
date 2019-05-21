using System;
using System.Collections.Generic;
using System.Linq;
using DK.NHibernate.Base;
using DK.Util.DB;

namespace DK.NHibernate.Fakes
{
	class FakeTransaction : ITransactionController
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

		private static readonly IDictionary<String, IDbBackup> dbs
			= new Dictionary<String, IDbBackup>();

		private static IDictionary<String, IDictionary<Int32, IEntity>> backups;

		public static void AddDB(String name, IDbBackup db)
		{
			dbs.Add(name, db);
		}
	}
}
