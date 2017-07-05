using System;
using System.Collections.Generic;
using DK.Generic.Exceptions;
using DK.MVC.Cookies;
using DK.NHibernate.Base;
using DK.NHibernate.UserPassed;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;

namespace DK.NHibernate
{
    /// <summary>
    /// Session Manager
    /// </summary>
    public class NHManager
    {
        private static readonly IDictionary<String, SessionWithCount> sessionList = new Dictionary<String, SessionWithCount>();

        /// <summary>
        /// Communicates with DB
        /// </summary>
        public static ISession Session
        {
            get { return getSession(key).NHSession; }
        }

        /// <summary>
        /// Helps to get old data without messing with changed entities
        /// </summary>
        public static ISession SessionOld
        {
            get { return getSession(keyOld).NHSession; }
        }

        private static SessionWithCount getSession(String sessionKey)
        {
            if (!sessionList.ContainsKey(sessionKey))
            {
                var session = SessionBuilder.Open();

                try
                {
                    sessionList.Add(sessionKey, new SessionWithCount(session));
                }
                catch (ArgumentException) { }
            }

            return sessionList[sessionKey];
        }


        /// <summary>
        /// Open Session and SessionOld to use
        /// </summary>
        public static void Open()
        {
            getSession(key).AddUse();
            getSession(keyOld).AddUse();
        }


        /// <summary>
        /// Starts session factory
        /// </summary>
        /// <typeparam name="TMap">AutoMappingOverride sample</typeparam>
        /// <typeparam name="TEntity">Entity sample</typeparam>
        public static void Start<TMap, TEntity>() 
            where TMap : IAutoMappingOverride<TEntity>
        {
            var mapInfo = new AutoMappingInfo<TMap, TEntity>();

            SessionBuilder.Start(mapInfo);
        }



        /// <summary>
        /// Handle error case
        /// </summary>
        public static void Error()
        {
            error(Session, key);
            error(SessionOld, keyOld);
        }

        private static void error(ISession session, string sessionKey)
        {
            SessionBuilder.Error(session);
            sessionList.Remove(sessionKey);
        }



        /// <summary>
        /// Close session
        /// </summary>
        public static void Close()
        {
            close(IsActive, key);
            close(isActiveOld, keyOld);
        }

        private static void close(Boolean isActive, String sessionKey)
        {
            if (!isActive)
                return;

            var session = getSession(sessionKey);

            session.RemoveUse();

            if (!session.IsInUse())
            {
                SessionBuilder.Close(session.NHSession);
                sessionList.Remove(sessionKey);
            }
        }



        /// <summary>
        /// Finishes session factory
        /// </summary>
        public static void End()
        {
            SessionBuilder.End();
        }



        private static String key
        {
            get { return MyCookie.Get().Key; }
        }

        private static String keyOld
        {
            get { return key + "_old"; }
        }



        /// <summary>
        /// Verify whether can use session
        /// </summary>
        public static Boolean IsActive
        {
            get { return isActive(key); }
        }

        private static Boolean isActiveOld
        {
            get { return isActive(keyOld); }
        }

        private static Boolean isActive(String sessionKey)
        {
            try
            {
                return getSession(sessionKey).NHSession.IsOpen;
            }
            catch (DKException)
            {
                return false;
            }
        }




        internal class SessionWithCount
        {
            public SessionWithCount(ISession session)
            {
                NHSession = session;
            }

            public void AddUse()
            {
                Count++;
            }

            public void RemoveUse()
            {
                Count--;
            }

            public Boolean IsInUse()
            {
                return Count > 0;
            }

            public ISession NHSession { get; private set; }
            public Int32 Count { get; private set; }

        }


    }
}