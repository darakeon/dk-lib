using System;
using System.Collections.Generic;
using Ak.DataAccess.NHibernate.Helpers;
using Ak.DataAccess.NHibernate.UserPassed;
using Ak.Generic.Exceptions;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Metadata;

namespace Ak.DataAccess.NHibernate
{
    class SessionFactoryBuilder
    {
        private static ISessionFactory sessionFactory;

        private static Boolean hasSessionFactory
        {
            get { return sessionFactory != null && !sessionFactory.IsClosed; }
        }



        internal static void CreateSessionFactory<TM, TE>(ConnectionInfo connectionInfo, AutoMappingInfo<TM, TE> autoMappingInfo)
            where TM : IAutoMappingOverride<TE>
        {
            if (hasSessionFactory)
                return;

            var schemaChanger = new SchemaChanger(connectionInfo.ScriptFileFullName);

            var automapping = autoMappingInfo.CreateAutoMapping();

            var config = Fluently.Configure()
                .Database(connectionInfo.ConfigureDataBase())
                .Mappings(m => m.AutoMappings.Add(automapping));


            switch (connectionInfo.DBAction)
            {
                case DBAction.Recreate:
                    config = config.ExposeConfiguration(schemaChanger.Build);

                    if (connectionInfo.DataInitializer != null)
                        connectionInfo.DataInitializer.Initialize();

                    break;

                case DBAction.Update:
                    config = config.ExposeConfiguration(schemaChanger.Update);
                    break;

                case DBAction.Validate:
                    config = config.ExposeConfiguration(schemaChanger.Validate);
                    break;
            }

            sessionFactory = config.BuildSessionFactory();
        }

        internal static ISession OpenSession()
        {
            if (sessionFactory == null)
                throw new AkException("Restart the Application.");

            return sessionFactory.OpenSession();
        }

        internal static void End()
        {
            if (hasSessionFactory)
                sessionFactory.Close();
        }

        internal static IClassMetadata GetClassMetadata(Type type)
        {
            return sessionFactory.GetClassMetadata(type);
        }

    }
}
