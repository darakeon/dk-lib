using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace DK.NHibernate.Conventions
{
    internal class CascadeConvention
    {
		public class ManyToOne : IReferenceConvention
	    {
			public void Apply(IManyToOneInstance instance)
			{
				instance.Cascade.None();
			}
		}

	    public class OneToMany : IHasManyConvention
	    {
		    public void Apply(IOneToManyCollectionInstance instance)
		    {
				instance.Cascade.None();
			}
	    }


    }
}
