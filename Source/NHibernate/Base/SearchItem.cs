using System;
using System.Linq.Expressions;
using Keon.Util.Reflection;

namespace Keon.NHibernate.Base
{
	/// <summary>
	/// To search for string
	/// </summary>
	/// <typeparam name="T">Main entity</typeparam>
	public class SearchItem<T>
	{
		/// <param name="property">Lambda of property</param>
		/// <param name="term">Text to search</param>
		public SearchItem(Expression<Func<T, object>> property, String term)
		{
			Property = property;
			Term = term;
		}

		internal Expression<Func<T, object>> Property { get; }
		internal String Term { get; }

		internal Type ParentType()
		{
			return Property.ReferenceType();
		}


	}


}