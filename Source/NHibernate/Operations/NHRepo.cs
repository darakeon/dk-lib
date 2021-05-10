using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Keon.NHibernate.Queries;
using Keon.Util.DB;

namespace Keon.NHibernate.Operations
{
	/// <summary>
	/// Higher level queries
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	/// <typeparam name="ID">Integer ID type</typeparam>
	public class NHRepo<Entity, ID>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
	{
		private readonly SessionOperations<Entity, ID> data;

		/// <summary>
		/// Initializes DB reader/writer
		/// </summary>
		protected NHRepo()
		{
			data = getBaseData();
		}

		private SessionOperations<Entity, ID> getBaseData()
		{
			return new();
		}

		/// <summary>
		/// Signature of methods to execute on Save or Update
		/// </summary>
		/// <param name="entity"></param>
		public delegate void DelegateAction(Entity entity);

		/// <summary>
		/// Records that at DB
		/// </summary>
		public Entity SaveOrUpdate(Entity entity, params DelegateAction[] actions)
		{
			return data.SaveOrUpdate(entity, actions);
		}

		/// <summary>
		/// Get entity by its ID
		/// </summary>
		public Entity Get(ID id)
		{
			return data.GetById(id);
		}

		/// <summary>
		/// Get old data of the entity
		/// </summary>
		protected Entity getNonCached(ID id)
		{
			return data.GetNonCached(id);
		}

		/// <summary>
		/// Get old data of query
		/// </summary>
		protected TResult newNonCachedQuery<TResult>(Func<Query<Entity, ID>, TResult> action)
		{
			return data.NewNonCachedQuery(action);
		}

		/// <summary>
		/// Verify if there is any entity that correspond to the expression
		/// </summary>
		/// <param name="func"></param>
		/// <returns></returns>
		public Boolean Any(Expression<Func<Entity, Boolean>> func)
		{
			return data.NewQuery().Where(func).Count > 0;
		}

		/// <summary>
		/// Return unique entity for expression
		/// </summary>
		/// <exception cref="Exception">Not unique object</exception>
		public Entity SingleOrDefault(Expression<Func<Entity, Boolean>> func)
		{
			return data.NewQuery().Where(func).SingleOrDefault;
		}

		/// <summary>
		/// Delete permanently the entity of DB
		/// </summary>
		public void Delete(Entity entity)
		{
			data.Delete(entity);
		}

		/// <summary>
		/// Delete permanently the entity of DB
		/// </summary>
		public void Delete(ID id)
		{
			Delete(Get(id));
		}

		/// <summary>
		/// Return an object to take data
		/// </summary>
		public Query<Entity, ID> NewQuery()
		{
			return data.NewQuery();
		}

		/// <summary>
		/// Get all elements of the type from database
		/// </summary>
		public IList<Entity> GetAll()
		{
			return data.NewQuery().List;
		}

		/// <summary>
		/// Use this instead NewQuery from simple conditions
		/// </summary>
		/// <param name="condition">Lambda expression condition</param>
		public IList<Entity> Where(Expression<Func<Entity, bool>> condition)
		{
			return data.NewQuery().Where(condition).List;
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
		public Int32 Count(Expression<Func<Entity, Boolean>> condition)
		{
			return data.NewQuery().Where(condition).Count;
		}
	}
}
