using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Metadata;
using NHibernate.Type;

namespace Ak.DataAccess.NHibernate.Helpers
{
    internal class StringsAsLike
    {
        public static Disjunction GenericSearch<T>(String[] words)
        {
            var type = typeof(T);
            var meta = SessionFactoryBuilder.GetClassMetadata(type);

            var disjunction = new Disjunction();

            foreach (string mappedPropertyName in meta.PropertyNames)
            {
                var propertyType = meta.GetPropertyType(mappedPropertyName);

                if (propertyType == NHibernateUtil.String)
                {
                    foreach (string word in words)
                    {
                        disjunction.Add(
                            Restrictions.InsensitiveLike(
                                mappedPropertyName,
                                "%" + word + "%"));
                    }
                }
            }

            return disjunction;

        }
    }
}
