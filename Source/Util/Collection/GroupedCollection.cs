using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Keon.Util.Collection
{
    /// <inheritdoc />
    /// <summary>
    /// Collection Grouped by one of the properties of the original IList
    /// </summary>
    /// <typeparam name="Items">Type of the Items of the Group</typeparam>
    /// <typeparam name="Prop">Type of the property responsible for Grouping</typeparam>
    public class GroupedCollection<Items, Prop> : IEnumerable
        where Items : IGroupable<Prop>
    {
        /// <summary>
        /// Collection Grouped by one of the properties of the original IList
        /// </summary>
        public GroupedCollection()
        {
            groupList = new List<ItemGroup<Items, Prop>>();
        }


        private IList<ItemGroup<Items, Prop>> groupList { get; }

        ///<summary>
        /// Add an Item to the list, evaluation the Group whether it belongs
        ///</summary>
        public void Add(Items item)
        {
            var group = item.GetGroup();

            if (!groupList.Any(mg => mg.Group.Equals(group)))
                groupList.Add(new ItemGroup<Items, Prop>(group));


            groupList
                .SingleOrDefault(gp => gp.Group.Equals(group))?
                .Add(item);
        }

        ///<summary>
        /// Add an List of the Item to the list, evaluation the Group whether it belongs
        ///</summary>
        public void AddRange(IList<Items> list)
        {
            foreach (var measure in list)
            {
                Add(measure);
            }
        }


        ///<summary>
        /// Return a Group of items
        ///</summary>
        public ItemGroup<Items, Prop> this[Int32 group]
        {
            get => groupList[group];
	        set => groupList[group] = value;
        }

        ///<summary>
        /// Return the list of Groups and items
        /// Recommended when is needed to use Linq
        ///</summary>
        public IList<ItemGroup<Items, Prop>> List => groupList;


	    /// <summary> Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>. </summary>
        /// <returns> A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>. </returns>
        public override String ToString()
        {
            return $"Count = {List.Count}";
        }

        ///<summary>
        /// To make ForEach
        ///</summary>
        public IEnumerator<ItemGroup<Items, Prop>> GetEnumerator()
        {
            return groupList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return groupList.GetEnumerator();
        }
    }

}
