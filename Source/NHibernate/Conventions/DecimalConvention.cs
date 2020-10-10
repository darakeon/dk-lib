using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Keon.NHibernate.Conventions
{
	internal class DecimalConvention : IUserTypeConvention
	{
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			criteria.Expect(
				p => p.Type == typeof(Decimal) 
					|| p.Type == typeof(Decimal?)
			);
		}

		public void Apply(IPropertyInstance instance)
		{
			instance.CustomType(instance.Property.PropertyType);
			instance.CustomSqlType("decimal");
		}
	}
}
