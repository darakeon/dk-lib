using System;
using FluentNHibernate.Automapping.Alterations;
using Keon.NHibernate.UserPassed;
using Keon.Util.DB;
using Keon.Util.Exceptions;
using Keon.Util.Extensions;
using Microsoft.Extensions.Configuration;
using NHibernate;

namespace Keon.NHibernate.Schema
{
	/// <summary>
	/// Manager of Build of Sessions
	/// </summary>
	public sealed class SessionFactoryManager
	{
		/// <summary>
		/// Singleton
		/// </summary>
		public static ISessionFactory Instance { get; private set; }

		/// <summary>
		/// Initialize function, need to be called before use instance
		/// </summary>
		/// <param name="config">Dictionary of application configuration</param>
		public static void Initialize<TMap, TEntity>(IConfiguration config)
			where TMap : IAutoMappingOverride<TEntity>
		{
			Initialize<TMap, TEntity>(config, null);
		}

		/// <summary>
		/// Initialize function, need to be called before use instance
		/// </summary>
		/// <param name="config">Dictionary of application configuration</param>
		/// <param name="dbInitializer">Object to pre-populate DB</param>
		public static void Initialize<TMap, TEntity>(
			IConfiguration config,
			IDataInitializer dbInitializer
		)
			where TMap : IAutoMappingOverride<TEntity>
		{
			if (Instance != null)
				return;

			var mapInfo =
				new AutoMappingInfo<TMap, TEntity>
				{
					BaseEntities = new [] { typeof (IEntity<>) }
				};

			var dbAction = getDBAction(config, dbInitializer);

			Instance = SessionFactoryBuilder.Start(config, mapInfo, dbAction);

			if (dbInitializer != null && dbAction == DBAction.Recreate)
				dbInitializer.PopulateDB();
		}

		private static DBAction getDBAction(IConfiguration config, IDataInitializer dbInitializer)
		{
			try
			{
				var dbActionConfig = config["DBAction"];
				return dbActionConfig.Cast<DBAction>();
			}
			catch (Exception)
			{
				return dbInitializer?.DBAction ?? DBAction.Validate;
			}
		}

		internal static ISession OpenSession()
		{
			if (Instance == null)
				throw new DKException("Restart the Application.");

			var session = Instance.OpenSession();

			session.FlushMode = FlushMode.Commit;

			return session;
		}

		/// <summary>
		/// Finishes SessionFactory
		/// </summary>
		public static void End()
		{
			Instance?.Close();
		}
	}
}
