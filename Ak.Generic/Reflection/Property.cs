using System;
using System.Linq;
using System.Linq.Expressions;

namespace Ak.Generic.Reflection
{
    ///<summary>
    /// Helper of PropertyInfo
    ///</summary>
    public class Property
    {
        ///<summary>
        /// Give a name of a property, to use lambda expression, not strings
        ///</summary>
        public static String Name<TOrigin>(Expression<Func<TOrigin>> property)
        {
            return name(property.Body);
        }

        ///<summary>
        /// Give a name of a property, to use lambda expression, not strings
        ///</summary>
        public static String Name<TOrigin, TProperty>(Expression<Func<TOrigin, TProperty>> property)
        {
            return name(property.Body);
        }

        private static String name(Expression property)
        {
            var expression = property.ToString().Split('.');

            return expression.LastOrDefault();
        }
    }
}
