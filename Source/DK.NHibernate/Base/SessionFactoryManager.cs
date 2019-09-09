using System;
using System.Configuration;
using FluentNHibernate.Automapping.Alterations;
using Keon.NHibernate.Fakes;
using Keon.NHibernate.Helpers;
using Keon.NHibernate.UserPassed;
using Keon.Util.DB;
using Keon.Util.Exceptions;
using Keon.Util.Extensions;
using NHibernate;

namespace Keon.NHibernate.Base
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
		public static void Initialize<TMap, TEntity>()
			where TMap : IAutoMappingOverride<TEntity>
		{
			Initialize<TMap, TEntity>(null);
		}

		/// <summary>
		/// Initialize function, need to be called before use instance
		/// </summary>
		/// <param name="dbInitializer">Object to pre-populate DB</param>
		public static void Initialize<TMap, TEntity>(
			IDataInitializer dbInitializer
		)
			where TMap : IAutoMappingOverride<TEntity>
		{
			if (Instance != null)
				return;

			FakeHelper.IsFake = (
				ConfigurationManager.AppSettings["FakeDB"] ?? "false"
			).ToLower() == "true";

			if (FakeHelper.IsFake) return;

			var mapInfo =
				new AutoMappingInfo<TMap, TEntity>
				{
					BaseEntities = new [] { typeof (IEntity<>) }
				};

			var dbAction = getDBAction(dbInitializer);

			Instance = SessionFactoryBuilder.Start(mapInfo, dbAction);

			if (dbInitializer != null && dbAction == DBAction.Recreate)
				dbInitializer.PopulateDB();
		}

		private static DBAction getDBAction(IDataInitializer dbInitializer)
		{
			try
			{
				var dbActionConfig = ConfigurationManager.AppSettings["DBAction"];
				return dbActionConfig.Cast<DBAction>();
			}
			catch (Exception)
			{
				return dbInitializer?.DBAction ?? DBAction.Validate;
			}
		}




		internal static ISession OpenSession()
		{
			if (FakeHelper.IsFake)
				return null;

			if (Instance == null)
				throw new DKException("Restart the Application.");

			var session = Instance.OpenSession();

			session.FlushMode = FlushMode.Manual;

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
