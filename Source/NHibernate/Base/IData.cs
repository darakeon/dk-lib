using System;
using Keon.NHibernate.Queries;
using Keon.Util.DB;

namespace Keon.NHibernate.Base
{
	internal interface IData<Entity, ID>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
	{
		Entity SaveOrUpdate(Entity entity, params BaseRepository<Entity, ID>.DelegateAction[] actions);
		Entity GetNonCached(ID id);
		void Delete(Entity obj);
		Entity GetById(ID id);
		IQuery<Entity, ID> NewQuery();
		TResult NewNonCachedQuery<TResult>(Func<IQuery<Entity, ID>, TResult> action);
	}
}
