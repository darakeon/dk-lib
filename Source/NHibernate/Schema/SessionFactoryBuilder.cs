﻿using System;
using System.IO;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using Keon.NHibernate.UserPassed;
using Keon.Util.Extensions;
using Microsoft.Extensions.Configuration;
using NHibernate;

namespace Keon.NHibernate.Schema
{
	internal class SessionFactoryBuilder
	{
		/// <summary>
		/// Create Session Factory, using the AppSettings.
		/// The keys required are the ConnectionInfo class properties.
		/// To be used at Application_Start.
		/// </summary>
		/// <typeparam name="Map">Any mapping class. Passed automatic by passing to AutoMappingInfo.</typeparam>
		/// <typeparam name="Entity">Any entity class. Passed automatic by passing to AutoMappingInfo.</typeparam>
		/// <param name="config">Dictionary of application configuration</param>
		/// <param name="autoMappingInfo">About mappings on the entities</param>
		/// <param name="dbAction">Action into DB when start project</param>
		/// <param name="logQueries">Method to log queries if needed</param>
		internal static ISessionFactory Start<Map, Entity>(
			IConfiguration config,
			AutoMappingInfo<Map, Entity> autoMappingInfo,
			DBAction dbAction,
			Action<String> logQueries
		)
			where Map : IAutoMappingOverride<Entity>
		{
			return createSessionFactory(
				config,
				getConnectionInfo(config, logQueries),
				autoMappingInfo,
				dbAction
			);
		}

		private static ConnectionInfo getConnectionInfo(IConfiguration config, Action<String> logQueries)
		{
			var scriptFileFullName = getScriptFileFullName(config);

			var connectionInfo = new ConnectionInfo
			{
				DBMS = config["DBMS"].Cast<DBMS>(),
				ScriptFileFullName = scriptFileFullName,
				LogQueries = logQueries,
				ShowSQL = (config["ShowSQL"] ?? "false").ToLower() == "true",
			};

			populateConnStr(config, connectionInfo);

			return connectionInfo;
		}

		private static void populateConnStr(IConfiguration config, ConnectionInfo connectionInfo)
		{
			var connStr = config["ConnectionString"];

			if (connStr != null)
			{
				connectionInfo.ConnectionString = connStr;
			}
			else
			{
				connectionInfo.Server = config["Server"];
				connectionInfo.DataBase = config["DataBase"];
				connectionInfo.Login = config["Login"];
				connectionInfo.Password = config["Password"];
			}
		}

		private static String getScriptFileFullName(IConfiguration config)
		{
			var scriptFileFullName = config["ScriptFileFullName"];

			if (scriptFileFullName == null)
				return null;

			if (scriptFileFullName.ToLower() != "current")
				return scriptFileFullName;

			scriptFileFullName = Directory.GetCurrentDirectory();
			// ReSharper disable StringLiteralTypo
			const String dateFormat = @"Ba\seyyyyMMddhhmmss";
			// ReSharper restore StringLiteralTypo
			var filename = DateTime.Now.ToString(dateFormat);

			scriptFileFullName = Path.Combine(scriptFileFullName, filename);

			return scriptFileFullName;
		}

		private static ISessionFactory createSessionFactory<Map, Entity>(
			IConfiguration config, ConnectionInfo connectionInfo,
			AutoMappingInfo<Map, Entity> autoMappingInfo,
			DBAction dbAction
		)
			where Map : IAutoMappingOverride<Entity>
		{
			var schemaChanger = new SchemaChanger(
				connectionInfo.ScriptFileFullName,
				connectionInfo.LogQueries
			);

			var autoMapping = autoMappingInfo.CreateAutoMapping();

			var fluent = Fluently.Configure()
				.Database(connectionInfo.ConfigureDataBase())
				.Mappings(m => m.AutoMappings.Add(autoMapping));

			if (config["NHibernateProfiler-enable"] == "true")
			{
				fluent.ExposeConfiguration(
					c => c.SetProperty("generate_statistics", "true")
				);
			}

			fluent = setDbAction(dbAction, fluent, schemaChanger);

			return fluent.BuildSessionFactory();
		}

		private static FluentConfiguration setDbAction(
			DBAction dbAction,
			FluentConfiguration fluent,
			SchemaChanger schemaChanger
		)
		{
			switch (dbAction)
			{
				case DBAction.Recreate:
					fluent = fluent.ExposeConfiguration(schemaChanger.Build);
					break;

				case DBAction.Update:
					fluent = fluent.ExposeConfiguration(schemaChanger.Update);
					break;

				case DBAction.Validate:
					fluent = fluent.ExposeConfiguration(schemaChanger.Validate);
					break;
				case DBAction.None:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(dbAction), dbAction, null);
			}

			return fluent;
		}
	}
}
