using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ak.Generic.Reflection;
using NHibernate;
using NHibernate.Criterion;

namespace Ak.NHibernate.Base
{
    /// <summary>
    /// Object to handle database fluently
    /// </summary>
    /// <typeparam name="T">Entity Type</typeparam>
    public class Query<T> where T : class
    {
        private ICriteria criteria;

        /// <summary></summary>
        /// <param name="session">NH Session</param>
        public Query(ISession session)
        {
            criteria = session.CreateCriteria<T>();
        }


        /// <summary>
        /// Make a filter with lambda expression
        /// </summary>
        /// <param name="where">Lambda expression</param>
        public Query<T> Filter(Expression<Func<T, Boolean>> where)
        {
            criteria = criteria.Add(Restrictions.Where(where));

            return this;
        }
		
		/// <summary>
		/// To make a filter with ascending entities
		/// </summary>
		/// <typeparam name="TFinalEntity">The entity with the property to be compared</typeparam>
		/// <param name="where">Lambda for filter</param>
		/// <param name="levels">All the entities between current and final</param>
		/// <returns></returns>
		public Query<T> FilterWithAscending<TFinalEntity>(Expression<Func<TFinalEntity, Boolean>> where, params Type[] levels)
		{
			var newCriteria = criteria;

			foreach (var level in levels)
			{
				newCriteria = newCriteria.CreateCriteria(level.Name);
			}

			newCriteria = newCriteria.CreateCriteria(typeof(TFinalEntity).Name);

			newCriteria.Add(Restrictions.Where(where));

			return this;
		}



        /// <summary>
        /// Search for text inside entity property values
        /// </summary>
		/// <param name="property">Lambda of property</param>
		/// <param name="term">Text to search</param>
        public Query<T> LikeCondition(Expression<Func<T, object>> property, String term)
        {
            var searchTerms = new List<SearchItem<T>>
            {
                new SearchItem<T>(property, term)
            };

            criteria = criteria.Add(accumulateLikeOr(searchTerms));

            return this;
        }

		/// <summary>
		/// Search for text inside entity property values
		/// </summary>
		/// <param name="searchTerms">Fields and texts to search</param>
		public Query<T> LikeCondition(IList<SearchItem<T>> searchTerms)
        {
            criteria = criteria.Add(accumulateLikeOr(searchTerms));

            foreach (var searchTerm in searchTerms)
            {
                if (searchTerm.ParentType() != typeof (T))
                {
                    var typeName = searchTerm.ParentType().Name;

                    criteria.CreateAlias(typeName, typeName, CriteriaSpecification.InnerJoin);
                }
            }

            return this;
        }

        private AbstractCriterion accumulateLikeOr(IList<SearchItem<T>> searchTerms)
        {
            if (!searchTerms.Any())
                return null;

            var searchTerm = searchTerms.First();
            var restriction = Restrictions.On(searchTerm.Property).IsLike("%" + searchTerm.Term + "%");

            if (searchTerms.Count() == 1)
            {
                return restriction;
            }

            var otherTerms = searchTerms.Skip(1).ToList();
            var otherProcessedTerms = accumulateLikeOr(otherTerms);

            return Restrictions.Or(restriction, otherProcessedTerms);
        }

        

        /// <summary>
        /// Reordering results
        /// </summary>
        /// <param name="order">Property to order</param>
        /// <param name="ascending">Whether the order is ascending (true) or descending (false)</param>
        public Query<T> OrderBy<TPropOrder>(Expression<Func<T, TPropOrder>> order, Boolean? ascending = true)
        {
            var propName = order.GetName();
            var orderBy = ascending.HasValue && ascending.Value
                ? Order.Asc(propName) : Order.Desc(propName);

            criteria = criteria.AddOrder(orderBy);

            return this;
        }


        /// <summary>
        /// To get a page of the results
        /// </summary>
        /// <param name="itemsPerPage">Amount of items in each page</param>
        /// <param name="page">Page to get</param>
        public Query<T> Page(Int32? itemsPerPage, Int32? page = 1)
        {
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
        /// Execute the query, getting just the amount of items
        /// </summary>
        public Int32 Count
        {
            get
            {
                return criteria
                    .SetProjection(Projections.Count(Projections.Id()))
                    .List<Int32>()
                    .Sum();
            }
        }

		/// <summary>
		/// Execute the query, getting all the results
		/// </summary>
		public IList<T> Result
        {
            get { return criteria.List<T>(); }
        }

		/// <summary>
		/// Execute the query, return just one result
		/// </summary>
		/// <exception cref="NonUniqueResultException">If there is more than one result from constructed query</exception>
		public T UniqueResult
        {
            get { return criteria.UniqueResult<T>(); }
        }
		
		/// <summary>
		/// Return first result of list, or null if list is empty
		/// </summary>
		public T FirstOrDefault
		{
			get { return Page(1).UniqueResult; }
		}


    }
}