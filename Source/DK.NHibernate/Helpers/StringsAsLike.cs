using System;
using DK.NHibernate.Base;
using NHibernate;
using NHibernate.Criterion;

namespace DK.NHibernate.Helpers
{
    internal class StringsAsLike
    {
        public static Disjunction GenericSearch<T>(String[] words)
        {
            var type = typeof(T);
            var meta = SessionFactoryBuilder.GetClassMetadata(type);

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
