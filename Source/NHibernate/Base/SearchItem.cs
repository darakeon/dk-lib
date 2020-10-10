using System;
using System.Linq.Expressions;
using Keon.Util.Reflection;

namespace Keon.NHibernate.Base
{
	/// <summary>
	/// To search for string
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	public class SearchItem<Entity>
	{
		/// <param name="property">Lambda of property</param>
		/// <param name="term">Text to search</param>
		public SearchItem(Expression<Func<Entity, object>> property, String term)
		{
			Property = property;
			Term = term;
		}

		internal Expression<Func<Entity, object>> Property { get; }
		internal String Term { get; }

		internal Type ParentType()
		{
			return Property.ReferenceType();
		}
	}
}
