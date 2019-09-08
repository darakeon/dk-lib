using System;
using System.Linq.Expressions;
using Keon.Util.DB;
using Keon.Util.Reflection;

namespace Keon.NHibernate.Queries
{
	/// <summary>
	/// Class to construct Summarize parameters
	/// </summary>
	/// <typeparam name="T">Main entity</typeparam>
	/// <typeparam name="I">Integer ID type</typeparam>
	/// <typeparam name="D">Result class of grouping</typeparam>
	/// <typeparam name="P">Type of the property in both entities</typeparam>
	public class GroupBy<T, I, D, P>
		where T : class, IEntity<I>, new()
		where I : struct
		where D : new()
	{
		private GroupBy() { }

		/// <summary>
		/// To construct each parameter of grouping
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		public static GroupBy<T, I, D, P> GeG(
			Expression<Func<T, P>> origin,
			Expression<Func<D, P>> destiny
		)
		{
			return new GroupBy<T, I, D, P>
			{
				Origin = origin.GetName(),
				Destiny = destiny.GetName(),
				OriginFunc = origin,
				DestinyFunc = destiny,
			};
		}

		internal String Origin;
		internal String Destiny;

		internal Expression<Func<T, P>> OriginFunc;
		internal Expression<Func<D, P>> DestinyFunc;
	}
}
