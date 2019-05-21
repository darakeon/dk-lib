using System;
using System.Collections.Generic;
using System.Linq;
using DK.NHibernate.Base;
using DK.NHibernate.Queries;
using DK.Util.DB;

namespace DK.NHibernate.Fakes
{
	class FakeData<T> : IData<T>, IDbBackup
		where T : class, IEntity, new()
	{
		public FakeData()
		{
			FakeTransaction.AddDB(typeof(T).Name, this);
		}

		private static readonly IDictionary<Int32, T> db = new Dictionary<Int32, T>();

		public T SaveOrUpdate(T entity, params BaseRepository<T>.DelegateAction[] actions)
		{
			actions.ToList()
				.ForEach(a => a(entity));

			FakeHelper.TestSizes(entity);

			if (entity.ID == 0)
			{
				var id = db.Keys.Any() 
					? db.Keys.Max() + 1
					: 1;

				entity.ID = id;

				db.Add(id, entity);
			}
			else
			{
				db[entity.ID] = entity;
			}

			return entity;
		}

		public T GetNonCached(Int32 id)
		{
			return GetById(id);
		}

		public void Delete(T obj)
		{
			if (db.ContainsKey(obj.ID))
				db.Remove(obj.ID);
		}

		public T GetById(int id)
		{
			return db.ContainsKey(id) ? db[id] : null;
		}

		public IQuery<T> NewQuery()
		{
			return new FakeQuery<T>(db);
		}

		public TResult NewNonCachedQuery<TResult>(Func<IQuery<T>, TResult> action)
		{
			return action(NewQuery());
		}



		public ICollection<Int32> Keys => db.Keys;

		public void Remove(Int32 entityId)
		{
			db.Remove(entityId);
		}

		public void Add(IEntity entity)
		{
			db.Add(entity.ID, (T)entity);
		}

		public void Replace(IEntity entity)
		{
			db[entity.ID] = (T)entity;
		}

		public IDictionary<Int32, IEntity> Clone()
		{
			return db.Select(e => e.Value)
				.Cast<IEntity>()
				.ToDictionary(
					e => e.ID,
					e => e
				);
		}
	}
}
