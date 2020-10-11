using System;
using System.Linq.Expressions;
using Keon.Util.DB;
using Keon.Util.Reflection;
using NHibernate.Criterion;

namespace Keon.NHibernate.Queries
{
	/// <summary>
	/// Class to construct Summarize parameters
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	/// <typeparam name="ID">Integer ID type</typeparam>
	/// <typeparam name="Result">Result class of grouping</typeparam>
	/// <typeparam name="Prop">Type of the property in both entities</typeparam>
	internal class GroupBy<Entity, ID, Result, Prop>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
		where Result : new()
	{
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		internal GroupBy(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny
		)
		{
			var originName = origin.GetName();
			var destinyName = destiny.GetName();

			var property = Projections.GroupProperty(originName);
			Projection = Projections.Alias(property, destinyName);
		}

		internal IProjection Projection { get; }
	}
}
