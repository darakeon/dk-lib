using System;
using System.Collections.Generic;
using System.Linq;
using Keon.NHibernate.Base;
using Keon.NHibernate.Queries;
using Keon.Util.DB;

namespace Keon.NHibernate.Fakes
{
	class FakeData<T, I> : IData<T, I>, IDbBackup<I>
		where T : class, IEntity<I>, new()
		where I : struct
	{
		private Func<I, I> next { get; }

		public FakeData(Func<I, I> next)
		{
			this.next = next;
			FakeTransaction<I>.AddDB(typeof(T).Name, this);
		}

		private static readonly IDictionary<I, T> db = new Dictionary<I, T>();

		public T SaveOrUpdate(T entity, params BaseRepository<T, I>.DelegateAction[] actions)
		{
			actions.ToList()
				.ForEach(action => action(entity));

			if (entity.ID.Equals(default(I)))
			{
				var id = db.Keys.Any() 
					? db.Keys.Max()
					: default;

				entity.ID = next(id);

				db.Add(id, entity);
			}
			else
			{
				db[entity.ID] = entity;
			}

			return entity;
		}

		public T GetNonCached(I id)
		{
			return GetById(id);
		}

		public void Delete(T obj)
		{
			if (db.ContainsKey(obj.ID))
				db.Remove(obj.ID);
		}

		public T GetById(I id)
		{
			return db.ContainsKey(id) ? db[id] : null;
		}

		public IQuery<T, I> NewQuery()
		{
			return new FakeQuery<T, I>(db);
		}

		public TResult NewNonCachedQuery<TResult>(Func<IQuery<T, I>, TResult> action)
		{
			return action(NewQuery());
		}



		public ICollection<I> Keys => db.Keys;

		public void Remove(I entityId)
		{
			db.Remove(entityId);
		}

		public void Add(IEntity<I> entity)
		{
			db.Add(entity.ID, (T)entity);
		}

		public void Replace(IEntity<I> entity)
		{
			db[entity.ID] = (T)entity;
		}

		public IDictionary<I, IEntity<I>> Clone()
		{
			return db.Select(e => e.Value)
				.Cast<IEntity<I>>()
				.ToDictionary(
					e => e.ID,
					e => e
				);
		}
	}
}
