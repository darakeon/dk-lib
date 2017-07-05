using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ak.Generic.DB;

namespace Ak.NHibernate.Base
{
	/// <summary>
	/// Higher level queries
	/// </summary>
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
		public T Get(Int32 id)
		{
			return data.GetById(id);
		}

		/// <summary>
		/// Get old data of the entity
		/// </summary>
		public T GetOldById(Int32 id)
		{
			return data.GetOldById(id);
		}



		/// <summary>
		/// Verify if there is any entity that correspond to the expression
		/// </summary>
		/// <param name="func"></param>
		/// <returns></returns>
		public Boolean Any(Expression<Func<T, Boolean>> func)
		{
			return data.NewQuery().Filter(func).Count > 0;
		}


		/// <summary>
		/// Return unique entity for expression
		/// </summary>
		/// <exception cref="Exception">Not unique object</exception>
		public T SingleOrDefault(Expression<Func<T, Boolean>> func)
		{
			return data.NewQuery().Filter(func).UniqueResult;
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
			Delete(Get(id));
		}


		/// <summary>
		/// Return an object to take data
		/// </summary>
		public Query<T> NewQuery()
		{
			return data.NewQuery();
		}


		/// <summary>
		/// Get all elements of the type from database
		/// </summary>
		public IList<T> GetAll()
		{
			return data.NewQuery().Result;
		}

		/// <summary>
		/// Use this instead NewQuery from simple conditions
		/// </summary>
		/// <param name="condition">Lambda expression condition</param>
		public IList<T> SimpleFilter(Expression<Func<T, bool>> condition)
		{
			return data.NewQuery().Filter(condition).Result;
		}

		/// <summary>
		/// Count all elements of the type from database
		/// </summary>
		public Int32 Count()
		{
			return data.NewQuery().Count;
		}

		/// <summary>
		/// Use this instead NewQuery from simple conditions count
		/// </summary>
		/// <param name="condition">Lambda expression condition</param>
		public Int32 Count(Expression<Func<T, Boolean>> condition)
		{
			return data.NewQuery().Filter(condition).Count;
		}







	}
}
