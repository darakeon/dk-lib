using System;
using System.Collections.Generic;
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
            var propValueName = Property.Name(value);
            var propTextName = Property.Name(text);

            return new SelectList(list, propValueName, propTextName);
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
            var enumList = Enum.GetValues(typeof(TEnum));
            var selectList = new List<SelectListItem>();

            foreach (var item in enumList)
            {
                var value = item.ToString();

                var text = texts != null
                        && texts.Keys.Contains((TEnum)item)
                    ? texts[(TEnum)item] : value;

                selectList.Add(new SelectListItem
                {
                    Value = value,
                    Text = text,
                });
            }

            return new SelectList(selectList, "Value", "Text", 0);
        }
    }
}