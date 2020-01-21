﻿using System;
using Keon.NHibernate.Base;
using NHibernate;
using NHibernate.Criterion;

namespace Keon.NHibernate.Helpers
{
    internal class StringsAsLike
    {
        public static Disjunction GenericSearch<T>(String[] words)
        {
            var type = typeof(T);
            var meta = SessionFactoryManager.Instance.GetClassMetadata(type);

            var disjunction = new Disjunction();

            foreach (var mappedPropertyName in meta.PropertyNames)
            {
                var propertyType = meta.GetPropertyType(mappedPropertyName);

                if (!propertyType.Equals(NHibernateUtil.String))
                    continue;

                foreach (var word in words)
                {
                    disjunction.Add(
                        Restrictions.InsensitiveLike(
                            mappedPropertyName,
                            "%" + word + "%"));
                }
            }

            return disjunction;

        }
    }
}