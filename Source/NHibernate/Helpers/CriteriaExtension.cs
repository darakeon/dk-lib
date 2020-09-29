using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Keon.Util.Reflection;
using NHibernate;
using NHibernate.SqlCommand;

namespace Keon.NHibernate.Helpers
{
	internal static class CriteriaExtension
	{
		private const JoinType default_join_type = JoinType.InnerJoin;



		internal static ICriteria GetOrCreatePropertyCriteria<T, TEntity>(this ICriteria criteria, Expression<Func<T, TEntity>> property, JoinType joinType = default_join_type)
		{
			return getOrCreateCriteria(criteria, property.NormalizePropertyName(), joinType);
		}

		
		internal static ICriteria GetOrCreateRelationCriteria<T, TEntity>(this ICriteria criteria, Expression<Func<T, TEntity>> property, JoinType joinType = default_join_type)
		{
			return getOrCreateCriteria(criteria, property.NormalizePropertyName(), joinType, false);
		}
		
		internal static ICriteria GetOrCreateRelationCriteria<T, TL>(this ICriteria criteria, Expression<Func<T, IList<TL>>> property, JoinType joinType = default_join_type)
		{
			return getOrCreateCriteria(criteria, property.NormalizePropertyName(), joinType, false);
		}




		private static ICriteria getOrCreateCriteria(this ICriteria criteria, IEnumerable<String> ascendings, JoinType joinType = default_join_type, Boolean skipLast = true)
		{
			var newCriteria = criteria;

			var list = ascendings.ToList();

			if (skipLast && list.Any())
				list = list.Take(list.Count - 1).ToList();

			for (var i = 0; i < list.Count; i++)
			{
				var name = list[i];
				var alias = String.Join(".", list.Take(i + 1));
				newCriteria = getOrCreateCriteria(newCriteria, name, alias, joinType);
			}

			return newCriteria;
		}
		private static ICriteria getOrCreateCriteria(this ICriteria criteria, string name, string alias, JoinType joinType)
		{
			return criteria.GetCriteriaByPath(name)
				   ?? criteria.GetCriteriaByAlias(alias)
				   ?? criteria.CreateCriteria(name, alias, joinType);
		}
	}
}
