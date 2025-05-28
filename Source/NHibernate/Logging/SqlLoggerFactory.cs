using System;
using NHibernate;

namespace Keon.NHibernate.Logging;

internal class SqlLoggerFactory : INHibernateLoggerFactory
{
	public static void Config(Action<String> log)
	{
		var logger = new SqlLoggerFactory(log);
		NHibernateLogger.SetLoggersFactory(logger);
	}

	private const String logName = "NHibernate.SQL";

	private SqlLoggerFactory(Action<String> log)
	{
		this.log = log;
	}

	private readonly Action<String> log;

	public INHibernateLogger LoggerFor(String keyName)
	{
		return keyName == logName
			? new SqlLogger(log)
			: NoLogger.Instance;
	}

	public INHibernateLogger LoggerFor(Type type)
	{
		return NoLogger.Instance;
	}
}
