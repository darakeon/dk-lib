using System.Collections.Generic;
using NHibernate;
using NHibernate.Engine;

namespace Keon.NHibernate.Helpers
{
	internal static class SessionExtension
	{
		public static void Refresh(this ISession session)
		{
			var dirtyEntityList = new List<object>();

			var sessionImplementation = session.GetSessionImplementation();
			var persistenceContext = sessionImplementation.PersistenceContext;
			var entityEntryList = persistenceContext.EntityEntries.Values;

			foreach (EntityEntry entityEntry in entityEntryList)
			{
				if (entityEntry.Id == null)
				{
					continue;
				}

				var entity = persistenceContext.GetEntity(entityEntry.EntityKey);
				var persister = entityEntry.Persister;
				var currentState = persister.GetPropertyValues(entity);
				var loadedState = entityEntry.LoadedState;

				if (loadedState == null)
				{
					continue;
				}

				try
				{
					var dirtyEntity = entityEntry.Persister.FindDirty(
						currentState, loadedState, entity, sessionImplementation
					);

					if (dirtyEntity != null)
					{
						dirtyEntityList.Add(entity);
					}
				}
				catch (TransientObjectException) { }
			}

			foreach (var dirtyEntity in dirtyEntityList)
			{
				try
				{
					session.Evict(dirtyEntity);
				}
				catch (UnresolvableObjectException) { }
			}
		}
	}
}
