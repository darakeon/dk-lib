using System.Collections.Generic;
using NHibernate;
using NHibernate.Engine;

namespace DK.NHibernate.Helpers
{
	static class SessionExtension
	{
        //TODO: REFACTORING: change 'refresh' to 'evict'
		public static void Refresh(this ISession session)
		{
			var dirtyEntityList = new List<object>();

			var sessionImplementation = session.GetSessionImplementation();
			var persistenceContext = sessionImplementation.PersistenceContext;
			var entityMode = sessionImplementation.EntityMode;
			var entityEntryList = persistenceContext.EntityEntries.Values;

			foreach (EntityEntry entityEntry in entityEntryList)
			{
				if (entityEntry.Id == null)
				{
					continue;
				}

				var entity = persistenceContext.GetEntity(entityEntry.EntityKey);
				var persister = entityEntry.Persister;
				var currentState = persister.GetPropertyValues(entity, entityMode);
				var loadedState = entityEntry.LoadedState;

				if (loadedState == null)
				{
					continue;
				}

				try
				{
					var dirtyEntity = entityEntry.Persister.FindDirty(currentState, loadedState, entity, sessionImplementation);

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