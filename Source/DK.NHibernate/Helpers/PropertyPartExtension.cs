using System;
using FluentNHibernate.Mapping;

namespace Keon.NHibernate.Helpers
{
    internal static class PropertyPartExtension
	{
		public const Int32 BIG_STRING = 8000;
	    public const String ORDER_COLUMN = "_Order";

		public static PropertyPart ReallyBigString(this PropertyPart propertyPart)
		{
			return propertyPart
				.CustomType("StringClob")
				.CustomSqlType("varchar(" + BIG_STRING + ")")
				.Length(BIG_STRING);
		}

		public static PropertyPart FileContent(this PropertyPart propertyPart)
		{
			return propertyPart
				.CustomSqlType("mediumtext")
				.Nullable();
		}
    }
}