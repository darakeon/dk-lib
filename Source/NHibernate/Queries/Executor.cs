using System.Collections.Generic;
using NHibernate;

namespace Keon.NHibernate.Queries
{
	/// <summary>
	/// Centralize common final executions of Query and Transformer
	/// </summary>
	public abstract class Executor<Result>
	{
		private protected ICriteria criteria;

		private protected virtual void executeBeforeEnd() { }

		/// <summary>
		/// Execute the query, getting all the results
		/// </summary>
		public IList<Result> List
		{
			get
			{
				executeBeforeEnd();
				return criteria.List<Result>();
			}
		}

		/// <summary>
		/// Execute the query, return just one result
		/// </summary>
		public Result SingleOrDefault
		{
			get
			{
				executeBeforeEnd();
				return criteria.UniqueResult<Result>();
			}
		}
	}
}
