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
		private const JoinType defaultJoinType = JoinType.InnerJoin;

		internal static ICriteria PropertyCriteria<Entity, Prop>(
			this ICriteria criteria,
			Expression<Func<Entity, Prop>> property,
			JoinType joinType = defaultJoinType
		)
		{
			return criteria.childCriteria(
				property.NormalizePropertyName(),
				joinType,
				true
			);
		}

		internal static ICriteria RelationCriteria<Entity, Prop>(
			this ICriteria criteria,
			Expression<Func<Entity, Prop>> property,
			JoinType joinType = defaultJoinType
		)
		{
			return criteria.childCriteria(
				property.NormalizePropertyName(),
				joinType,
				false
			);
		}

		internal static ICriteria RelationCriteria<Entity, PropItem>(
			this ICriteria criteria,
			Expression<Func<Entity, IList<PropItem>>> property,
			JoinType joinType = defaultJoinType
		)
		{
			return criteria.childCriteria(
				property.NormalizePropertyName(),
				joinType,
				false
			);
		}

		private static ICriteria childCriteria(
			this ICriteria criteria, IEnumerable<String> ascendants,
			JoinType joinType, Boolean skipLast
		)
		{
			var newCriteria = criteria;

			var list = ascendants.ToList();

			if (skipLast && list.Any())
				list = list.Take(list.Count - 1).ToList();

			for (var i = 0; i < list.Count; i++)
			{
				var name = list[i];
				var alias = String.Join(".", list.Take(i + 1));
				newCriteria = childCriteria(
					newCriteria, name, alias, joinType
				);
			}

			return newCriteria;
		}

		private static ICriteria childCriteria(
			this ICriteria criteria, string name, string alias, JoinType joinType
		)
		{
			return criteria.GetCriteriaByPath(name)
				?? criteria.GetCriteriaByAlias(alias)
				?? criteria.CreateCriteria(name, alias, joinType);
		}
	}
}
