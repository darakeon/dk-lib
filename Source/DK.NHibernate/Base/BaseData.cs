using System;
using Keon.NHibernate.Queries;
using Keon.Util.DB;
using NHibernate;

namespace Keon.NHibernate.Base
{
	/// <summary>
	/// Base communication with DB
	/// </summary>
	/// <typeparam name="T">Main entity</typeparam>
	/// <typeparam name="I">Integer ID type</typeparam>
	internal class BaseData<T, I> : IData<T, I>
        where T : class, IEntity<I>, new()
		where I : struct
	{
        public T SaveOrUpdate(T entity, params BaseRepository<T, I>.DelegateAction[] actions)
        {
            foreach (var delegateAction in actions)
            {
                delegateAction(entity);
            }

            return saveOrUpdate(entity);
        }

        private static T saveOrUpdate(T entity)
        {
			var session = SessionManager.GetCurrent();

            if (entity.ID.Equals(default(I)) || session.Contains(entity))
                session.SaveOrUpdate(entity);
            else
                session.Merge(entity);

            return entity;
        }



		public T GetNonCached(I id)
		{
			return SessionManager.GetNonCached<T, I>(id);
		}



        public void Delete(T obj)
        {
			var session = SessionManager.GetCurrent();

			if (obj != null)
                session.Delete(obj);
        }


        public T GetById(I id)
        {
			var session = SessionManager.GetCurrent();
			return session.Get<T>(id);
        }



		public IQuery<T, I> NewQuery()
		{
			var session = SessionManager.GetCurrent();
			return getQuery(session);
		}

		public TResult NewNonCachedQuery<TResult>(Func<IQuery<T, I>, TResult> action)
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

		private IQuery<T, I> getQuery(ISession session)
		{
			return new Query<T, I>(session);
		}
	}
}
