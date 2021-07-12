using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Keon.NHibernate.Mappings
{
	internal class BooleanConvention : IUserTypeConvention
	{
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			criteria.Expect(
				p => p.Type == typeof(Boolean)
					|| p.Type == typeof(Boolean?)
			);
		}

		public void Apply(IPropertyInstance instance)
		{
			instance.CustomType(instance.Property.PropertyType);
			instance.CustomSqlType("bit");
		}
	}
}
