using System;
using System.Linq.Expressions;
using Keon.Util.DB;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Keon.NHibernate.Queries
{
	/// <summary>
	/// Object result transformer
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	/// <typeparam name="ID">Integer ID type</typeparam>
	/// <typeparam name="Result">New class to be returned</typeparam>
	public class Transformer<Entity, ID, Result> : Executor<Result>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
		where Result : new()
	{
		internal Transformer(ICriteria criteria)
		{
			this.criteria = criteria;
			projections = Projections.ProjectionList();
		}

		private readonly ProjectionList projections;

		/// <summary>
		/// Add a property to group the final result
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		/// <typeparam name="Prop">Type of the property</typeparam>
		public Transformer<Entity, ID, Result> GroupBy<Prop>(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny
		)
		{
			var group =
				new GroupBy<Entity, ID, Result, Prop>(
					origin, destiny
				);

			projections.Add(group.Projection);

			return this;
		}

		/// <summary>
		/// Add property to Sum
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		/// <typeparam name="Prop">Type of the property</typeparam>
		public Transformer<Entity, ID, Result> Sum<Prop>(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny
		)
		{
			return aggregate(origin, destiny, SummarizeType.Sum);
		}

		/// <summary>
		/// Add property to Count
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		/// <typeparam name="Prop">Type of the property</typeparam>
		public Transformer<Entity, ID, Result> Count<Prop>(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny
		)
		{
			return aggregate(origin, destiny, SummarizeType.Count);
		}

		/// <summary>
		/// Add property to find Max value
		/// </summary>
		/// <param name="origin">Property on original entity</param>
		/// <param name="destiny">Corresponding property on result class</param>
		/// <typeparam name="Prop">Type of the property</typeparam>
		public Transformer<Entity, ID, Result> Max<Prop>(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny
		)
		{
			return aggregate(origin, destiny, SummarizeType.Max);
		}

		private Transformer<Entity, ID, Result> aggregate<Prop>(
			Expression<Func<Entity, Prop>> origin,
			Expression<Func<Result, Prop>> destiny,
			SummarizeType type
		)
		{
			var aggregation =
				new Summarize<Entity, ID, Result, Prop>(
					origin, destiny, type
				);

			projections.Add(aggregation.Projection);

			return this;
		}

		private protected override void executeBeforeEnd()
		{
			criteria.SetProjection(projections);

			var type = typeof(Result);
			var bean = Transformers.AliasToBean(type);
			criteria.SetResultTransformer(bean);
		}
	}
}
