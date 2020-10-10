using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Keon.NHibernate.Base;
using Keon.Util.DB;
using NHibernate;

namespace Keon.NHibernate.Queries
{
	/// <summary>
	/// Object to handle database fluently
	/// </summary>
	/// <typeparam name="Entity">Main entity</typeparam>
	/// <typeparam name="ID">Integer ID type</typeparam>
	public interface IQuery<Entity, ID>
		where Entity : class, IEntity<ID>, new()
		where ID : struct
	{
		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="where">Lambda expression</param>
		IQuery<Entity, ID> SimpleFilter(Expression<Func<Entity, Boolean>> where);

		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="entityRelation">Lambda expression for parent entity</param>
		/// <param name="where">Lambda expression of condition</param>
		IQuery<Entity, ID> SimpleFilter<TEntity>(Expression<Func<Entity, TEntity>> entityRelation, Expression<Func<TEntity, Boolean>> where);

		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="entityRelation">Lambda expression for child entity</param>
		/// <param name="where">Lambda expression of condition</param>
		IQuery<Entity, ID> SimpleFilter<TEntity>(Expression<Func<Entity, IList<TEntity>>> entityRelation, Expression<Func<TEntity, Boolean>> where);

		/// <summary>
		/// List of entities where certain property is in a list of possibilities
		/// </summary>
		/// <param name="property">Lambda of property to test</param>
		/// <param name="contains">List to be verified</param>
		IQuery<Entity, ID> InCondition<TEntity>(Expression<Func<Entity, TEntity>> property, TEntity[] contains);

		/// <summary>
		/// Test whether a list is not empty
		/// </summary>
		IQuery<Entity, ID> IsNotEmpty<Prop>(Expression<Func<Entity, IList<Prop>>> listProperty);

		/// <summary>
		/// Test whether a list is empty
		/// </summary>
		IQuery<Entity, ID> IsEmpty<Prop>(Expression<Func<Entity, IList<Prop>>> listProperty);

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="property">Lambda of property</param>
		/// <param name="term">Text to search</param>
		/// <param name="likeType">Start, End, Both</param>
		IQuery<Entity, ID> LikeCondition(Expression<Func<Entity, object>> property, String term, LikeType likeType = LikeType.Both);

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="ascendingRelation">Relation to parent entity</param>
		/// <param name="property">Property of parent</param>
		/// <param name="term">Terms of search</param>
		IQuery<Entity, ID> LikeCondition<TAscending>(Expression<Func<Entity, TAscending>> ascendingRelation, Expression<Func<TAscending, object>> property, String term);

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="searchTerms">Fields and texts to search</param>
		IQuery<Entity, ID> LikeCondition(IList<SearchItem<Entity>> searchTerms);

		/// <summary>
		/// Show primary entity even if the other entity doesn't exists
		/// </summary>
		/// <param name="entityRelation">Parent entity</param>
		IQuery<Entity, ID> LeftJoin<TEntity>(Expression<Func<Entity, TEntity>> entityRelation);

		/// <summary>
		/// Show primary entity even if the other entity doesn't exists
		/// </summary>
		/// <param name="entityRelation">Child entity</param>
		IQuery<Entity, ID> LeftJoin<TEntity>(Expression<Func<Entity, IList<TEntity>>> entityRelation);

		/// <summary>
		/// Fetch eagerly, using a separate select.
		/// </summary>
		IQuery<Entity, ID> FetchModeEager<TEntity>(Expression<Func<Entity, IList<TEntity>>> listProperty);

		/// <summary>
		/// Verify if a flagged enum has a specific flag
		/// </summary>
		/// <param name="func">Property to be checked</param>
		/// <param name="value">Value to find</param>
		IQuery<Entity, ID> HasFlag<TEnum>(Expression<Func<Entity, TEnum>> func, TEnum value)
			where TEnum : struct, IConvertible;

		/// <summary>
		/// Reordering results
		/// </summary>
		/// <param name="order">Property to order</param>
		/// <param name="ascending">Whether the order is ascending (true) or descending (false)</param>
		IQuery<Entity, ID> OrderBy<Prop>(Expression<Func<Entity, Prop>> order, Boolean? ascending = true);

		/// <summary>
		/// Ordering using parent entity
		/// </summary>
		IQuery<Entity, ID> OrderByParent<Prop>(Expression<Func<Entity, Prop>> order, Boolean? ascending = true);

		/// <summary>
		/// Take just the first items
		/// </summary>
		/// <param name="topItems">Number of items to take</param>
		IQuery<Entity, ID> Take(Int32 topItems);

		/// <summary>
		/// Verify if there is any item that corresponds to the query
		/// </summary>
		Boolean Any();

		/// <summary>
		/// To get a page of the results
		/// </summary>
		/// <param name="search">Parameters of paging</param>
		IQuery<Entity, ID> Page(ISearch search);

		/// <summary>
		/// To do not duplicate main entity
		/// </summary>
		IQuery<Entity, ID> DistinctMainEntity();

		/// <summary>
		/// Group and summarize result
		/// </summary>
		/// <param name="groupProperties">Group by</param>
		/// <param name="summarizeProperties">Summarize properties (Count, Max, Sum)</param>
		/// <typeparam name="Result">Type of class to be returned (summarized)</typeparam>
		/// <typeparam name="Prop">Type of the property in both entities</typeparam>
		/// <typeparam name="Group">Need to construct from Query.GroupBy</typeparam>
		/// <typeparam name="Aggregate">Need to construct from Query.Summarize</typeparam>
		IQuery<Entity, ID> TransformResult<Result, Prop, Group, Aggregate>(
			IList<Group> groupProperties,
			IList<Aggregate> summarizeProperties
		)
			where Group : GroupBy<Entity, ID, Result, Prop>
			where Aggregate : Summarize<Entity, ID, Result, Prop>
			where Result : new();

		/// <summary>
		/// Group and summarize result
		/// </summary>
		/// <param name="groupProperties">Group by</param>
		/// <typeparam name="Result">Type of class to be returned (summarized)</typeparam>
		/// <typeparam name="Prop">Type of the property in both entities</typeparam>
		/// <typeparam name="Group">Need to construct from Query.GroupBy</typeparam>
		IQuery<Entity, ID> TransformResult<Result, Prop, Group>(
			IList<Group> groupProperties
		)
			where Group : GroupBy<Entity, ID, Result, Prop>
			where Result : new();

		/// <summary>
		/// Execute the query, getting just the amount of items
		/// </summary>
		Int32 Count { get; }

		/// <summary>
		/// Execute the query, getting all the results
		/// </summary>
		IList<Entity> Result { get; }

		/// <summary>
		/// Execute the query, return just one result
		/// </summary>
		/// <exception cref="NonUniqueResultException">If there is more than one result from constructed query</exception>
		Entity UniqueResult { get; }

		/// <summary>
		/// Return first result of list, or null if list is empty
		/// </summary>
		Entity FirstOrDefault { get; }

		/// <summary>
		/// Result for summarized queries
		/// </summary>
		IList<TResult> ResultAs<TResult>();

		/// <summary>
		/// Sum for Int32 field
		/// </summary>
		Int32 SumInt(Expression<Func<Entity, object>> property);

		/// <summary>
		/// Sum for Int64 field
		/// </summary>
		Int64 SumLong(Expression<Func<Entity, object>> property);

		/// <summary>
		/// Sum for Decimal field
		/// </summary>
		Decimal SumDecimal(Expression<Func<Entity, object>> property);
	}
}
