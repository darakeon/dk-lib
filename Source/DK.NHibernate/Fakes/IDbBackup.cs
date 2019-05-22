using System;
using System.Collections.Generic;
using Keon.Util.DB;

namespace Keon.NHibernate.Fakes
{
	interface IDbBackup
	{
		ICollection<Int32> Keys { get; }
		void Remove(Int32 entityId);
		void Add(IEntity entity);
		void Replace(IEntity entity);
		IDictionary<Int32, IEntity> Clone();
	}
}