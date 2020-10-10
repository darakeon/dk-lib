using System;
using Keon.NHibernate.Schema;
using NHibernate;
using NHibernate.Criterion;

namespace Keon.NHibernate.Queries
{
    internal class StringsAsLike
    {
        public static Disjunction LikeAny<Entity>(String[] words)
        {
            var type = typeof(Entity);
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
                            "%" + word + "%"
                        )
                    );
                }
            }

            return disjunction;
        }
    }
}
