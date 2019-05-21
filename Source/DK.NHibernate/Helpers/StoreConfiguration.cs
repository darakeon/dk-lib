using System;
using DK.Util.Collection;
using DK.Util.DB;
using FluentNHibernate.Automapping;

namespace DK.NHibernate.Helpers
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
