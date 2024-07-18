using System;
using System.Collections.Generic;
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
	/// <typeparam name="Result">Result class of summarize</typeparam>
	/// <typeparam name="Prop">Property to summarize</typeparam>
	internal class Summarize<Entity, ID, Result, Prop>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
		where Result : new()
	{
		/// <param name="origin">property on the original object</param>
		/// <param name="destiny">property on the result object</param>
		/// <param name="type">type of aggregation</param>
		internal Summarize(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny,
			SummarizeType type
		)
		{
			var originName = origin.GetName();
			var destinyName = destiny.GetName();

			var getProjection = projections[type];
			var projection = getProjection(originName);

			Projection = Projections.Alias(projection, destinyName);
		}

		internal IProjection Projection { get; }

		private static readonly IDictionary<SummarizeType, Func<String, IProjection>> projections =
			new Dictionary<SummarizeType, Func<String, IProjection>>
			{
				{ SummarizeType.Count, Projections.Count },
				{ SummarizeType.Max, Projections.Max },
				{ SummarizeType.Sum, Projections.Sum },
				{ SummarizeType.Min, Projections.Min },
			};
	}
}
