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
	/// <typeparam name="TDestiny">Result class of summarize</typeparam>
	public class Summarize<T, TDestiny, TProp>
		where T : class, IEntity, new()
		where TDestiny : new()
	{
		private Summarize() { }

		/// <summary>
		/// To construct each parameter of summarize
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		/// <param name="type">(Count, Max, Sum)</param>
		public static Summarize<T, TDestiny, TProp> GetSummarize(
			Expression<Func<T, TProp>> origin,
			Expression<Func<TDestiny, TProp>> destiny,
			SummarizeType type
		)
		{
			return new Summarize<T, TDestiny, TProp>
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

		internal Expression<Func<T, TProp>> OriginFunc;
		internal Expression<Func<TDestiny, TProp>> DestinyFunc;

		internal SummarizeType Type;
	}
}