using System;
using System.Linq;
using FluentNHibernate.Automapping;
using Keon.Util.Collection;
using Keon.Util.DB;

namespace Keon.NHibernate.Schema
{
	internal class StoreConfiguration : DefaultAutomappingConfiguration
	{
		private readonly Type[] ignoredEntities;

		public StoreConfiguration(params Type[] ignoredEntities)
		{
			this.ignoredEntities = ignoredEntities;
		}

		public override Boolean ShouldMap(Type type)
		{
			return !type.IsNested
				&& type.GetInterfaces().Any(
					i => i.GUID == typeof(IEntity<>).GUID
				);
		}

		public override Boolean IsDiscriminated(Type type)
		{
			return type.IsIn(ignoredEntities);
		}
	}
}
