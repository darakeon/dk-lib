﻿using System;
using Ak.NHibernate.Conventions;
using Ak.NHibernate.Helpers;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Conventions;

namespace Ak.NHibernate.UserPassed
{
    /// <summary>
    /// Information to use Automapping of Fluent
    /// </summary>
    /// <typeparam name="TM">Some of the AutoMaps, just for reference</typeparam>
    /// <typeparam name="TE">Some of the Entities, just for reference</typeparam>
    //public class AutoMappingInfo<TM, TE> where TM : IAutoMappingOverride<TE>
    public class AutoMappingInfo<TM, TE> where TM : IAutoMappingOverride<TE>
    {
        /// <summary>
        /// EntityBase, if it exists, to be ignored on mapping
        /// </summary>
        public Type EntityBase { get; set; }

        /// <summary>
        /// Classes which subclasses use its table
        /// </summary>
        public Type[] SuperEntities { get; set; }

        /// <summary>
        /// Conventions to configure Fluent
        /// </summary>
        public IConvention[] Conventions { get; set; }



        internal AutoPersistenceModel CreateAutoMapping()
        {
            var storeConfiguration = new StoreConfiguration(typeof(TE), SuperEntities);
            var assembly = typeof(TE).Assembly;

            var autoMap = AutoMap
                .Assemblies(storeConfiguration, assembly)
                .UseOverridesFromAssemblyOf<TM>()
                .IgnoreBase(EntityBase)
                .Conventions.Add(
                    new NullableConvention.Property(),
                    new NullableConvention.Reference(),
                    new EnumConvention(),
                    new CascadeConvention(),
                    new NameConvention.N2N(),
                    new NameConvention.HasMany(),
                    new NameConvention.Reference()
                );


            if (Conventions != null)
                autoMap = autoMap.Conventions.Add(Conventions);

            return autoMap;
        }
    
    }
}