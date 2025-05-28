using System;
using System.Linq;
using NHibernate;

namespace Keon.NHibernate.Logging;

class SqlLogger(Action<String> log) : INHibernateLogger
{
	public void Log(NHibernateLogLevel level, NHibernateLogValues state, Exception exception)
	{
		var query = state.ToString();
		if (query != null)
		{
			var queries = query.Split(";");
			var safeQueries = queries[..^1];
			var safeQuery = String.Join(';', safeQueries);
			log(safeQuery);
		}

		if (exception != null)
			log(exception.ToString());
	}

	public Boolean IsEnabled(NHibernateLogLevel level)
	{
		return true;
	}
}
