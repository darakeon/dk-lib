using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using Keon.NHibernate.Base;
using Keon.NHibernate.Queries;
using Keon.Util.DB;
using Decimal = System.Decimal;

namespace Keon.NHibernate.Fakes
{
	internal class FakeQuery<T, I> : IQuery<T, I>
		where T : class, IEntity<I>, new()
		where I : struct
	{
		private IEnumerable<T> localList;
		private IList<object> summarizedList;

		public FakeQuery(IDictionary<I, T> list)
		{
			localList = list.Values.ToList();
			summarizedList = new List<object>();
		}

		public IQuery<T, I> SimpleFilter(Expression<Func<T, bool>> where)
		{
			localList = localList.Where(where.Compile());
			return this;
		}

		public IQuery<T, I> SimpleFilter<TEntity>(Expression<Func<T, TEntity>> entityRelation, Expression<Func<TEntity, bool>> where)
		{
			localList = localList
				.Select(entity =>
					new {
						entity,
						related = entityRelation.Compile()(entity)
					}
				)
				.Where(
					compose => where.Compile()(compose.related)
				)
				.Select(compose => compose.entity);

			return this;
		}

		public IQuery<T, I> SimpleFilter<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation, Expression<Func<TEntity, bool>> where)
		{
			localList = localList
				.Select(entity =>
					new {
						entity,
						related = entityRelation.Compile()(entity)
					}
				)
				.Where(
					compose => compose.related.Any(where.Compile())
				)
				.Select(compose => compose.entity);

			return this;
		}

		public IQuery<T, I> InCondition<TEntity>(Expression<Func<T, TEntity>> property, IList<TEntity> contains)
		{
			localList = localList
				.Select(entity =>
					new {
						entity,
						related = property.Compile()(entity)
					}
				)
				.Where(
					compose => contains.Contains(compose.related)
				)
				.Select(compose => compose.entity);

			return this;
		}

		public IQuery<T, I> IsNotEmpty<L>(Expression<Func<T, IList<L>>> listProperty)
		{
			localList = localList
				.Select(entity =>
					new {
						entity,
						related = listProperty.Compile()(entity)
					}
				)
				.Where(
					compose => compose.related.Any()
				)
				.Select(compose => compose.entity);

			return this;
		}

		public IQuery<T, I> IsEmpty<L>(Expression<Func<T, IList<L>>> listProperty)
		{
			localList = localList
				.Select(entity =>
					new {
						entity,
						related = listProperty.Compile()(entity)
					}
				)
				.Where(
					compose => !compose.related.Any()
				)
				.Select(compose => compose.entity);

			return this;
		}

		public IQuery<T, I> LikeCondition(Expression<Func<T, object>> property, String term, LikeType likeType = LikeType.Both)
		{
			localList = localList
				.Select(entity =>
					new {
						entity,
						related = property.Compile()(entity)
					}
				)
				.Where(
					compose => whereLike(
						compose.related.ToString(),
						term,
						likeType
					)
				)
				.Select(compose => compose.entity);

			return this;
		}

		public IQuery<T, I> LikeCondition<TAscending>(
			Expression<Func<T, TAscending>> ascendingRelation,
			Expression<Func<TAscending, object>> property,
			String term
		)
		{
			localList = localList
				.Select(entity =>
					new {
						entity,
						related = property.Compile()(
							ascendingRelation.Compile()(entity)
						)
					}
				)
				.Where(
					compose => whereLike(compose.related, term, LikeType.Both)
				)
				.Select(compose => compose.entity);

			return this;
		}

		public IQuery<T, I> LikeCondition(IList<SearchItem<T>> searchTerms)
		{
			foreach (var searchTerm in searchTerms)
			{
				LikeCondition(searchTerm.Property, searchTerm.Term);
			}

			return this;
		}

		private Boolean whereLike(object field, String term, LikeType likeType)
		{
			var text = field.ToString();

			switch (likeType)
			{
				case LikeType.Both:
					return text.Contains(term);
				case LikeType.JustStart:
					return text.StartsWith(term);
				case LikeType.JustEnd:
					return text.EndsWith(term);
				default:
					throw new NotImplementedException();
			}
		}

		public IQuery<T, I> LeftJoin<TEntity>(Expression<Func<T, TEntity>> entityRelation)
		{
			return this;
		}

