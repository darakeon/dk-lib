﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;

namespace Keon.NHibernate.AutoMapping
{
	///<summary>
	/// Extensions to relation 1:N
	///</summary>
	public static class AutoMappingOne2ManyEnum
	{
		/// <summary>
		///  Used at Enum Lists
		/// </summary>
		/// <param name="mapping">Object received in AutoMappingOverride</param>
		/// <param name="propertyExpression">Lambda of property with correspondent enum list</param>
		public static OneToManyPart<TEnum> IsEnumList<T, TEnum>(this AutoMapping<T> mapping, Expression<Func<T, IEnumerable<TEnum>>> propertyExpression)
			where TEnum : struct, IConvertible
		{
			return mapping.HasMany(propertyExpression)
				.Element("Value", e => e.Type<Int16>())
				.AsBag();
		}

	}
}