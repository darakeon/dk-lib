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
	/// <summary>
	/// Object to handle database fluently
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	/// <typeparam name="ID">Integer ID type</typeparam>
	public class Query<Entity, ID> : Executor<Entity>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
	{
		private Boolean distinctMainEntity { get; set; }

		internal Query(ISession session)
		{
			criteria = session.CreateCriteria<Entity>();
		}

		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="where">Lambda expression</param>
		public Query<Entity, ID> Where(Expression<Func<Entity, Boolean>> where)
        {
            criteria = criteria.Add(Restrictions.Where(where));
            return this;
        }

		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="entityRelation">Lambda expression for parent entity</param>
		/// <param name="where">Lambda expression of condition</param>
		public Query<Entity, ID> Where<Parent>(
			Expression<Func<Entity, Parent>> entityRelation,
			Expression<Func<Parent, Boolean>> where
		)
		{
			var newCriteria = criteria.RelationCriteria(entityRelation);
			newCriteria.Add(Restrictions.Where(where));
			return this;
		}

		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="entityRelation">Lambda expression for child entity</param>
		/// <param name="where">Lambda expression of condition</param>
		public Query<Entity, ID> Where<Parent>(
			Expression<Func<Entity, IList<Parent>>> entityRelation,
			Expression<Func<Parent, Boolean>> where
		)
		{
			var newCriteria = criteria.RelationCriteria(entityRelation);
			newCriteria.Add(Restrictions.Where(where));
			return this;
		}

		/// <summary>
		/// List of entities where certain property is in a list of possibilities
		/// </summary>
		/// <param name="property">Lambda of property to test</param>
		/// <param name="contains">List to be verified</param>
		public Query<Entity, ID> Contains<Prop>(
			Expression<Func<Entity, Prop>> property, Prop[] contains
		)
		{
			var propertyName = property.GetName();
			
			var newCriteria = criteria.PropertyCriteria(property);
			
			newCriteria.Add(
				Restrictions.In(propertyName, contains)
			);

			return this;
		}

		/// <summary>
		/// Test whether a list is not empty
		/// </summary>
		public Query<Entity, ID> IsNotEmpty<Prop>(
			Expression<Func<Entity, IList<Prop>>> listProperty
		)
		{
			var newCriteria = criteria.RelationCriteria(
				listProperty, JoinType.LeftOuterJoin
			);
			
			newCriteria.Add(Restrictions.IsNotNull(Projections.Id()));
			
			return this;
		}

		/// <summary>
		/// Test whether a list is empty
		/// </summary>
		public Query<Entity, ID> IsEmpty<PropItem>(
			Expression<Func<Entity, IList<PropItem>>> listProperty
		)
		{
			var newCriteria = criteria.RelationCriteria(listProperty, JoinType.LeftOuterJoin);
			newCriteria.Add(Restrictions.IsNull(Projections.Id()));
			return this;
		}

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="property">Lambda of property</param>
		/// <param name="term">Text to search</param>
		/// <param name="likeType">Start, End, Both</param>
	    public Query<Entity, ID> Like(
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

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="ascendingRelation">Relation to parent entity</param>
		/// <param name="property">Property of parent</param>
		/// <param name="term">Terms of search</param>
		public Query<Entity, ID> Like<Parent>(
			Expression<Func<Entity, Parent>> ascendingRelation,
			Expression<Func<Parent, object>> property,
			String term
		)
		{
			var newCriteria = criteria.RelationCriteria(ascendingRelation);

			var searchTerms = new List<SearchItem<Parent>>
			{
				new SearchItem<Parent>(property, term)
			};

			newCriteria.Add(accumulateLikeOr(searchTerms));

			return this;
		}

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="searchTerms">Fields and texts to search</param>
		public Query<Entity, ID> Like(
			IList<SearchItem<Entity>> searchTerms
		)
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

		private static AbstractCriterion accumulateLikeOr<Search>(
			ICollection<SearchItem<Search>> searchTerms,
			LikeType likeType = LikeType.Both
		)
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
		public Query<Entity, ID> LeftJoin<Parent>(
			Expression<Func<Entity, Parent>> entityRelation
		)
		{
			criteria.RelationCriteria(
				entityRelation, JoinType.LeftOuterJoin
			);

			return this;
		}

		/// <summary>
		/// Show primary entity even if the other entity doesn't exists
		/// </summary>
		/// <param name="entityRelation">Child entity</param>
		public Query<Entity, ID> LeftJoin<Parent>(
			Expression<Func<Entity, IList<Parent>>> entityRelation
		)
		{
			criteria.RelationCriteria(
				entityRelation,
				JoinType.LeftOuterJoin
			);

			return this;
		}

		/// <summary>
		/// Fetch eagerly, using a separate select.
		/// </summary>
		public Query<Entity, ID> Eager<Parent>(
			Expression<Func<Entity, IList<Parent>>> listProperty
		)
		{
			var name = listProperty.GetName();
			criteria.Fetch(SelectMode.Fetch, name);

			return this;
		}

		/// <summary>
		/// Verify if a flagged enum has a specific flag
		/// </summary>
		/// <param name="func">Property to be checked</param>
		/// <param name="value">Value to find</param>
		public Query<Entity, ID> HasFlag<Prop>(
			Expression<Func<Entity, Prop>> func, Prop value
		)
			where Prop : struct, IConvertible
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
		public Query<Entity, ID> OrderBy<Prop>(
			Expression<Func<Entity, Prop>> order,
			Boolean? ascending = true
		)
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
		public Query<Entity, ID> OrderByParent<Parent>(
			Expression<Func<Entity, Parent>> order,
			Boolean? ascending = true
		)
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

		/// <summary>
		/// Take just the first items
		/// </summary>
		/// <param name="topItems">Number of items to take</param>
		public Query<Entity, ID> Take(Int32 topItems)
		{
			if (topItems != 0)
			{
				criteria = criteria
					.SetMaxResults(topItems);
			}

			return this;
		}

		/// <summary>
		/// To get a page of the results
		/// </summary>
		/// <param name="search">Parameters of paging</param>
	    public Query<Entity, ID> Page(ISearch search)
		{
			return page(search.ItemsPerPage, search.Page);
		}

		private Query<Entity, ID> page(Int32? itemsPerPage, Int32? page = 1)
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
		public Query<Entity, ID> DistinctMainEntity()
		{
			criteria.SetResultTransformer(Transformers.DistinctRootEntity);

			distinctMainEntity = true;

			return this;
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
		/// Return first result of list, or null if list is empty
		/// </summary>
		public Entity FirstOrDefault => page(1).SingleOrDefault;

		/// <summary>
		/// Verify if there is any item that corresponds to the query
		/// </summary>
		public Boolean Any => Count > 0;

		/// <summary>
		/// Sum for numeric fields
		/// </summary>
		public Number Sum<Number>(Expression<Func<Entity, object>> property)
			where Number: struct
		{
			return (Number?)criteria
				.SetProjection(Projections.Sum(property))
				.UniqueResult() ?? default;
		}

		public Transformer<Entity, ID, Result> TransformResult<Result>()
			where Result : new()
		{
			return new Transformer<Entity, ID, Result>(criteria);
		}
	}
}
