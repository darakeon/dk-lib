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
	/// <typeparam name="D">Result class of summarize</typeparam>
	/// <typeparam name="P">Property to summarize</typeparam>
	public class Summarize<T, I, D, P>
		where T : class, IEntity<I>, new()
		where I : struct
		where D : new()
	{
		private Summarize() { }

		/// <summary>
		/// To construct each parameter of summarize
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		/// <param name="type">(Count, Max, Sum)</param>
		public static Summarize<T, I, D, P> GeS(
			Expression<Func<T, P>> origin,
			Expression<Func<D, P>> destiny,
			SummarizeType type
		)
		{
			return new Summarize<T, I, D, P>
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

		internal Expression<Func<T, P>> OriginFunc;
		internal Expression<Func<D, P>> DestinyFunc;

		internal SummarizeType Type;
	}
}