		public IQuery<T, I> LeftJoin<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation)
		{
			return this;
		}

		public IQuery<T, I> FetchModeEager<TEntity>(Expression<Func<T, IList<TEntity>>> listProperty)
		{
			return this;
		}

		public IQuery<T, I> HasFlag<TEnum>(Expression<Func<T, TEnum>> func, TEnum value) where TEnum : struct, IConvertible
		{
			localList = localList
				.Select(entity =>
					new {
						entity,
						related = func.Compile()(entity)
					}
				)
				.Where(
					compose => compose.related.Equals(value)
				)
				.Select(compose => compose.entity);

			return this;
		}

		public IQuery<T, I> OrderBy<O>(Expression<Func<T, O>> order, bool? ascending)
		{
			localList =
				ascending ?? false
					? localList.OrderBy(order.Compile())
					: localList.OrderByDescending(order.Compile());

			return this;
		}

		public IQuery<T, I> OrderByParent<O>(Expression<Func<T, O>> order, bool? ascending)
		{
			localList =
				ascending ?? false
					? localList.OrderBy(order.Compile())
					: localList.OrderByDescending(order.Compile());

			return this;
		}

		public IQuery<T, I> Take(Int32 topItems)
		{
			localList = localList.Take(topItems);

			return this;
		}

		public bool Any()
		{
			return localList.Any();
		}

		public IQuery<T, I> Page(ISearch search)
		{
			var skip = (search.Page - 1) * search.ItemsPerPage;

			localList = localList
				.Skip(skip)
				.Take(search.ItemsPerPage);

			return this;
		}

		public IQuery<T, I> DistinctMainEntity()
		{
			localList = localList.Distinct();

			return this;
		}

		public IQuery<T, I> TransformResult<D, P, G, S>(
			IList<G> groupProperties,
			IList<S> summarizeProperties
		)
			where D : new() 
			where G : GroupBy<T, I, D, P> 
			where S : Summarize<T, I, D, P>
		{
			summarizedList = new List<object>();

			var grouped = groupItems
				<D, P, G>
				(groupProperties);

			foreach (var item in grouped)
			{
				var destiny = castToDestiny
					<D, P, S>
					(summarizeProperties, item);

				summarizedList.Add(destiny);
			}

			localList = null;

			return this;
		}

		private IDictionary<IDictionary<String, P>, IEnumerable<T>> groupItems
			<D, P, G>
			(IList<G> groupProperties)
			where D : new()
			where G : GroupBy<T, I, D, P>
		{
			return localList
				.Select(
					entity => new
					{
						entity,
						props = getProps<D, P, G>(entity, groupProperties)
					}
				)
				.GroupBy(compose => compose.props)
				.ToDictionary(
					compose => compose.Key,
					compose => compose.Select(c => c.entity)
				);
		}


		private IDictionary<String, P> getProps<D, P, G>(
			T entity,
			IList<G> groupProperties
		)
			where D : new()
			where G : GroupBy<T, I, D, P>
		{
			return groupProperties
				.ToDictionary(
					g => g.Origin,
					g => g.OriginFunc.Compile()(entity)
				);
		}

		private D castToDestiny<D, P, S>(
			IList<S> summarizeProperties,
			KeyValuePair<IDictionary<String, P>, IEnumerable<T>> item
		)
			where D : new()
			where S : Summarize<T, I, D, P>
		{
			var destiny = new D();
			var type = typeof(D);

			foreach (var prop in item.Key)
			{
				var field = type.GetProperty(prop.Key);
				field?.SetValue(destiny, prop.Value);
			}

			foreach (var summary in summarizeProperties)
			{
				summarize(item, summary, destiny);
			}

			return destiny;
		}

		private void summarize<D, P, S>
			(KeyValuePair<IDictionary<String, P>, IEnumerable<T>> item,
			S summary,
			D destiny
		)
			where D : new()
			where S : Summarize<T, I, D, P>
		{
			var destinyField = typeof(D)
				.GetProperty(summary.Destiny);

			var valuesList = item.Value
				.Select(summary.OriginFunc.Compile())
				.Cast<Decimal>();

			var value = summarize(valuesList, summary.Type);

			destinyField?.SetValue(destiny, value);
		}

		private object summarize(IEnumerable<Decimal> list, SummarizeType summaryType)
		{
			switch (summaryType)
			{
				case SummarizeType.Count:
					return list.Count();
				case SummarizeType.Max:
					return list.Max();
				case SummarizeType.Sum:
					return list.Sum();
				default:
					throw new NotImplementedException();
			}
		}

		public IQuery<T, I> TransformResult<D, P, G>(
			IList<G> groupProperties
		)
			where D : new()
			where G : GroupBy<T, I, D, P>
		{
			TransformResult<D, P, G, Summarize<T, I, D, P>>(
				groupProperties,
				new List<Summarize<T, I, D, P>>()
			);

			return this;
		}

		public int Count
		{
			get
			{
				if (summarizedList.Any())
					return summarizedList.Count;

				return localList.Count();
			}
		}

		public IList<T> Result
		{
			get
			{
				failIfSummarized();

				return localList.ToList();
			}
		}

		public T UniqueResult
		{
			get
			{
				failIfSummarized();

				return localList.SingleOrDefault();
			}
		}

		public T FirstOrDefault
		{
			get
			{
				failIfSummarized();

				return localList.FirstOrDefault();
			}
		}

		public IList<TResult> ResultAs<TResult>()
		{
			return summarizedList
				.Cast<TResult>()
				.ToList();
		}

		public Int32 SumInt(Expression<Func<T, object>> property)
		{
			failIfSummarized();
			return localList
				.Select(property.Compile())
				.Cast<Int32>()
				.Sum();
		}

		public Int64 SumLong(Expression<Func<T, object>> property)
		{
			failIfSummarized();
			return localList
				.Select(property.Compile())
				.Cast<Int64>()
				.Sum();
		}

		public Decimal SumDecimal(Expression<Func<T, object>> property)
		{
			failIfSummarized();
			return localList
				.Select(property.Compile())
				.Cast<Decimal>()
				.Sum();
		}

		private void failIfSummarized()
		{
			if (summarizedList.Any())
			{
				throw new ActionNotSupportedException(
					"Use ResultAs to get summarized list"
				);
			}
		}
	}
}
