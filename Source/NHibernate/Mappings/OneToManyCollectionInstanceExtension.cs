using System;
using FluentNHibernate.Conventions.Instances;

namespace Keon.NHibernate.Mappings
{
	internal static class OneToManyCollectionInstanceExtension
	{
		internal static Boolean IsSystemEntity(
			this IOneToManyCollectionInstance instance
		)
		{
			return instance.ChildType != null 
			    && !instance.ChildType.IsEnum;
		}
	}
}
