using System;
using System.Collections.Generic;
using System.Linq;

namespace Keon.MVC.Forms
{
    /// <summary>
    /// To create a CheckBoxList in Form
    /// (copy the View naming it CheckboxList.cshtml to Views/Shared/EditorTemplates)
    /// </summary>
    public class CheckBoxList
    {
        /// <summary>
        /// Items of CheckBoxList
        /// </summary>
        public IEnumerable<CheckBoxItem> Items { get; set; }

        /// <summary>
        /// Constructor for MVC build
        /// </summary>
        public CheckBoxList()
        {
            Items = new List<CheckBoxItem>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor to use in your code
        /// </summary>
        /// <param name="all">All the items to show</param>
        /// <param name="chosen">Just the chosen items IDs</param>
        public CheckBoxList(IEnumerable<IListable> all, IEnumerable<Int32> chosen)
            : this()
        {
            Items = all.Select(e => add(e, chosen));
        }

        private static CheckBoxItem add(IListable item, IEnumerable<Int32> chosen)
        {
            return new CheckBoxItem
            {
                ID = item.ID,
                Name = item.Name,
                Chosen = chosen.Contains(item.ID),
            };
        }

        /// <summary>
        /// Get the chosen ids
        /// </summary>
        public IEnumerable<Int32> GetChosen()
        {
            return Items.Where(cbi => cbi.Chosen).Select(cbi => cbi.ID);
        }


        /// <summary>
        /// Item for CheckBoxList
        /// </summary>
        public class CheckBoxItem
        {
            /// <summary>
			/// Field ID
			/// </summary>
            public Int32 ID { get; set; }

			/// <summary>
			/// Field name
			/// </summary>
            public String Name { get; set; }
			
			/// <summary>
			/// Check
			/// </summary>
            public Boolean Chosen { get; set; }
        }



        /// <summary>
        /// HTML to create the EditorFor
        /// Past it in Views/Shared/EditorTemplates/CheckboxList.cshtml
        /// </summary>
        public readonly String View = @"
@model DK.MVC.Forms.CheckBoxList

<ul>
    @for (var e = 0; e < Model.Items.Count; e++)
    {
        <li>
            @Html.CheckBoxFor(m => m.Items[e].Chosen)
            @Html.HiddenFor(m => m.Items[e].ID)
            @Html.LabelFor(m => m.Items[e].Chosen, Model.Items[e].Name)
        </li>
    }
</ul>
";

    }
}
