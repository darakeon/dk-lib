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
	/// <typeparam name="Result">Result class of summarize</typeparam>
	/// <typeparam name="Prop">Property to summarize</typeparam>
	public class Summarize<Entity, ID, Result, Prop>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
		where Result : new()
	{
		private Summarize() { }

		/// <summary>
		/// To construct each parameter of summarize
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		/// <param name="type">(Count, Max, Sum)</param>
		public static Summarize<Entity, ID, Result, Prop> GeS(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny,
			SummarizeType type
		)
		{
			return new Summarize<Entity, ID, Result, Prop>
			{
				Origin = origin.GetName(),
				Destiny = destiny.GetName(),
				OriginFunc = origin,
				DestinyFunc = destiny,
				Type = type
			};
		}

		internal String Origin;
		internal String Destiny;

		internal Expression<Func<Entity, Prop>> OriginFunc;
		internal Expression<Func<Result, Prop>> DestinyFunc;

		internal SummarizeType Type;
	}
}
