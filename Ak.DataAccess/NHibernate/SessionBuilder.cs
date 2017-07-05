using System;
using System.Web;
using Ak.DataAccess.NHibernate.Helpers;
using Ak.DataAccess.NHibernate.UserPassed;
using Ak.Generic.Enums;
using FluentNHibernate.Automapping.Alterations;
using NHibernate;
using CM = System.Configuration.ConfigurationManager;

namespace Ak.DataAccess.NHibernate
{
    /// <summary>
    /// To create NHibernate Session and communicate with DB
    /// </summary>
    public class SessionBuilder
    {
        /// <summary>
        /// Session of NHibernate
        /// </summary>
        public static ISession Session
        {
            get { return HttpContext.Current == null ? null : (ISession) HttpContext.Current.Items["NhSession"]; }
            private set { HttpContext.Current.Items.Add("NhSession", value); }
        }

        private static Boolean hasSession
        {
            get { return Session != null && Session.IsOpen; }
        }

        private static Boolean hasTransaction
        {
            get
            {
                return hasSession && transaction.IsActive
                    && !transaction.WasRolledBack;
            }
        }

        private static ITransaction transaction
        {
            get { return Session.Transaction; }
        }


        ///<summary>
        /// Create Session Factory.
        /// To be used at Application_Start.
        ///</summary>
        /// <typeparam name="TM">Any mapping class. Passed automatic by passing to AutoMappingInfo.</typeparam>
        /// <typeparam name="TE">Any entity class. Passed automatic by passing to AutoMappingInfo.</typeparam>
        /// <param name="connectionInfo">About database connection</param>
        /// <param name="autoMappingInfo">About mappings on the entities</param>
        public static void Start<TM, TE>(ConnectionInfo connectionInfo, AutoMappingInfo<TM, TE> autoMappingInfo)
            where TM : IAutoMappingOverride<TE>
        {
            if (connectionInfo == null)
                connectionInfo = new ConnectionInfo
                {
                    Server = CM.AppSettings["Server"],
                    DataBase = CM.AppSettings["DataBase"],
                    Login = CM.AppSettings["Login"],
                    Password = CM.AppSettings["Password"],
                    DBMS = Str2Enum.Cast<DBMS>(CM.AppSettings["DBMS"]),
                    DBAction = Str2Enum.Cast<DBAction>(CM.AppSettings["DBAction"]),
                    ScriptFileFullName = CM.AppSettings["ScriptFileFullName"],
                    ShowSQL = CM.AppSettings["ShowSQL"].ToLower() == "true",
                };

            SessionFactoryBuilder.CreateSessionFactory(connectionInfo, autoMappingInfo);
        }



        /// <summary>
        /// Create Session Factory, using the AppSettings.
        /// The keys required are the ConnectionInfo class properties.
        /// To be used at Application_Start.
        /// </summary>
        /// <typeparam name="TM">Any mapping class. Passed automatic by passing to AutoMappingInfo.</typeparam>
        /// <typeparam name="TE">Any entity class. Passed automatic by passing to AutoMappingInfo.</typeparam>
        /// <param name="autoMappingInfo">About mappings on the entities</param>
        public static void Start<TM, TE>(AutoMappingInfo<TM, TE> autoMappingInfo)
            where TM : IAutoMappingOverride<TE>
        {
            Start(null, autoMappingInfo);
        }



        /// <summary>
        /// Open the NH session.
        /// To be used at Application_BeginRequest.
        /// </summary>
        public static void Open()
        {
            Session = SessionFactoryBuilder.OpenSession();
            Session.BeginTransaction();
        }

        
        
        ///<summary>
        /// Disconnect from DB.
        /// To be used at Application_EndRequest.
        ///</summary>
        public static void Close()
        {
            if (!hasTransaction) 
                return;

            transaction.Commit();
            Session.Flush();
        }

        ///<summary>
        /// Close the SessionFactory.
        /// To be used at Application_End.
        ///</summary>
        public static void End()
        {
            if (hasSession)
                Session.Close();

            SessionFactoryBuilder.End();
        }



        ///<summary>
        /// Rollback the actions in case of Error.
        /// To be used at Application_Error.
        ///</summary>
        public static void Error()
        {
            if (hasTransaction)
                transaction.Rollback();
        }



        ///<summary>
        /// Force the Initialize of NHibernate objects.
        ///</summary>
        public static void NhInitialize(object obj)
        {
            if (hasSession && !Session.Contains(obj))
                NHibernateUtil.Initialize(obj);
        }
    }
}