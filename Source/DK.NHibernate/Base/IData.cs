using System;
using Keon.NHibernate.Queries;
using Keon.Util.DB;

namespace Keon.NHibernate.Base
{
	internal interface IData<T>
		where T : class, IEntity, new()
	{
		T SaveOrUpdate(T entity, params BaseRepository<T>.DelegateAction[] actions);
		T GetNonCached(Int32 id);
		void Delete(T obj);
		T GetById(Int32 id);
		IQuery<T> NewQuery();
		TResult NewNonCachedQuery<TResult>(Func<IQuery<T>, TResult> action);
	}
}
