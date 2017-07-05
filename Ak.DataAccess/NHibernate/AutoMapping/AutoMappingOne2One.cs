using System;
using System.Linq.Expressions;
using Ak.Generic.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;

namespace DFM.Repositories.Mappings
{
    public static class AutoMappingOne2One
    {
        public static IdentityPart IsWeakEntity<T, TFather>(this AutoMapping<T> mapping, Expression<Func<T, TFather>> memberExpression)
        {
            var motherEntityName = Property.Name(memberExpression);

            return mapping.Id()
                .GeneratedBy.Foreign(motherEntityName);
        }

    }
}
