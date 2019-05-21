using System;
using System.Configuration;
using System.IO;
using DK.NHibernate.Helpers;
using DK.NHibernate.UserPassed;
using DK.Util.Extensions;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using NHibernate;

namespace DK.NHibernate.Base
{
    internal class SessionFactoryBuilder
    {
	    /// <summary>
		///  Create Session Factory.
		///  To be used at Application_Start.
		/// </summary>
		///  <typeparam name="TM">Any mapping class. Passed automatic by passing to AutoMappingInfo.</typeparam>
		///  <typeparam name="TE">Any entity class. Passed automatic by passing to AutoMappingInfo.</typeparam>
		/// <param name="connectionInfo">About database connection</param>
		/// <param name="autoMappingInfo">About mappings on the entities</param>
		/// <param name="dbAction"></param>
		private static ISessionFactory start<TM, TE>(ConnectionInfo connectionInfo, AutoMappingInfo<TM, TE> autoMappingInfo, DBAction dbAction)
			where TM : IAutoMappingOverride<TE>
		{
			// ReSharper disable once InvertIf
			if (connectionInfo == null)
			{
				var scriptFileFullName = ConfigurationManager.AppSettings["ScriptFileFullName"];

				if (scriptFileFullName != null && scriptFileFullName.ToLower() == "current")
				{
					scriptFileFullName = Directory.GetCurrentDirectory();
					var filename = DateTime.Now.ToString(@"Ba\seyyyyMMddhhmmss");

					scriptFileFullName = Path.Combine(scriptFileFullName, filename);
				}

				connectionInfo = new ConnectionInfo
				{
					DBMS = ConfigurationManager.AppSettings["DBMS"].Cast<DBMS>(),
					ScriptFileFullName = scriptFileFullName,
					ShowSQL = (ConfigurationManager.AppSettings["ShowSQL"] ?? "false").ToLower() == "true",
				};

				var connStr = ConfigurationManager.ConnectionStrings["ConnectionString"];

				if (connStr != null)
				{
					connectionInfo.ConnectionString = connStr.ConnectionString;
				}
				else
				{
					connectionInfo.Server = ConfigurationManager.AppSettings["Server"];
					connectionInfo.DataBase = ConfigurationManager.AppSettings["DataBase"];
					connectionInfo.Login = ConfigurationManager.AppSettings["Login"];
					connectionInfo.Password = ConfigurationManager.AppSettings["Password"];
				}
			}

			return createSessionFactory(connectionInfo, autoMappingInfo, dbAction);
		}

		/// <summary>
		/// Create Session Factory, using the AppSettings.
		/// The keys required are the ConnectionInfo class properties.
		/// To be used at Application_Start.
		/// </summary>
		/// <typeparam name="TM">Any mapping class. Passed automatic by passing to AutoMappingInfo.</typeparam>
		/// <typeparam name="TE">Any entity class. Passed automatic by passing to AutoMappingInfo.</typeparam>
		/// <param name="autoMappingInfo">About mappings on the entities</param>
		/// <param name="dbAction">Action into DB when start project</param>
		internal static ISessionFactory Start<TM, TE>(AutoMappingInfo<TM, TE> autoMappingInfo, DBAction dbAction)
			where TM : IAutoMappingOverride<TE>
		{
			return start(null, autoMappingInfo, dbAction);
		}

		private static ISessionFactory createSessionFactory<TM, TE>(ConnectionInfo connectionInfo, AutoMappingInfo<TM, TE> autoMappingInfo, DBAction dbAction)
            where TM : IAutoMappingOverride<TE>
        {
            var schemaChanger = new SchemaChanger(connectionInfo.ScriptFileFullName);

            var automapping = autoMappingInfo.CreateAutoMapping();

            var config = Fluently.Configure()
                .Database(connectionInfo.ConfigureDataBase())
                .Mappings(m => m.AutoMappings.Add(automapping));

			if (ConfigurationManager.AppSettings["NHibernateProfiler-enable"] == "true")
			{
				config.ExposeConfiguration(c => c.SetProperty("generate_statistics", "true"));
			}

			switch (dbAction)
            {
                case DBAction.Recreate:
                    config = config.ExposeConfiguration(schemaChanger.Build);
		            break;

                case DBAction.Update:
                    config = config.ExposeConfiguration(schemaChanger.Update);
                    break;

                case DBAction.Validate:
                    config = config.ExposeConfiguration(schemaChanger.Validate);
                    break;
	            case DBAction.None:
		            break;
	            default:
		            throw new ArgumentOutOfRangeException(nameof(dbAction), dbAction, null);
            }

            return config.BuildSessionFactory();
        }

    }
}
