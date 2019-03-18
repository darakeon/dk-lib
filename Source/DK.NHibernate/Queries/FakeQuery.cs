using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using DK.Generic.DB;
using DK.NHibernate.Base;
using Decimal = System.Decimal;

namespace DK.NHibernate.Queries
{
	internal class FakeQuery<T> : IQuery<T>
		where T : class, IEntity, new()
	{
		private IEnumerable<T> localList;
		private IList<object> summarizedList;

		public FakeQuery(IDictionary<Int32, T> list)
		{
			localList = list.Values.ToList();
			summarizedList = new List<object>();
		}

		public IQuery<T> SimpleFilter(Expression<Func<T, bool>> where)
		{
			localList = localList.Where(where.Compile());
			return this;
		}

		public IQuery<T> SimpleFilter<TEntity>(Expression<Func<T, TEntity>> entityRelation, Expression<Func<TEntity, bool>> where)
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

		public IQuery<T> SimpleFilter<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation, Expression<Func<TEntity, bool>> where)
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

		public IQuery<T> InCondition<TEntity>(Expression<Func<T, TEntity>> property, IList<TEntity> contains)
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

		public IQuery<T> IsNotEmpty<TL>(Expression<Func<T, IList<TL>>> listProperty)
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

		public IQuery<T> IsEmpty<TL>(Expression<Func<T, IList<TL>>> listProperty)
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

		public IQuery<T> LikeCondition(Expression<Func<T, object>> property, String term, LikeType likeType = LikeType.Both)
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

		public IQuery<T> LikeCondition<TAscending>(
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

		public IQuery<T> LikeCondition(IList<SearchItem<T>> searchTerms)
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

		public IQuery<T> LeftJoin<TEntity>(Expression<Func<T, TEntity>> entityRelation)
		{
			return this;
		}

		public IQuery<T> LeftJoin<TEntity>(Expression<Func<T, IList<TEntity>>> entityRelation)
		{
			return this;
		}

		public IQuery<T> FetchModeEager<TEntity>(Expression<Func<T, IList<TEntity>>> listProperty)
		{
			return this;
		}

		public IQuery<T> HasFlag<TEnum>(Expression<Func<T, TEnum>> func, TEnum value) where TEnum : struct, IConvertible
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

		public IQuery<T> OrderBy<TPropOrder>(Expression<Func<T, TPropOrder>> order, bool? ascending)
		{
			localList =
				ascending ?? false
					? localList.OrderBy(order.Compile())
					: localList.OrderByDescending(order.Compile());

			return this;
		}

		public IQuery<T> OrderByParent<TPropOrder>(Expression<Func<T, TPropOrder>> order, bool? ascending)
		{
			localList =
				ascending ?? false
					? localList.OrderBy(order.Compile())
					: localList.OrderByDescending(order.Compile());

			return this;
		}

		public IQuery<T> Take(Int32 topItems)
		{
			localList = localList.Take(topItems);

			return this;
		}

		public bool Any()
		{
			return localList.Any();
		}

		public IQuery<T> Page(ISearch search)
		{
			var skip = (search.Page - 1) * search.ItemsPerPage;

			localList = localList
				.Skip(skip)
				.Take(search.ItemsPerPage);

			return this;
		}

		public IQuery<T> DistinctMainEntity()
		{
			localList = localList.Distinct();

			return this;
		}

		public IQuery<T> TransformResult<TDestiny, TProp, TGroupBy, TSummarize>(
			IList<TGroupBy> groupProperties,
			IList<TSummarize> summarizeProperties
		)
			where TDestiny : new() 
			where TGroupBy : GroupBy<T, TDestiny, TProp> 
			where TSummarize : Summarize<T, TDestiny, TProp>
		{
			summarizedList = new List<object>();

			var grouped = groupItems
				<TDestiny, TProp, TGroupBy>
				(groupProperties);

			foreach (var item in grouped)
			{
				var destiny = castToDestiny
					<TDestiny, TProp, TSummarize>
					(summarizeProperties, item);

				summarizedList.Add(destiny);
			}

			localList = null;

			return this;
		}

		private IDictionary<IDictionary<String, TProp>, IEnumerable<T>> groupItems
			<TDestiny, TProp, TGroupBy>
			(IList<TGroupBy> groupProperties) where TDestiny : new()
			where TGroupBy : GroupBy<T, TDestiny, TProp>
		{
			return localList
				.Select(
					entity => new
					{
						entity,
						props = getProps
							<TDestiny, TProp, TGroupBy>
							(entity, groupProperties)
					}
				)
				.GroupBy(compose => compose.props)
				.ToDictionary(
					compose => compose.Key,
					compose => compose.Select(c => c.entity)
				);
		}


		private IDictionary<String, TProp> getProps<TDestiny, TProp, TGroupBy>(
			T entity,
			IList<TGroupBy> groupProperties
		)
			where TDestiny : new()
			where TGroupBy : GroupBy<T, TDestiny, TProp>
		{
			return groupProperties
				.ToDictionary(
					g => g.Origin,
					g => g.OriginFunc.Compile()(entity)
				);
		}

		private TDestiny castToDestiny<TDestiny, TProp, TSummarize>(
			IList<TSummarize> summarizeProperties,
			KeyValuePair<IDictionary<String, TProp>, IEnumerable<T>> item
		)
			where TDestiny : new() where TSummarize : Summarize<T, TDestiny, TProp>
		{
			var destiny = new TDestiny();
			var type = typeof(TDestiny);

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

		private void summarize<TDestiny, TProp, TSummarize>
			(KeyValuePair<IDictionary<String, TProp>, IEnumerable<T>> item,
			TSummarize summary,
			TDestiny destiny
		)
			where TDestiny : new()
			where TSummarize : Summarize<T, TDestiny, TProp>
		{
			var destinyField = typeof(TDestiny)
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

		public IQuery<T> TransformResult<TDestiny, TProp, TGroupBy>(
			IList<TGroupBy> groupProperties
		)
			where TDestiny : new()
			where TGroupBy : GroupBy<T, TDestiny, TProp>
		{
			TransformResult<
				TDestiny,
				TProp,
				TGroupBy,
				Summarize<T, TDestiny, TProp>
			>(
				groupProperties,
				new List<Summarize<T, TDestiny, TProp>>()
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

				return localList.SingleOrDefault();
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
