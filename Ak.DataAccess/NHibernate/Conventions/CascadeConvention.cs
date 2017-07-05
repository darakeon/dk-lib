using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Ak.DataAccess.NHibernate.Conventions
{
    internal class CascadeConvention : IReferenceConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.SaveUpdate();
        }
    }
}
