using System;

namespace Ak.MVC.Forms
{
    /// <summary>
    /// Items for CheckBoxList and SelectListExtension
    /// </summary>
    public interface IListable
    {
        /// <summary>
        /// Value of CheckBox / SelectList
        /// </summary>
        Int32 ID { get; set; }

        /// <summary>
        /// Text of CheckBox / SelectList
        /// </summary>
        String Name { get; set; }
    }
}