using System;
using FluentNHibernate.Mapping;

namespace Keon.NHibernate.Mappings
{
    internal static class PropertyPartExtension
	{
		public const Int32 big_string = 8000;

		public static PropertyPart ReallyBigString(this PropertyPart propertyPart)
		{
			return propertyPart
				.CustomType("StringClob")
				.CustomSqlType("varchar(" + big_string + ")")
				.Length(big_string);
		}

		public static PropertyPart FileContent(this PropertyPart propertyPart)
		{
			return propertyPart
				.CustomSqlType("mediumtext")
				.Nullable();
		}
    }
}
