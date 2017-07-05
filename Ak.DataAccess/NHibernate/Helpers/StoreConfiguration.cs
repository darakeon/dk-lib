using System;
using Ak.Generic.Collection;
using FluentNHibernate.Automapping;

namespace Ak.DataAccess.NHibernate.Helpers
{
    internal class StoreConfiguration : DefaultAutomappingConfiguration
    {
        private readonly String entitiesNamespace;
        private readonly Type[] superEntities;

        public StoreConfiguration(Type entities, params Type[] superEntities)
        {
            entitiesNamespace = entities.Namespace;
            this.superEntities = superEntities;
        }

        public override Boolean ShouldMap(Type type)
        {
            return !type.IsNested && type.Namespace == entitiesNamespace;
        }

        public override Boolean IsDiscriminated(Type type)
        {
            return type.IsIn(superEntities);
        }

        
    }
}
