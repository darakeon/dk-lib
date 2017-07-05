using System;
using DK.Generic.DB;
using DK.Generic.Exceptions;
using NHibernate;

namespace DK.NHibernate.Base
{
    /// <summary>
    /// Base communication with DB
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    internal class BaseData<T>
        where T : class, IEntity
    {
        private static ISession session
        {
            get { return NHManager.Session; }
        }

        private static ISession sessionOld
        {
            get { return NHManager.SessionOld; }
        }



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
            try
            {
                //TODO
                //if (entity.ID == 0)
                //    session.Save(entity);
                //else if (session.Contains(entity))
                //    session.Update(entity);
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

		

        internal T GetOldById(Int32 id)
        {
            return sessionOld.Get<T>(id);
        }



        internal void Delete(T obj)
        {
            if (obj != null)
                session.Delete(obj);
        }


        internal T GetById(Int32 id)
        {
            return session.Get<T>(id);
        }



		public Query<T> NewQuery()
		{
			return new Query<T>(session);
		}


    }
}
