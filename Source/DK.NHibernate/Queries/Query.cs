using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using DK.NHibernate.Base;
using DK.NHibernate.Helpers;
using Keon.Util.DB;
using Keon.Util.Reflection;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace DK.NHibernate.Queries
{
	/// <summary>
	/// Object to handle database fluently
	/// </summary>
	/// <typeparam name="T">Entity Type</typeparam>
	public class Query<T> : IQuery<T>
		where T : class, IEntity, new()
	{
		private ICriteria criteria { get; set; }
		private Boolean distinctMainEntity { get; set; }

		/// <summary></summary>
		/// <param name="session">NH Session</param>
		internal Query(ISession session)
        {
			criteria = session.CreateCriteria<T>();
		}


		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="where">Lambda expression</param>
		public IQuery<T> SimpleFilter(Expression<Func<T, Boolean>> where)
        {
            criteria = criteria.Add(Restrictions.Where(where));
            return this;
        }


		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="entityRelation">Lambda expression for parent entity</param>
		/// <param name="where">Lambda expression of condition</param>
		public IQuery<T> SimpleFilter<TEntity>(Expression<Func<T, TEntity>> entityRelation, Expression<Func<TEntity, Boolean>> where)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(entityRelation);
			newCriteria.Add(Restrictions.Where(where));
			return this;
		}

		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="entityRelation">Lambda expression for child entity</param>
		/// <param name="where">Lambda expression of condition</param>
		public IQuery<T> SimpleFilter<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation, Expression<Func<TEntity, Boolean>> where)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(entityRelation);
			newCriteria.Add(Restrictions.Where(where));
			return this;
		}



		/// <summary>
		/// List of entities where certain property is in a list of possibilities
		/// </summary>
		/// <param name="property">Lambda of property to test</param>
		/// <param name="contains">List to be verified</param>
		public IQuery<T> InCondition<TEntity>(Expression<Func<T, TEntity>> property, IList<TEntity> contains)
		{
			var propertyName = property.GetName();
			var newCriteria = criteria.GetOrCreatePropertyCriteria(property);
			newCriteria.Add(Restrictions.In(propertyName, contains.ToArray()));

			return this;
		}



		/// <summary>
		/// Test whether a list is not empty
		/// </summary>
		public IQuery<T> IsNotEmpty<TL>(Expression<Func<T, IList<TL>>> listProperty)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(listProperty, JoinType.LeftOuterJoin);
			newCriteria.Add(Restrictions.IsNotNull(Projections.Id()));
			return this;
		}

		/// <summary>
		/// Test whether a list is empty
		/// </summary>
		public IQuery<T> IsEmpty<TL>(Expression<Func<T, IList<TL>>> listProperty)
		{
			var newCriteria = criteria.GetOrCreateRelationCriteria(listProperty, JoinType.LeftOuterJoin);
			newCriteria.Add(Restrictions.IsNull(Projections.Id()));
			return this;
		}


	    /// <summary>
	    /// Search for text inside entity property values
	    /// </summary>
	    /// <param name="property">Lambda of property</param>
	    /// <param name="term">Text to search</param>
	    /// <param name="likeType">Start, End, Both</param>
	    public IQuery<T> LikeCondition(
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

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="ascendingRelation">Relation to parent entity</param>
		/// <param name="property">Property of parent</param>
		/// <param name="term">Terms of search</param>
		public IQuery<T> LikeCondition<TAscending>(
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

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="searchTerms">Fields and texts to search</param>
		public IQuery<T> LikeCondition(IList<SearchItem<T>> searchTerms)
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



		/// <summary>
		/// Show primary entity even if the other entity doesn't exists
		/// </summary>
		/// <param name="entityRelation">Parent entity</param>
		public IQuery<T> LeftJoin<TEntity>(Expression<Func<T, TEntity>> entityRelation)
		{
			criteria.GetOrCreateRelationCriteria(entityRelation, JoinType.LeftOuterJoin);

			return this;
		}

		/// <summary>
		/// Show primary entity even if the other entity doesn't exists
		/// </summary>
		/// <param name="entityRelation">Child entity</param>
		public IQuery<T> LeftJoin<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation)
		{
			criteria.GetOrCreateRelationCriteria(entityRelation, JoinType.LeftOuterJoin);

			return this;
		}

		/// <summary>
		/// Fetch eagerly, using a separate select.
		/// </summary>
		public IQuery<T> FetchModeEager<TEntity>(Expression<Func<T, IList<TEntity>>> listProperty)
		{
			var name = listProperty.GetName();
			criteria.SetFetchMode(name, FetchMode.Eager);

			return this;
		}



		/// <summary>
		/// Verify if a flagged enum has a specific flag
		/// </summary>
		/// <param name="func">Property to be checked</param>
		/// <param name="value">Value to find</param>
		public IQuery<T> HasFlag<TEnum>(Expression<Func<T, TEnum>> func, TEnum value)
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

		

		/// <summary>
		/// Reordering results
		/// </summary>
		/// <param name="order">Property to order</param>
		/// <param name="ascending">Whether the order is ascending (true) or descending (false)</param>
		public IQuery<T> OrderBy<TPropOrder>(Expression<Func<T, TPropOrder>> order, Boolean? ascending = true)
        {
            var propName = order.GetName();

			var orderBy = ascending.HasValue && ascending.Value
				? Order.Asc(propName)
				: Order.Desc(propName);

			criteria = criteria.AddOrder(orderBy);

            return this;
        }

		/// <summary>
		/// Ordering using parent entity
		/// </summary>
		public IQuery<T> OrderByParent<TPropOrder>(Expression<Func<T, TPropOrder>> order, Boolean? ascending = true)
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





		/// <summary>
		/// Take just the first items
		/// </summary>
		/// <param name="topItems">Number of items to take</param>
		public IQuery<T> Take(Int32 topItems)
		{
			if (topItems != 0)
			{
				criteria = criteria
					.SetMaxResults(topItems);
			}

			return this;
		}

		/// <summary>
		/// Verify if there is any item that corresponds to the query
		/// </summary>
		public Boolean Any()
		{
			return Take(1).Result.Count > 0;
		}

	    /// <summary>
	    /// To get a page of the results
	    /// </summary>
	    /// <param name="search">Parameters of paging</param>
	    public IQuery<T> Page(ISearch search)
		{
			return page(search.ItemsPerPage, search.Page);
		}

		private IQuery<T> page(Int32? itemsPerPage, Int32? page = 1)
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



		/// <summary>
		/// To do not duplicate main entity
		/// </summary>
		public IQuery<T> DistinctMainEntity()
		{
			criteria.SetResultTransformer(Transformers.DistinctRootEntity);

			distinctMainEntity = true;

			return this;
		}


		/// <summary>
		/// Group and summarize result
		/// </summary>
		/// <param name="groupProperties">Group by</param>
		/// <param name="summarizeProperties">Summarize properties (Count, Max, Sum)</param>
		/// <typeparam name="TDestiny">Type of class to be returned (summarized)</typeparam>
		/// <typeparam name="TProp">Type of the property in both entities</typeparam>
		/// <typeparam name="TGroupBy">Need to construct from Query.GroupBy</typeparam>
		/// <typeparam name="TSummarize">Need to construct from Query.Summarize</typeparam>
		public IQuery<T> TransformResult<TDestiny, TProp, TGroupBy, TSummarize>(
			IList<TGroupBy> groupProperties,
			IList<TSummarize> summarizeProperties
		)
			where TGroupBy : GroupBy<T, TDestiny, TProp>
			where TSummarize : Summarize<T, TDestiny, TProp>
			where TDestiny : new()
		{
			setProjections<TDestiny, TProp, TGroupBy, TSummarize>(groupProperties, summarizeProperties);
			criteria.SetResultTransformer(Transformers.AliasToBean(typeof(TDestiny)));

			return this;
		}

		private void setProjections<TDestiny, TProp, TGroupBy, TSummarize>
		(
			IEnumerable<TGroupBy> groupProperties,
			IEnumerable<TSummarize> summarizeProperties
		)
			where TGroupBy : GroupBy<T, TDestiny, TProp>
			where TSummarize : Summarize<T, TDestiny, TProp>
			where TDestiny : new()
		{
			var projections = Projections.ProjectionList();

			setGroupProjections<TDestiny, TProp, TGroupBy>(projections, groupProperties);
			setSummarizeProjections<TDestiny, TProp, TSummarize>(projections, summarizeProperties);

			criteria.SetProjection(projections);
		}

		/// <summary>
		/// Group and summarize result
		/// </summary>
		/// <param name="groupProperties">Group by</param>
		/// <typeparam name="TDestiny">Type of class to be returned (summarized)</typeparam>
		/// <typeparam name="TProp">Type of the property in both entities</typeparam>
		/// <typeparam name="TGroupBy">Need to construct from Query.GroupBy</typeparam>
		public IQuery<T> TransformResult<TDestiny, TProp, TGroupBy>(IList<TGroupBy> groupProperties)
			where TGroupBy : GroupBy<T, TDestiny, TProp> where TDestiny : new()
		{
			setProjections<TDestiny, TProp, TGroupBy>(groupProperties);
			criteria.SetResultTransformer(Transformers.AliasToBean(typeof(TDestiny)));

			return this;
		}

		private void setProjections<TDestiny, TProp, TGroupBy>(IEnumerable<TGroupBy> groupProperties)
			where TGroupBy : GroupBy<T, TDestiny, TProp>
			where TDestiny : new()
		{
			var projections = Projections.ProjectionList();

			setGroupProjections<TDestiny, TProp, TGroupBy>(projections, groupProperties);

			criteria.SetProjection(projections);
		}

		private static void setGroupProjections<TDestiny, TProp, TGroupBy>(
			ProjectionList list, IEnumerable<TGroupBy> group
		)
			where TGroupBy : GroupBy<T, TDestiny, TProp>
			where TDestiny : new()
		{
			foreach (var expression in group)
			{
				list.Add(Projections.Alias(Projections.GroupProperty(expression.Origin), expression.Destiny));
			}
		}

		private static void setSummarizeProjections
			<TDestiny, TProp, TSummarize>
		(ProjectionList projections, IEnumerable<TSummarize> summarizeProperties)
			where TSummarize : Summarize<T, TDestiny, TProp>
			where TDestiny : new()
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




		/// <summary>
		/// Execute the query, getting just the amount of items
		/// </summary>
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

		
		/// <summary>
		/// Execute the query, getting all the results
		/// </summary>
		public IList<T> Result => criteria.List<T>();


	    /// <summary>
		/// Execute the query, return just one result
		/// </summary>
		/// <exception cref="NonUniqueResultException">If there is more than one result from constructed query</exception>
		public T UniqueResult => criteria.UniqueResult<T>();


		/// <summary>
		/// Return first result of list, or null if list is empty
		/// </summary>
		public T FirstOrDefault => page(1).UniqueResult;


		/// <summary>
		/// Result for summarized queries
		/// </summary>
		public IList<TResult> ResultAs<TResult>()
		{
			return criteria.List<TResult>();
		}


	    /// <summary>
		/// Sum for Int32 field
		/// </summary>
		public Int32 SumInt(Expression<Func<T, object>> property)
		{
			return (Int32?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}

		/// <summary>
		/// Sum for Int64 field
		/// </summary>
		public Int64 SumLong(Expression<Func<T, object>> property)
		{
			return (Int64?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}

		/// <summary>
		/// Sum for Decimal field
		/// </summary>
		public Decimal SumDecimal(Expression<Func<T, object>> property)
		{
			return (Decimal?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? 0;
		}
	}
}