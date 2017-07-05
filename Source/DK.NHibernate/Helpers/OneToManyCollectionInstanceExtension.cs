using System;
using FluentNHibernate.Conventions.Instances;

namespace DK.NHibernate.Helpers
{
	static class OneToManyCollectionInstanceExtension
	{
		internal static Boolean IsSystemEntity(this IOneToManyCollectionInstance instance)
		{
			return instance.ChildType != null && !instance.ChildType.IsEnum;
		}

	}
}
