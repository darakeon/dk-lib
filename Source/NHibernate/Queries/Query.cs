using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Keon.NHibernate.Operations;
using Keon.Util.DB;
using Keon.Util.Reflection;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace Keon.NHibernate.Queries
{
	class Query<Entity, ID> : IQuery<Entity, ID>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
	{
		private ICriteria criteria { get; set; }
		private Boolean distinctMainEntity { get; set; }

		public Query(ISession session)
		{
			criteria = session.CreateCriteria<Entity>();
		}

		public IQuery<Entity, ID> SimpleFilter(Expression<Func<Entity, Boolean>> where)
        {
            criteria = criteria.Add(Restrictions.Where(where));
            return this;
        }

		public IQuery<Entity, ID> SimpleFilter<TEntity>(Expression<Func<Entity, TEntity>> entityRelation, Expression<Func<TEntity, Boolean>> where)
		{
			var newCriteria = criteria.RelationCriteria(entityRelation);
			newCriteria.Add(Restrictions.Where(where));
			return this;
		}

		public IQuery<Entity, ID> SimpleFilter<TEntity>(Expression<Func<Entity, IList<TEntity>>> entityRelation, Expression<Func<TEntity, Boolean>> where)
		{
			var newCriteria = criteria.RelationCriteria(entityRelation);
			newCriteria.Add(Restrictions.Where(where));
			return this;
		}

		public IQuery<Entity, ID> InCondition<Prop>(Expression<Func<Entity, Prop>> property, Prop[] contains)
		{
			var propertyName = property.GetName();
			
			var newCriteria = criteria.PropertyCriteria(property);
			
			newCriteria.Add(
				Restrictions.In(propertyName, contains)
			);

			return this;
		}

		public IQuery<Entity, ID> IsNotEmpty<Prop>(
			Expression<Func<Entity, IList<Prop>>> listProperty
		)
		{
			var newCriteria = criteria.RelationCriteria(
				listProperty, JoinType.LeftOuterJoin
			);
			
			newCriteria.Add(Restrictions.IsNotNull(Projections.Id()));
			
			return this;
		}

		public IQuery<Entity, ID> IsEmpty<PropItem>(Expression<Func<Entity, IList<PropItem>>> listProperty)
		{
			var newCriteria = criteria.RelationCriteria(listProperty, JoinType.LeftOuterJoin);
			newCriteria.Add(Restrictions.IsNull(Projections.Id()));
			return this;
		}

	    public IQuery<Entity, ID> LikeCondition(
		    Expression<Func<Entity, object>> property,
		    String term,
		    LikeType likeType = LikeType.Both
		)
		{
			var searchTerms = new List<SearchItem<Entity>>
			{
				new SearchItem<Entity>(property, term)
			};

			criteria = criteria.Add(accumulateLikeOr(searchTerms, likeType));

			return this;
		}

		public IQuery<Entity, ID> LikeCondition<TAscending>(
			Expression<Func<Entity, TAscending>> ascendingRelation,
			Expression<Func<TAscending, object>> property,
			String term
		)
		{
			var newCriteria = criteria.RelationCriteria(ascendingRelation);

			var searchTerms = new List<SearchItem<TAscending>>
			{
				new SearchItem<TAscending>(property, term)
			};

			newCriteria.Add(accumulateLikeOr(searchTerms));

			return this;
		}

		public IQuery<Entity, ID> LikeCondition(IList<SearchItem<Entity>> searchTerms)
		{
			criteria = criteria.Add(accumulateLikeOr(searchTerms));

			foreach (var searchTerm in searchTerms)
			{
				if (searchTerm.ParentType() == typeof(Entity))
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

		public IQuery<Entity, ID> LeftJoin<TEntity>(Expression<Func<Entity, TEntity>> entityRelation)
		{
			criteria.RelationCriteria(entityRelation, JoinType.LeftOuterJoin);

			return this;
		}

		public IQuery<Entity, ID> LeftJoin<TEntity>(Expression<Func<Entity, IList<TEntity>>> entityRelation)
		{
			criteria.RelationCriteria(entityRelation, JoinType.LeftOuterJoin);

			return this;
		}

		public IQuery<Entity, ID> FetchModeEager<TEntity>(Expression<Func<Entity, IList<TEntity>>> listProperty)
		{
			var name = listProperty.GetName();
			criteria.Fetch(SelectMode.Fetch, name);

			return this;
		}

		public IQuery<Entity, ID> HasFlag<TEnum>(Expression<Func<Entity, TEnum>> func, TEnum value)
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

		public IQuery<Entity, ID> OrderBy<TPropOrder>(Expression<Func<Entity, TPropOrder>> order, Boolean? ascending = true)
        {
            var propName = order.GetName();

			var orderBy = ascending.HasValue && ascending.Value
				? Order.Asc(propName)
				: Order.Desc(propName);

			criteria = criteria.AddOrder(orderBy);

            return this;
        }

		public IQuery<Entity, ID> OrderByParent<TPropOrder>(Expression<Func<Entity, TPropOrder>> order, Boolean? ascending = true)
		{
			criteria.PropertyCriteria(order, JoinType.LeftOuterJoin);

			var normalizedPropertyName = order.NormalizePropertyName();
			var propName = String.Join(".", normalizedPropertyName);

			var orderBy = ascending.HasValue && ascending.Value
				? Order.Asc(propName)
				: Order.Desc(propName);

			criteria = criteria.AddOrder(orderBy);

			DistinctMainEntity();

			return this;
		}

		public IQuery<Entity, ID> Take(Int32 topItems)
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

	    public IQuery<Entity, ID> Page(ISearch search)
		{
			return page(search.ItemsPerPage, search.Page);
		}

		private IQuery<Entity, ID> page(Int32? itemsPerPage, Int32? page = 1)
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

		public IQuery<Entity, ID> DistinctMainEntity()
		{
			criteria.SetResultTransformer(Transformers.DistinctRootEntity);

			distinctMainEntity = true;

			return this;
		}

		public IQuery<Entity, ID> TransformResult<Result, Prop, Group, Aggregate>(
			IList<Group> groupProperties,
			IList<Aggregate> summarizeProperties
		)
			where Group : GroupBy<Entity, ID, Result, Prop>
			where Aggregate : Summarize<Entity, ID, Result, Prop>
			where Result : new()
		{
			setProjections<Result, Prop, Group, Aggregate>(groupProperties, summarizeProperties);
			criteria.SetResultTransformer(Transformers.AliasToBean(typeof(Result)));

			return this;
		}

		private void setProjections<Result, Prop, Group, Aggregate>
		(
			IEnumerable<Group> groupProperties,
			IEnumerable<Aggregate> summarizeProperties
		)
			where Group : GroupBy<Entity, ID, Result, Prop>
			where Aggregate : Summarize<Entity, ID, Result, Prop>
			where Result : new()
		{
			var projections = Projections.ProjectionList();

			setGroupProjections<Result, Prop, Group>(projections, groupProperties);
			seSProjections<Result, Prop, Aggregate>(projections, summarizeProperties);

			criteria.SetProjection(projections);
		}

		public IQuery<Entity, ID> TransformResult<Result, Prop, Group>(IList<Group> groupProperties)
			where Group : GroupBy<Entity, ID, Result, Prop> where Result : new()
		{
			setProjections<Result, Prop, Group>(groupProperties);
			criteria.SetResultTransformer(Transformers.AliasToBean(typeof(Result)));

			return this;
		}

		private void setProjections<Result, Prop, Group>(IEnumerable<Group> groupProperties)
			where Group : GroupBy<Entity, ID, Result, Prop>
			where Result : new()
		{
			var projections = Projections.ProjectionList();

			setGroupProjections<Result, Prop, Group>(projections, groupProperties);

			criteria.SetProjection(projections);
		}

		private static void setGroupProjections<Result, Prop, Group>(
			ProjectionList list, IEnumerable<Group> group
		)
			where Group : GroupBy<Entity, ID, Result, Prop>
			where Result : new()
		{
			foreach (var expression in group)
			{
				list.Add(Projections.Alias(Projections.GroupProperty(expression.Origin), expression.Destiny));
			}
		}

		private static void seSProjections<Result, Prop, Aggregate>(
			ProjectionList projections,
			IEnumerable<Aggregate> summarizeProperties
		)
			where Aggregate : Summarize<Entity, ID, Result, Prop>
			where Result : new()
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

		public IList<Entity> Result => criteria.List<Entity>();

		public Entity UniqueResult => criteria.UniqueResult<Entity>();

		public Entity FirstOrDefault => page(1).UniqueResult;

		public IList<TResult> ResultAs<TResult>()
		{
			return criteria.List<TResult>();
		}

		public Int32 SumInt(Expression<Func<Entity, object>> property)
		{
			return (Int32?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}

		public Int64 SumLong(Expression<Func<Entity, object>> property)
		{
			return (Int64?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}

		public Decimal SumDecimal(Expression<Func<Entity, object>> property)
		{
			return (Decimal?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}
	}
}
