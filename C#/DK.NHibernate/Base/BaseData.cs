using System;
using DK.Generic.DB;
using DK.Generic.Exceptions;

namespace DK.NHibernate.Base
{
    /// <summary>
    /// Base communication with DB
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    internal class BaseData<T>
        where T : class, IEntity, new()
    {
        internal T SaveOrUpdate(T entity, params BaseRepository<T>.DelegateAction[] actions)
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

			try
			{
                if (entity.ID == 0 || session.Contains(entity))
                    session.SaveOrUpdate(entity);
                else
                    session.Merge(entity);
            }
            catch (Exception e)
            {
                DKException.TestOtherIfTooLarge(e);
            }

            return entity;
        }



		public T GetNonCached(Int32 id)
		{
			return SessionManager.GetNonCached<T>(id);
		}



        internal void Delete(T obj)
        {
			var session = SessionManager.GetCurrent();

			if (obj != null)
                session.Delete(obj);
        }


        internal T GetById(Int32 id)
        {
			var session = SessionManager.GetCurrent();
			return session.Get<T>(id);
        }



		public Query<T> NewQuery()
		{
			var session = SessionManager.GetCurrent();
			return new Query<T>(session);
		}

		public TResult NewNonCachedQuery<TResult>(Func<Query<T>, TResult> action)
		{
			TResult result;
			using (var otherSession = SessionManager.GetNonCached())
			{
				var query = new Query<T>(otherSession);
				result = action(query);
				otherSession.Close();
				otherSession.Dispose();
			}
			return result;
		}


	}
}
