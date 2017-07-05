using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ak.Generic.DB;

namespace Ak.NHibernate
{
    /// <summary>
    /// Higher level queries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> where T : class, IEntity
    {
        private readonly BaseData<T> data;

        /// <summary>
        /// Initializes DB reader/writer
        /// </summary>
        protected BaseRepository()
        {
            data = new BaseData<T>();
        }



        /// <summary>
        /// Signature of methods to execute on Save or Update
        /// </summary>
        /// <param name="entity"></param>
        public delegate void DelegateAction(T entity);



        /// <summary>
        /// Records that at DB
        /// </summary>
        public T SaveOrUpdate(T entity, params DelegateAction[] actions)
        {
            return data.SaveOrUpdate(entity, actions);
        }


        /// <summary>
        /// Get entity by its ID
        /// </summary>
        public T GetById(Int32 id)
        {
            return data.GetById(id);
        }


        /// <summary>
        /// Verify if there is any entity that correspond to the expression
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Boolean Exists(Expression<Func<T, Boolean>> func)
        {
            return data.Exists(func);
        }


        /// <summary>
        /// Return unique entity for expression
        /// </summary>
        /// <exception cref="Exception">Not unique object</exception>
        public T SingleOrDefault(Expression<Func<T, Boolean>> func)
        {
            return data.SingleOrDefault(func);
        }


        /// <summary>
        /// Return as list of entities for expression
        /// </summary>
        public IList<T> List(Expression<Func<T, Boolean>> func)
        {
            return data.GetWhere(func);
        }


        /// <summary>
        /// Delete permanently the entity of DB
        /// </summary>
        public void Delete(T entity)
        {
            data.Delete(entity);
        }

        /// <summary>
        /// Delete permanently the entity of DB
        /// </summary>
        public void Delete(Int32 id)
        {
            Delete(GetById(id));
        }


        /// <summary>
        /// Get old data of the entity
        /// </summary>
        public T GetOldById(Int32 id)
        {
            return data.GetOldById(id);
        }


    }
}
