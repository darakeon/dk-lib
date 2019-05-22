using System;
using FluentNHibernate.Automapping;
using Keon.Util.Collection;
using Keon.Util.DB;

namespace Keon.NHibernate.Helpers
{
    internal class StoreConfiguration : DefaultAutomappingConfiguration
    {
	    private readonly Type[] superEntities;

        public StoreConfiguration(params Type[] superEntities)
        {
	        this.superEntities = superEntities;
        }

        public override Boolean ShouldMap(Type type)
        {
			return !type.IsNested
				&& type.GetInterface(typeof(IEntity).Name) == typeof(IEntity);
		}

		public override Boolean IsDiscriminated(Type type)
		{
			return type.IsIn(superEntities);
		}


	}
}
