using DK.NHibernate.Helpers;
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
				//The weak entity should not update the strong one.
				//Changing to SaveUpdate, transaction stops working.
				instance.Cascade.None();
			}
		}

	    public class OneToMany : IHasManyConvention
	    {
		    public void Apply(IOneToManyCollectionInstance instance)
		    {
				if (instance.IsSystemEntity())
				{
					instance.Cascade.None();
					instance.Inverse();
				}
				else
				{
					instance.Cascade.All();
				}
			}
		}


    }
}
