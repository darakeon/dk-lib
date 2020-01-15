using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Keon.NHibernate.Base;
using Keon.NHibernate.Helpers;
using Keon.Util.DB;
using Keon.Util.Reflection;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Keon.NHibernate.Queries
{
	class Query<T, I> : IQuery<T, I>
		where T : class, IEntity<I>, new()
		where I : struct
	{
		private ICriteria criteria { get; set; }
		private Boolean distinctMainEntity { get; set; }

		public Query(ISession session)
		{
			criteria = session.CreateCriteria<T>();
		}

		public IQuery<T, I> SimpleFilter(Expression<Func<T, Boolean>> where)
        {
            criteria = criteria.Add(Restrictions.Where(where));
            return this;
        }

		public IQuery<T, I> SimpleFilter<TEntity>(Expression<Func<T, TEntity>> entityRelation, Expression<Func<TEntity, Boolean>> where)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(entityRelation);
			newCriteria.Add(Restrictions.Where(where));
			return this;
		}

		public IQuery<T, I> SimpleFilter<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation, Expression<Func<TEntity, Boolean>> where)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(entityRelation);
			newCriteria.Add(Restrictions.Where(where));
			return this;
		}

		public IQuery<T, I> InCondition<TEntity>(Expression<Func<T, TEntity>> property, IList<TEntity> contains)
		{
			var propertyName = property.GetName();
			var newCriteria = criteria.GetOrCreatePropertyCriteria(property);
			newCriteria.Add(Restrictions.In(propertyName, contains.ToArray()));

			return this;
		}

		public IQuery<T, I> IsNotEmpty<L>(Expression<Func<T, IList<L>>> listProperty)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(listProperty, JoinType.LeftOuterJoin);
			newCriteria.Add(Restrictions.IsNotNull(Projections.Id()));
			return this;
		}

		public IQuery<T, I> IsEmpty<L>(Expression<Func<T, IList<L>>> listProperty)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(listProperty, JoinType.LeftOuterJoin);
			newCriteria.Add(Restrictions.IsNull(Projections.Id()));
			return this;
		}

	    public IQuery<T, I> LikeCondition(
		    Expression<Func<T, object>> property,
		    String term,
		    LikeType likeType = LikeType.Both
		)
		{
			var searchTerms = new List<SearchItem<T>>
			{
				new SearchItem<T>(property, term)
			};

			criteria = criteria.Add(accumulateLikeOr(searchTerms, likeType));

			return this;
		}

		public IQuery<T, I> LikeCondition<TAscending>(
			Expression<Func<T, TAscending>> ascendingRelation,
			Expression<Func<TAscending, object>> property,
			String term
		)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(ascendingRelation);

			var searchTerms = new List<SearchItem<TAscending>>
			{
				new SearchItem<TAscending>(property, term)
			};

			newCriteria.Add(accumulateLikeOr(searchTerms));

			return this;
		}

		public IQuery<T, I> LikeCondition(IList<SearchItem<T>> searchTerms)
		{
			criteria = criteria.Add(accumulateLikeOr(searchTerms));

			foreach (var searchTerm in searchTerms)
			{
				if (searchTerm.ParentType() == typeof(T))
					continue;

				var type = searchTerm.ParentType();
				addParent(type);
			}

			return this;
		}

		private static AbstractCriterion accumulateLikeOr<TSearch>(ICollection<SearchItem<TSearch>> searchTerms, LikeType likeType = LikeType.Both)
		{
			if (!searchTerms.Any())
				return null;

			var searchTerm = searchTerms.First();

			var likeTerm = searchTerm.Term;

			if (likeType != LikeType.JustEnd)
			{
				likeTerm = likeTerm + "%";
			}

			if (likeType != LikeType.JustStart)
			{
				likeTerm = "%" + likeTerm;
			}

			var restriction = Restrictions.On(
				searchTerm.Property
			).IsInsensitiveLike(likeTerm);

			if (searchTerms.Count == 1)
			{
				return restriction;
			}

			var otherTerms = searchTerms.Skip(1).ToList();
			var otherProcessedTerms = accumulateLikeOr(otherTerms);

			return Restrictions.Or(restriction, otherProcessedTerms);
		}

		private readonly IList<String> aliases = new List<String>();

		private void addParent(Type type)
		{
			if (aliases.Contains(type.Name))
				return;

			aliases.Add(type.Name);
			criteria.CreateAlias(type.Name, type.Name, JoinType.LeftOuterJoin);
		}

		public IQuery<T, I> LeftJoin<TEntity>(Expression<Func<T, TEntity>> entityRelation)
		{
			criteria.GetOrCreateRelationCriteria(entityRelation, JoinType.LeftOuterJoin);

			return this;
		}

		public IQuery<T, I> LeftJoin<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation)
		{
			criteria.GetOrCreateRelationCriteria(entityRelation, JoinType.LeftOuterJoin);

			return this;
		}

		public IQuery<T, I> FetchModeEager<TEntity>(Expression<Func<T, IList<TEntity>>> listProperty)
		{
			var name = listProperty.GetName();
			criteria.Fetch(SelectMode.Fetch, name);

			return this;
		}

		public IQuery<T, I> HasFlag<TEnum>(Expression<Func<T, TEnum>> func, TEnum value)
			where TEnum : struct, IConvertible
		{
			var columnName = func.GetName();
			var integerValue = value.ToInt32(new NumberFormatInfo());
			var query = $"({{alias}}.{columnName} & {integerValue}) as FlagCheck";

			var sqlProjection = Projections.SqlProjection(query, null, null);
			var equal = Restrictions.Eq(sqlProjection, integerValue);

			criteria.Add(equal);

			return this;
		}

		public IQuery<T, I> OrderBy<TPropOrder>(Expression<Func<T, TPropOrder>> order, Boolean? ascending = true)
        {
            var propName = order.GetName();

			var orderBy = ascending.HasValue && ascending.Value
				? Order.Asc(propName)
				: Order.Desc(propName);

			criteria = criteria.AddOrder(orderBy);

            return this;
        }

		public IQuery<T, I> OrderByParent<TPropOrder>(Expression<Func<T, TPropOrder>> order, Boolean? ascending = true)
		{
			criteria.GetOrCreatePropertyCriteria(order, JoinType.LeftOuterJoin);

			var normalizedPropertyName = order.NormalizePropertyName();
			var propName = String.Join(".", normalizedPropertyName);

			var orderBy = ascending.HasValue && ascending.Value
				? Order.Asc(propName)
				: Order.Desc(propName);

			criteria = criteria.AddOrder(orderBy);

			DistinctMainEntity();

			return this;
		}

		public IQuery<T, I> Take(Int32 topItems)
		{
			if (topItems != 0)
			{
				criteria = criteria
					.SetMaxResults(topItems);
			}

			return this;
		}

		public Boolean Any()
		{
			return Take(1).Result.Count > 0;
		}

	    public IQuery<T, I> Page(ISearch search)
		{
			return page(search.ItemsPerPage, search.Page);
		}

		private IQuery<T, I> page(Int32? itemsPerPage, Int32? page = 1)
		{
			// ReSharper disable once InvertIf
			if (itemsPerPage.HasValue && page != 0)
			{
				var skip = ((page ?? 1) - 1) * itemsPerPage.Value;

				criteria = criteria
					.SetFirstResult(skip)
					.SetMaxResults(itemsPerPage.Value);
			}

			return this;
		}

		public IQuery<T, I> DistinctMainEntity()
		{
			criteria.SetResultTransformer(Transformers.DistinctRootEntity);

			distinctMainEntity = true;

			return this;
		}

		public IQuery<T, I> TransformResult<D, P, G, S>(
			IList<G> groupProperties,
			IList<S> summarizeProperties
		)
			where G : GroupBy<T, I, D, P>
			where S : Summarize<T, I, D, P>
			where D : new()
		{
			setProjections<D, P, G, S>(groupProperties, summarizeProperties);
			criteria.SetResultTransformer(Transformers.AliasToBean(typeof(D)));

			return this;
		}

		private void setProjections<D, P, G, S>
		(
			IEnumerable<G> groupProperties,
			IEnumerable<S> summarizeProperties
		)
			where G : GroupBy<T, I, D, P>
			where S : Summarize<T, I, D, P>
			where D : new()
		{
			var projections = Projections.ProjectionList();

			setGroupProjections<D, P, G>(projections, groupProperties);
			seSProjections<D, P, S>(projections, summarizeProperties);

			criteria.SetProjection(projections);
		}

		public IQuery<T, I> TransformResult<D, P, G>(IList<G> groupProperties)
			where G : GroupBy<T, I, D, P> where D : new()
		{
			setProjections<D, P, G>(groupProperties);
			criteria.SetResultTransformer(Transformers.AliasToBean(typeof(D)));

			return this;
		}

		private void setProjections<D, P, G>(IEnumerable<G> groupProperties)
			where G : GroupBy<T, I, D, P>
			where D : new()
		{
			var projections = Projections.ProjectionList();

			setGroupProjections<D, P, G>(projections, groupProperties);

			criteria.SetProjection(projections);
		}

		private static void setGroupProjections<D, P, G>(
			ProjectionList list, IEnumerable<G> group
		)
			where G : GroupBy<T, I, D, P>
			where D : new()
		{
			foreach (var expression in group)
			{
				list.Add(Projections.Alias(Projections.GroupProperty(expression.Origin), expression.Destiny));
			}
		}

		private static void seSProjections
			<D, P, S>
		(ProjectionList projections, IEnumerable<S> summarizeProperties)
			where S : Summarize<T, I, D, P>
			where D : new()
		{
			foreach (var associationProperty in summarizeProperties)
			{
				var projection = getProjection(associationProperty.Origin, associationProperty.Type);
				projections.Add(Projections.Alias(projection, associationProperty.Destiny));
			}
		}

		private static IProjection getProjection(String property, SummarizeType type)
		{
			switch (type)
			{
				case SummarizeType.Count:
					return Projections.Count(property);
				case SummarizeType.Max:
					return Projections.Max(property);
				case SummarizeType.Sum:
					return Projections.Sum(property);
				default:
					throw new NotImplementedException();
			}
		}

		public Int32 Count
        {
            get
            {
				var countProjection = distinctMainEntity
					? Projections.Count(Projections.Distinct(Projections.Id()))
					: Projections.Count(Projections.Id());

				return criteria
					.SetProjection(countProjection)
					.List<Int32>()
					.Sum();
            }
        }

		public IList<T> Result => criteria.List<T>();

		public T UniqueResult => criteria.UniqueResult<T>();

		public T FirstOrDefault => page(1).UniqueResult;

		public IList<TResult> ResultAs<TResult>()
		{
			return criteria.List<TResult>();
		}

		public Int32 SumInt(Expression<Func<T, object>> property)
		{
			return (Int32?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}

		public Int64 SumLong(Expression<Func<T, object>> property)
		{
			return (Int64?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}

		public Decimal SumDecimal(Expression<Func<T, object>> property)
		{
			return (Decimal?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}
	}
}
