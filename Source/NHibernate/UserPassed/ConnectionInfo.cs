using System;
using FluentNHibernate.Cfg.Db;
using Keon.NHibernate.Schema;
using Keon.Util.Exceptions;

namespace Keon.NHibernate.UserPassed
{
	/// <summary>
	/// Information to Connect to the Database.
	/// </summary>
	public class ConnectionInfo
	{
		/// <summary>
		/// Server Name. No needed for Postgres and SQLite.
		/// </summary>
		public String Server { get; set; }

		/// <summary>
		/// Database name in DBMS. No needed for Oracle and SQLLite.
		/// </summary>
		public String DataBase { get; set; }

		/// <summary>
		/// Login for the DB.
		/// </summary>
		public String Login { get; set; }

		/// <summary>
		/// Password not encrypted for DB.
		/// </summary>
		public String Password { get; set; }

		/// <summary>
		/// Just for SQLite.
		/// </summary>
		public String ConnectionString { get; set; }

		/// <summary>
		/// Database Management System used.
		/// </summary>
		public DBMS DBMS { get; set; }

		/// <summary>
		/// The File Full Name for export Queries
		/// Just fill if see the Queries is needed
		/// </summary>
		public String ScriptFileFullName { get; set; }

		/// <summary>
		/// Log the Queries
		/// </summary>
		public Action<String> LogQueries { get; set; }

		/// <summary>
		/// Whether to show SQL in log
		/// </summary>
		public Boolean ShowSQL { get; set; }

		internal IPersistenceConfigurer ConfigureDataBase()
		{
			switch (DBMS)
			{
				case DBMS.MySQL:
				{
					return configure(
						MySQLConfiguration.Standard,
						c => c.Server(Server)
							.Database(DataBase)
							.Username(Login)
							.Password(Password)
					);
				}

				case DBMS.MsSql7:
				{
					return configure(
						MsSqlConfiguration.MsSql7,
						c => c.Server(Server)
							.Database(DataBase)
							.Username(Login)
							.Password(Password)
					);
				}

				case DBMS.MsSql2008:
				{
					return configure(
						MsSqlConfiguration.MsSql2008,
						c => c.Server(Server)
							.Database(DataBase)
							.Username(Login)
							.Password(Password)
					);
				}

				case DBMS.MsSql2005:
				{
					return configure(
						MsSqlConfiguration.MsSql2005,
						c => c.Server(Server)
							.Database(DataBase)
							.Username(Login)
							.Password(Password)
					);
				}

				case DBMS.MsSql2000:
				{
					return configure(
						MsSqlConfiguration.MsSql2000,
						c => c.Server(Server)
							.Database(DataBase)
							.Username(Login)
							.Password(Password)
					);
				}

				case DBMS.Postgres:
				{
					return configure(
						PostgreSQLConfiguration.Standard,
						c => c.Database(DataBase)
							.Username(Login)
							.Password(Password)
					);
				}

				case DBMS.Oracle9:
				{
					return configure(
						OracleManagedDataClientConfiguration.Oracle9,
						c => c.Server(Server)
							.Username(Login)
							.Password(Password)
					);
				}

				case DBMS.Oracle10:
				{
					return configure(
						OracleManagedDataClientConfiguration.Oracle10,
						c => c.Server(Server)
							.Username(Login)
							.Password(Password)
					);
				}

				case DBMS.SQLite:
				{
					return configure(SQLiteConfiguration.Standard);
				}

				default:
					throw new DKException("Not supported!");
			}
		}

		private IPersistenceConfigurer configure<Config, ConnStr>(
			PersistenceConfiguration<Config, ConnStr> connection,
			Action<ConnStr> config = null
		)
			where Config : PersistenceConfiguration<Config, ConnStr>
			where ConnStr : ConnectionStringBuilder, new()
		{
			var configuration =
				config != null && ConnectionString == null
					? connection.ConnectionString(config)
					: connection.ConnectionString(ConnectionString);

			if (ShowSQL)
				configuration = configuration.ShowSql();

			return configuration;
		}
	}
}
