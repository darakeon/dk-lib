using System;
using System.Linq.Expressions;
using Keon.Util.DB;
using Keon.Util.Reflection;

namespace Keon.NHibernate.Queries
{
	/// <summary>
	/// Class to construct Summarize parameters
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	/// <typeparam name="ID">Integer ID type</typeparam>
	/// <typeparam name="Result">Result class of grouping</typeparam>
	/// <typeparam name="Prop">Type of the property in both entities</typeparam>
	public class GroupBy<Entity, ID, Result, Prop>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
		where Result : new()
	{
		private GroupBy() { }

		/// <summary>
		/// To construct each parameter of grouping
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		public static GroupBy<Entity, ID, Result, Prop> GeG(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny
		)
		{
			return new GroupBy<Entity, ID, Result, Prop>
			{
				Origin = origin.GetName(),
				Destiny = destiny.GetName(),
				OriginFunc = origin,
				DestinyFunc = destiny,
			};
		}

		internal String Origin;
		internal String Destiny;

		internal Expression<Func<Entity, Prop>> OriginFunc;
		internal Expression<Func<Result, Prop>> DestinyFunc;
	}
}
