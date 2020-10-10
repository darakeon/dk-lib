using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Keon.NHibernate.Mappings
{
	internal class EnumConvention : IUserTypeConvention
	{
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			criteria.Expect(p => p.Type.IsEnum);
		}

		public void Apply(IPropertyInstance instance)
		{
			instance.CustomType(instance.Property.PropertyType);
			instance.CustomSqlType("smallint");
		}
	}
}
