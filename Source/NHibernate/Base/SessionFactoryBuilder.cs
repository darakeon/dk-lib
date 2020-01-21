using System;
using System.IO;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using Keon.NHibernate.Helpers;
using Keon.NHibernate.UserPassed;
using Keon.Util.Extensions;
using Microsoft.Extensions.Configuration;
using NHibernate;

namespace Keon.NHibernate.Base
{
    internal class SessionFactoryBuilder
    {
	    /// <summary>
	    ///  Create Session Factory.
	    ///  To be used at Application_Start.
	    /// </summary>
	    ///  <typeparam name="TM">Any mapping class. Passed automatic by passing to AutoMappingInfo.</typeparam>
	    ///  <typeparam name="TE">Any entity class. Passed automatic by passing to AutoMappingInfo.</typeparam>
	    /// <param name="config">Dictionary of application configuration</param>
	    /// <param name="connectionInfo">About database connection</param>
	    /// <param name="autoMappingInfo">About mappings on the entities</param>
	    /// <param name="dbAction"></param>
	    private static ISessionFactory start<TM, TE>(IConfiguration config, ConnectionInfo connectionInfo, AutoMappingInfo<TM, TE> autoMappingInfo, DBAction dbAction)
			where TM : IAutoMappingOverride<TE>
		{
			// ReSharper disable once InvertIf
			if (connectionInfo == null)
			{
				var scriptFileFullName = config["ScriptFileFullName"];

				if (scriptFileFullName != null && scriptFileFullName.ToLower() == "current")
				{
					scriptFileFullName = Directory.GetCurrentDirectory();
					var filename = DateTime.Now.ToString(@"Ba\seyyyyMMddhhmmss");

					scriptFileFullName = Path.Combine(scriptFileFullName, filename);
				}

				connectionInfo = new ConnectionInfo
				{
					DBMS = config["DBMS"].Cast<DBMS>(),
					ScriptFileFullName = scriptFileFullName,
					ShowSQL = (config["ShowSQL"] ?? "false").ToLower() == "true",
				};

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

			return createSessionFactory(config, connectionInfo, autoMappingInfo, dbAction);
		}

		/// <summary>
		/// Create Session Factory, using the AppSettings.
		/// The keys required are the ConnectionInfo class properties.
		/// To be used at Application_Start.
		/// </summary>
		/// <typeparam name="TM">Any mapping class. Passed automatic by passing to AutoMappingInfo.</typeparam>
		/// <typeparam name="TE">Any entity class. Passed automatic by passing to AutoMappingInfo.</typeparam>
		/// <param name="config">Dictionary of application configuration</param>
		/// <param name="autoMappingInfo">About mappings on the entities</param>
		/// <param name="dbAction">Action into DB when start project</param>
		internal static ISessionFactory Start<TM, TE>(IConfiguration config, AutoMappingInfo<TM, TE> autoMappingInfo, DBAction dbAction)
			where TM : IAutoMappingOverride<TE>
		{
			return start(config, null, autoMappingInfo, dbAction);
		}

		private static ISessionFactory createSessionFactory<TM, TE>(IConfiguration config, ConnectionInfo connectionInfo, AutoMappingInfo<TM, TE> autoMappingInfo, DBAction dbAction)
            where TM : IAutoMappingOverride<TE>
        {
            var schemaChanger = new SchemaChanger(connectionInfo.ScriptFileFullName);

            var autoMapping = autoMappingInfo.CreateAutoMapping();

            var fluent = Fluently.Configure()
                .Database(connectionInfo.ConfigureDataBase())
                .Mappings(m => m.AutoMappings.Add(autoMapping));

			if (config["NHibernateProfiler-enable"] == "true")
			{
				fluent.ExposeConfiguration(c => c.SetProperty("generate_statistics", "true"));
			}

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

            return fluent.BuildSessionFactory();
        }
    }
}
