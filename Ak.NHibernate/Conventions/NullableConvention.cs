using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Ak.NHibernate.Conventions
{
    internal class NullableConvention
    {
        public class Property : IPropertyConvention
        {
            public void Apply(IPropertyInstance instance)
            {
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
