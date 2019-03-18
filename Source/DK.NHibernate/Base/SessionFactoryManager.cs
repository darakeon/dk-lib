using System;
using System.Configuration;
using DK.Generic.DB;
using DK.Generic.Exceptions;
using DK.Generic.Extensions;
using DK.NHibernate.Helpers;
using DK.NHibernate.UserPassed;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;

namespace DK.NHibernate.Base
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
		/// Use in-memory data with lists, for tests
		/// </summary>
		public static Boolean FakeDB { get; set; }

		/// <summary>
		/// Initialize function, need to be called before use instance
		/// </summary>
		/// <param name="dbInitializer">Object to pre-populate DB</param>
		public static void Initialize<TMap, TEntity>(IDataInitializer dbInitializer = null) 
			where TMap : IAutoMappingOverride<TEntity>
		{
			if (Instance != null)
				return;

			FakeDB = (
				ConfigurationManager.AppSettings["FakeDB"] ?? "false"
			).ToLower() == "true";

			if (FakeDB) return;

			var mapInfo =
				new AutoMappingInfo<TMap, TEntity>
				{
					BaseEntities = new [] { typeof (IEntity) }
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
			if (FakeDB) return null;

			if (Instance == null)
				throw new DKException("Restart the Application.");

			return Instance.OpenSession();
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