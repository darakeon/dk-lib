using System;
using System.Linq.Expressions;
using Keon.Util.DB;
using Keon.Util.Reflection;

namespace DK.NHibernate.Queries
{
	/// <summary>
	/// Class to construct Summarize parameters
	/// </summary>
	/// <typeparam name="T">Main entity</typeparam>
	/// <typeparam name="TDestiny">Result class of grouping</typeparam>
	/// <typeparam name="TProp">Type of the property in both entities</typeparam>
	public class GroupBy<T, TDestiny, TProp>
		where T : class, IEntity, new()
		where TDestiny : new()
	{
		private GroupBy() { }

		/// <summary>
		/// To construct each parameter of grouping
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		public static GroupBy<T, TDestiny, TProp> GetGroupBy(
			Expression<Func<T, TProp>> origin,
			Expression<Func<TDestiny, TProp>> destiny
		)
		{
			return new GroupBy<T, TDestiny, TProp>
			{
				Origin = origin.GetName(),
				Destiny = destiny.GetName(),
				OriginFunc = origin,
				DestinyFunc = destiny,
			};
		}

		internal String Origin;
		internal String Destiny;

		internal Expression<Func<T, TProp>> OriginFunc;
		internal Expression<Func<TDestiny, TProp>> DestinyFunc;
	}
}