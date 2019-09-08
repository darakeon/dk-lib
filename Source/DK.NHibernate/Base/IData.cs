using System;
using Keon.NHibernate.Queries;
using Keon.Util.DB;

namespace Keon.NHibernate.Base
{
	internal interface IData<T, I>
		where T : class, IEntity<I>, new()
		where I : struct
	{
		T SaveOrUpdate(T entity, params BaseRepository<T, I>.DelegateAction[] actions);
		T GetNonCached(I id);
		void Delete(T obj);
		T GetById(I id);
		IQuery<T, I> NewQuery();
		TResult NewNonCachedQuery<TResult>(Func<IQuery<T, I>, TResult> action);
	}
}
