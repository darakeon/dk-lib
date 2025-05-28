using System;
using NHibernate;

namespace Keon.NHibernate.Logging;

class NoLogger : INHibernateLogger
{
	public static readonly NoLogger Instance = new();

	public void Log(NHibernateLogLevel level, NHibernateLogValues state, Exception exception)
	{
	}

	public Boolean IsEnabled(NHibernateLogLevel level)
	{
		return false;
	}
}
