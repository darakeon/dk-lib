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
	/// <typeparam name="T">Entity Type</typeparam>
	public interface IQuery<T>
		where T : class, IEntity, new()
	{
		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="where">Lambda expression</param>
		IQuery<T> SimpleFilter(Expression<Func<T, Boolean>> where);

		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="entityRelation">Lambda expression for parent entity</param>
		/// <param name="where">Lambda expression of condition</param>
		IQuery<T> SimpleFilter<TEntity>(Expression<Func<T, TEntity>> entityRelation, Expression<Func<TEntity, Boolean>> where);

		/// <summary>
		/// Make a filter with lambda expression
		/// </summary>
		/// <param name="entityRelation">Lambda expression for child entity</param>
		/// <param name="where">Lambda expression of condition</param>
		IQuery<T> SimpleFilter<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation, Expression<Func<TEntity, Boolean>> where);

		/// <summary>
		/// List of entities where certain property is in a list of possibilities
		/// </summary>
		/// <param name="property">Lambda of property to test</param>
		/// <param name="contains">List to be verified</param>
		IQuery<T> InCondition<TEntity>(Expression<Func<T, TEntity>> property, IList<TEntity> contains);

		/// <summary>
		/// Test whether a list is not empty
		/// </summary>
		IQuery<T> IsNotEmpty<TL>(Expression<Func<T, IList<TL>>> listProperty);

		/// <summary>
		/// Test whether a list is empty
		/// </summary>
		IQuery<T> IsEmpty<TL>(Expression<Func<T, IList<TL>>> listProperty);

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="property">Lambda of property</param>
		/// <param name="term">Text to search</param>
		/// <param name="likeType">Start, End, Both</param>
		IQuery<T> LikeCondition(Expression<Func<T, object>> property, String term, LikeType likeType = LikeType.Both);

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="ascendingRelation">Relation to parent entity</param>
		/// <param name="property">Property of parent</param>
		/// <param name="term">Terms of search</param>
		IQuery<T> LikeCondition<TAscending>(Expression<Func<T, TAscending>> ascendingRelation, Expression<Func<TAscending, object>> property, String term);

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="searchTerms">Fields and texts to search</param>
		IQuery<T> LikeCondition(IList<SearchItem<T>> searchTerms);

		/// <summary>
		/// Show primary entity even if the other entity doesn't exists
		/// </summary>
		/// <param name="entityRelation">Parent entity</param>
		IQuery<T> LeftJoin<TEntity>(Expression<Func<T, TEntity>> entityRelation);

		/// <summary>
		/// Show primary entity even if the other entity doesn't exists
		/// </summary>
		/// <param name="entityRelation">Child entity</param>
		IQuery<T> LeftJoin<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation);

		/// <summary>
		/// Fetch eagerly, using a separate select.
		/// </summary>
		IQuery<T> FetchModeEager<TEntity>(Expression<Func<T, IList<TEntity>>> listProperty);

		/// <summary>
		/// Verify if a flagged enum has a specific flag
		/// </summary>
		/// <param name="func">Property to be checked</param>
		/// <param name="value">Value to find</param>
		IQuery<T> HasFlag<TEnum>(Expression<Func<T, TEnum>> func, TEnum value)
			where TEnum : struct, IConvertible;

		/// <summary>
		/// Reordering results
		/// </summary>
		/// <param name="order">Property to order</param>
		/// <param name="ascending">Whether the order is ascending (true) or descending (false)</param>
		IQuery<T> OrderBy<TPropOrder>(Expression<Func<T, TPropOrder>> order, Boolean? ascending = true);

		/// <summary>
		/// Ordering using parent entity
		/// </summary>
		IQuery<T> OrderByParent<TPropOrder>(Expression<Func<T, TPropOrder>> order, Boolean? ascending = true);

		/// <summary>
		/// Take just the first items
		/// </summary>
		/// <param name="topItems">Number of items to take</param>
		IQuery<T> Take(Int32 topItems);

		/// <summary>
		/// Verify if there is any item that corresponds to the query
		/// </summary>
		Boolean Any();

		/// <summary>
		/// To get a page of the results
		/// </summary>
		/// <param name="search">Parameters of paging</param>
		IQuery<T> Page(ISearch search);

		/// <summary>
		/// To do not duplicate main entity
		/// </summary>
		IQuery<T> DistinctMainEntity();

		/// <summary>
		/// Group and summarize result
		/// </summary>
		/// <param name="groupProperties">Group by</param>
		/// <param name="summarizeProperties">Summarize properties (Count, Max, Sum)</param>
		/// <typeparam name="TDestiny">Type of class to be returned (summarized)</typeparam>
		/// <typeparam name="TProp">Type of the property in both entities</typeparam>
		/// <typeparam name="TGroupBy">Need to construct from Query.GroupBy</typeparam>
		/// <typeparam name="TSummarize">Need to construct from Query.Summarize</typeparam>
		IQuery<T> TransformResult<TDestiny, TProp, TGroupBy, TSummarize>(
			IList<TGroupBy> groupProperties,
			IList<TSummarize> summarizeProperties
		)
			where TGroupBy : GroupBy<T, TDestiny, TProp>
			where TSummarize : Summarize<T, TDestiny, TProp>
			where TDestiny : new();

		/// <summary>
		/// Group and summarize result
		/// </summary>
		/// <param name="groupProperties">Group by</param>
		/// <typeparam name="TDestiny">Type of class to be returned (summarized)</typeparam>
		/// <typeparam name="TProp">Type of the property in both entities</typeparam>
		/// <typeparam name="TGroupBy">Need to construct from Query.GroupBy</typeparam>
		IQuery<T> TransformResult<TDestiny, TProp, TGroupBy>(
			IList<TGroupBy> groupProperties
		)
			where TGroupBy : GroupBy<T, TDestiny, TProp>
			where TDestiny : new();

		/// <summary>
		/// Execute the query, getting just the amount of items
		/// </summary>
		Int32 Count { get; }

		/// <summary>
		/// Execute the query, getting all the results
		/// </summary>
		IList<T> Result { get; }

		/// <summary>
		/// Execute the query, return just one result
		/// </summary>
		/// <exception cref="NonUniqueResultException">If there is more than one result from constructed query</exception>
		T UniqueResult { get; }

		/// <summary>
		/// Return first result of list, or null if list is empty
		/// </summary>
		T FirstOrDefault { get; }

		/// <summary>
		/// Result for summarized queries
		/// </summary>
		IList<TResult> ResultAs<TResult>();

		/// <summary>
		/// Sum for Int32 field
		/// </summary>
		Int32 SumInt(Expression<Func<T, object>> property);

		/// <summary>
		/// Sum for Int64 field
		/// </summary>
		Int64 SumLong(Expression<Func<T, object>> property);

		/// <summary>
		/// Sum for Decimal field
		/// </summary>
		Decimal SumDecimal(Expression<Func<T, object>> property);
	}
}