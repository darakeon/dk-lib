using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Keon.NHibernate.Mappings
{
    internal class NullableConvention
    {
        public class Property : IPropertyConvention
        {
            public void Apply(IPropertyInstance instance)
            {
				if (instance.Type.IsGenericType && instance.Type.IsNullable)
					instance.Nullable();
				else
					instance.Not.Nullable();
            }
        }

        public class Reference : IReferenceConvention
        {
            public void Apply(IManyToOneInstance instance)
            {
                instance.Not.Nullable();
            }
        }
    }
}
