using System.Collections.Generic;
using Keon.Util.DB;

namespace Keon.NHibernate.Fakes
{
	interface IDbBackup<I>
		where I : struct
	{
		ICollection<I> Keys { get; }
		void Remove(I entityId);
		void Add(IEntity<I> entity);
		void Replace(IEntity<I> entity);
		IDictionary<I, IEntity<I>> Clone();
	}
}
