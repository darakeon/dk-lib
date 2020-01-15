using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Keon.Util.Reflection
{
    ///<summary>
    /// Helper of PropertyInfo
    ///</summary>
    public static class Property
    {
        ///<summary>
        /// Give a name of a property, to use lambda expression, not strings
        ///</summary>
        public static String GetName<TOrigin>(this Expression<Func<TOrigin>> property)
        {
			return property.Body.getName();
        }

		///<summary>
		/// Give a name of a property, to use lambda expression, not strings
		///</summary>
		public static String GetName<TOrigin, TProperty>(this Expression<Func<TOrigin, TProperty>> property)
		{
			return property.Body.getName();
		}

		private static String getName(this Expression property)
        {
            var expression = property.ToString().Split('.');

            return expression.LastOrDefault();
        }

		/// <summary>
		/// Parent type
		/// </summary>
		public static Type ReferenceType<TObject, TProperty>(this Expression<Func<TObject, TProperty>> property)
		{
			return ((MemberExpression)property.Body).Expression.Type;
		}

		///<summary>
		/// Break parent entities
		///</summary>
		public static IEnumerable<String> NormalizePropertyName<TOrigin, TProperty>(this Expression<Func<TOrigin, TProperty>> property)
		{
			var name = property.GetName();
			return name.Split('.').Skip(1);
		}

	}
}
