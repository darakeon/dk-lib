using System;
using Keon.NHibernate.Queries;
using Keon.NHibernate.Sessions;
using Keon.Util.DB;
using NHibernate;

namespace Keon.NHibernate.Operations
{
	/// <summary>
	/// Base communication with DB
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	/// <typeparam name="ID">Integer ID type</typeparam>
	internal class BaseData<Entity, ID> : IData<Entity, ID>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
	{
		private ISession session => SessionManager.GetCurrent();

		public Entity SaveOrUpdate(
			Entity entity,
			params BaseRepository<Entity, ID>.DelegateAction[] actions
		)
		{
			foreach (var delegateAction in actions)
			{
				delegateAction(entity);
			}

			return saveOrUpdate(entity);
		}

		private Entity saveOrUpdate(Entity entity)
		{
			if (entity.ID.Equals(default(ID)) || session.Contains(entity))
				session.SaveOrUpdate(entity);
			else
				session.Merge(entity);

			return entity;
		}

		public Entity GetNonCached(ID id)
		{
			return SessionManager.GetNonCached<Entity, ID>(id);
		}

		public void Delete(Entity obj)
		{
			if (obj != null)
				session.Delete(obj);
		}

		public Entity GetById(ID id)
		{
			return session.Get<Entity>(id);
		}

		public IQuery<Entity, ID> NewQuery()
		{
			return getQuery(session);
		}

		public TResult NewNonCachedQuery<TResult>(Func<IQuery<Entity, ID>, TResult> action)
		{
			TResult result;
			using (var otherSession = SessionManager.GetNonCached())
			{
				var query = getQuery(otherSession);
				result = action(query);
				otherSession.Close();
				otherSession.Dispose();
			}

			return result;
		}

		private static IQuery<Entity, ID> getQuery(ISession session)
		{
			return new Query<Entity, ID>(session);
		}
	}
}
