using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ak.Generic.DB;
using Ak.Generic.Exceptions;
using NHibernate;
using NHibernate.Criterion;

namespace Ak.NHibernate
{
    /// <summary>
    /// Base communication with DB
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class BaseData<T>
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



        private static ICriteria createSimpleCriteria(Expression<Func<T, Boolean>> expression = null)
        {
            return session.CreateCriteria<T>().Add(Restrictions.Where(expression));
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
                AkException.TestOtherIfTooLarge(e);
            }

            return entity;
        }



        internal Boolean Exists(Expression<Func<T, Boolean>> func)
        {
            var criteria = createSimpleCriteria(func);

            return criteria.Future<T>().Any();

        }

        internal T SingleOrDefault(Expression<Func<T, Boolean>> func)
        {
            var criteria = createSimpleCriteria(func);

            return criteria.UniqueResult<T>();
        }

        internal IList<T> GetWhere(Expression<Func<T, Boolean>> func)
        {
            var criteria = createSimpleCriteria(func);

            return criteria.List<T>();
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



        internal IList<T> GetAll()
        {
            return createSimpleCriteria().List<T>();
        }


    }
}
