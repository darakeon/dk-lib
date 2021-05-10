using System;
using System.Collections.Generic;
using Keon.NHibernate.Schema;
using NHibernate;

namespace Keon.NHibernate.Sessions
{
	/// <summary>
	/// Manager for NH Session
	/// </summary>
	public sealed class SessionManager
	{
		/// <summary>
		/// Need to be called before once using SessionManager
		/// </summary>
		public static void Init(GetKey getKey)
		{
			SessionManager.getKey = getKey;
		}

		/// <summary>
		/// Signature to get key of current NH Session
		/// </summary>
		public delegate String GetKey();
		private static event GetKey getKey;

		/// <summary>
		/// Get current NH Session
		/// </summary>
		public static ISession GetCurrent()
		{
			return session ??= SessionFactoryManager.OpenSession();
		}

		/// <summary>
		/// Tells if the Session Transaction have Failed
		/// </summary>
		public static void AddFailed(ISession youHaveFailed)
		{
			failed.Add(youHaveFailed);
		}

		private static readonly IList<ISession> failed = new List<ISession>();

		/// <summary>
		/// Close session
		/// </summary>
		public static void Close()
		{
			if (session == null)
				return;

			if (session.IsOpen)
			{
				if (failed.Contains(session))
				{
					session.Clear();
					failed.Remove(session);
				}
				else
				{
					session.Flush();
				}
			}

			session = null;
		}

		/// <summary>
		/// Get objects from DB without NH cache
		/// </summary>
		/// <typeparam name="Entity">Main entity</typeparam>
		/// <typeparam name="ID">Integer ID type</typeparam>
		public static Entity GetNonCached<Entity, ID>(ID id)
		{
			using var otherSession = GetNonCached();
			
			var result = otherSession.Get<Entity>(id);
			otherSession.Close();
			otherSession.Dispose();

			return result;
		}

		/// <summary>
		/// Get objects from DB without NH cache
		/// </summary>
		internal static ISession GetNonCached()
		{
			return SessionFactoryManager.OpenSession();
		}

		private static readonly IDictionary<String, ISession> sessionList =
			new Dictionary<String, ISession>();

		private static ISession session
		{
			get
			{
				var key = getKey?.Invoke();

				if (key == null || !sessionList.ContainsKey(key))
					return null;

				return sessionList[key];
			}
			set
			{
				var key = getKey?.Invoke();

				if (key == null)
					return;

				if (sessionList.ContainsKey(key))
					sessionList[key] = value;
				else
					sessionList.Add(key, value);
			}
		}
	}
}
