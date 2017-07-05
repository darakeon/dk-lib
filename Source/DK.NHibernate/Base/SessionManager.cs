using System;
using System.Collections.Generic;
using NHibernate;

namespace DK.NHibernate.Base
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
		private static String key => getKey?.Invoke();



		/// <summary>
		/// Get current NH Session
		/// </summary>
		public static ISession GetCurrent()
		{
			return session ?? (session = SessionFactoryManager.OpenSession());
		}



		public static Boolean Failed { private get; set; }

		/// <summary>
		/// Close session
		/// </summary>
		public static void Close()
		{
			if (session != null)
			{
				if (session.IsOpen)
				{
					if (Failed)
						session.Clear();
					else
						session.Flush();
				}

				session = null;
			}
		}



		/// <summary>
		/// Get objects from DB without NH cache
		/// </summary>
		public static T GetNonCached<T>(int id)
		{
			T result;

			using (var otherSession = GetNonCached())
			{
				result = otherSession.Get<T>(id);
				otherSession.Close();
				otherSession.Dispose();
			}

			return result;
		}

		/// <summary>
		/// Get objects from DB without NH cache
		/// </summary>
		internal static ISession GetNonCached()
        {
            return SessionFactoryManager.Instance.OpenSession();
        }



		private static readonly IDictionary<String, ISession> sessionList = new Dictionary<String, ISession>();

		private static ISession session
		{
			get
			{
				if (!sessionList.ContainsKey(key))
					return null;

				return sessionList[key];
			}
			set
			{
				if (sessionList.ContainsKey(key))
				{
					sessionList[key] = value;
				}
				else
				{
					sessionList.Add(key, value);
				}
			}
		}



	}
}