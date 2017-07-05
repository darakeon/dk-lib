using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Ak.Generic.Reflection;

namespace Ak.MVC.Forms
{
    ///<summary>
    ///</summary>
    public class SelectListExtension
    {
        ///<summary>
        /// Create a SelectList from List of objects
        ///</summary>
        ///<param name="list">The list that will fill the SelectList</param>
        ///<param name="value">The property the will fill the values of the options</param>
        ///<param name="text">The property the will fill the text that appears in options</param>
        public static SelectList CreateSelect<TOrigin, TValue, TText>(IList<TOrigin> list, Expression<Func<TOrigin, TValue>> value, Expression<Func<TOrigin, TText>> text)
        {
            var propValueName = value.GetName();
            var propTextName = text.GetName();

            return new SelectList(list, propValueName, propTextName);
        }


        ///<summary>
        /// Create a SelectList from List of objects (must be IListable)
        ///</summary>
        ///<param name="list">The list that will fill the SelectList</param>
        public static SelectList CreateSelect<T>(IList<T> list)
            where T : IListable
        {
            return new SelectList(list, "ID", "NAME");
        }


        ///<summary>
        /// Create a SelectList from a Enum, using the text.
        /// Can't use the number value, MVC don't recognize to assign selected.
        ///</summary>
        ///<param name="texts">
        /// The texts that will appear for the user.
        /// key = value of enum // value = text to appear
        /// </param>
        public static SelectList CreateSelect<TEnum>(IDictionary<TEnum, String> texts = null)
        {
            var selectList = texts == null
                ? createSelectFromEnumValues<TEnum>()
                : createSelectFromDictionary(texts);

            return new SelectList(selectList, "Value", "Text", 0);
        }

        private static IEnumerable<SelectListItem> createSelectFromEnumValues<TEnum>()
        {
            return Enum.GetValues(typeof (TEnum))
                .Cast<TEnum>()
                .Select(t => t.ToString())
                .Select(t => new SelectListItem { Value = t, Text = t, });
        }

        private static IEnumerable<SelectListItem> createSelectFromDictionary<TEnum>(IEnumerable<KeyValuePair<TEnum, string>> texts)
        {
            return texts.Select(t => 
                new SelectListItem { Value = t.Key.ToString(), Text = t.Value, });
        }

    }
}