﻿using System;
using System.Linq.Expressions;
using Ak.Generic.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;

namespace Ak.NHibernate.AutoMapping
{
	///<summary>
	/// Extensions to relation 1:1
	///</summary>
	public static class AutoMappingOne2One
	{
		/// <summary>
		///  Used on weak entity of 1:1.
		///  Use HasOne on the strong entity mapping.
		/// </summary>
		/// <param name="mapping">Object received in AutoMappingOverride</param>
		/// <param name="propertyExpression">Lambda of property with correspondent entity</param>
		/// <param name="parentExpression">Lambda of property that represents this entity in other entity</param>
		public static OneToOnePart<TFather> IsWeakEntity<T, TFather>(this AutoMapping<T> mapping, Expression<Func<T, TFather>> propertyExpression, Expression<Func<TFather, T>> parentExpression)
		{
			var body = (MemberExpression)parentExpression.Body;

			return mapping.HasOne(propertyExpression).PropertyRef(body.Member.Name);
		}

	}
}