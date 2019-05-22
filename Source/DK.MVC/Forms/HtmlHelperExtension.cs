using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Keon.MVC.Forms
{
    ///<summary>
    /// 
    ///</summary>
    public static class HtmlHelperExtension
    {
        ///<summary>
        /// Create a checkbox
        ///</summary>
        ///<param name="htmlHelper"></param>
        ///<param name="expression">The list which will receive the values in controller</param>
        ///<param name="value">The value of this checkbox</param>
        ///<param name="htmlAttributes">Optional</param>
        ///<typeparam name="TModel">The current Model</typeparam>
        ///<typeparam name="TProperty">The property that will receive the value in Controller</typeparam>
        public static MvcHtmlString CheckBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, object htmlAttributes)
        {
            var model = htmlHelper.RadioButtonFor(expression, value, htmlAttributes);

            var checkbox = model.ToString().Replace(@"type=""radio""", @"type=""checkbox""");

            return new MvcHtmlString(checkbox);
        }


        ///<summary>
        /// Create a checkbox
        ///</summary>
        ///<param name="htmlHelper"></param>
        ///<param name="expression">The list which will receive the values in controller</param>
        ///<param name="value">The value of this checkbox</param>
        ///<typeparam name="TModel">The current Model</typeparam>
        ///<typeparam name="TProperty">The property that will receive the value in Controller</typeparam>
        public static MvcHtmlString CheckBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value)
        {
            return htmlHelper.CheckBoxFor(expression, value, null);
        }

    }
}