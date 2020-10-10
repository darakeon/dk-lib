using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Keon.NHibernate.Files;
using Keon.NHibernate.Queries;
using Keon.Util.DB;
using Keon.Util.Extensions;

namespace Keon.NHibernate.Operations
{
	/// <summary>
	/// Higher level queries
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	/// <typeparam name="ID">Integer ID type</typeparam>
	public class BaseRepository<Entity, ID>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
	{
		private readonly BaseData<Entity, ID> data;

		/// <summary>
		/// Initializes DB reader/writer
		/// </summary>
		protected BaseRepository()
		{
			data = getBaseData();
		}

		private BaseData<Entity, ID> getBaseData()
		{
			return new BaseData<Entity, ID>();
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
			return data.NewQuery().SimpleFilter(func).Count > 0;
		}

		/// <summary>
		/// Return unique entity for expression
		/// </summary>
		/// <exception cref="Exception">Not unique object</exception>
		public Entity SingleOrDefault(Expression<Func<Entity, Boolean>> func)
		{
			return data.NewQuery().SimpleFilter(func).UniqueResult;
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
			return data.NewQuery().Result;
		}

		/// <summary>
		/// Use this instead NewQuery from simple conditions
		/// </summary>
		/// <param name="condition">Lambda expression condition</param>
		public IList<Entity> SimpleFilter(Expression<Func<Entity, bool>> condition)
		{
			return data.NewQuery().SimpleFilter(condition).Result;
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
			return data.NewQuery().SimpleFilter(condition).Count;
		}

		/// <summary>
		/// Save file method for attach file to entity
		/// </summary>
		protected static void saveFile<TUpload, TEntity>(TUpload upload, TEntity entity, String uploadsDirectory)
			where TUpload : IUpload
			where TEntity : IUploadParent
		{
			var info = new FileInfo(upload.OriginalName);
			var siteDirectory = Directory.GetCurrentDirectory();
			var directory = Path.Combine(siteDirectory, uploadsDirectory);
			var newFileName = Token.New() + info.Extension;
			var path = Path.Combine(directory, newFileName);

			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			upload.Save(path);

			entity.SetFileNames(newFileName, upload.OriginalName);
		}
	}
}
