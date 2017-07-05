using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Ak.NHibernate.Conventions
{
    internal class CascadeConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.SaveUpdate();
        }
    }
}
