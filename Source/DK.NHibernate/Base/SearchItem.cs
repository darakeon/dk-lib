using System;
using System.Linq.Expressions;
using DK.Generic.Reflection;

namespace DK.NHibernate.Base
{
	/// <summary>
	/// To search for string
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SearchItem<T>
	{
		/// <param name="property">Lambda of property</param>
		/// <param name="term">Text to search</param>
		public SearchItem(Expression<Func<T, object>> property, String term)
		{
			Property = property;
			Term = term;
		}

		internal Expression<Func<T, object>> Property { get; private set; }
		internal String Term { get; private set; }

		internal Type ParentType()
		{
			return Property.ReferenceType();
		}


	}


}