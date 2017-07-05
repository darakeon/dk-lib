﻿using System;
using System.Linq.Expressions;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;

namespace DK.NHibernate.AutoMapping
{
	///<summary>
	/// Extensions to ReserveWord
	///</summary>
	public static class ReservedWord
	{
		/// <summary>
		///  Used on weak entity of 1:1.
		///  Use HasOne on the strong entity mapping.
		/// </summary>
		/// <param name="mapping">Object received in AutoMappingOverride</param>
		/// <param name="propertyExpression">Lambda of property with correspondent property</param>
		public static PropertyPart MapAsReservedWord<T>(this AutoMapping<T> mapping, Expression<Func<T, object>> propertyExpression)
		{
			var unary = (UnaryExpression) propertyExpression.Body;
			var body = (MemberExpression)unary.Operand;

			return mapping.Map(propertyExpression).Column("_" + body.Member.Name);
		}

	}
}