﻿using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace DK.NHibernate.Conventions
{
	internal class BooleanConvention : IUserTypeConvention
	{
		public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
		{
			criteria.Expect(p => p.Type == typeof(Boolean));
		}

		public void Apply(IPropertyInstance instance)
		{
			instance.CustomSqlType("bit");
		}
	}
}