using System;
using System.Collections;
using System.Collections.Generic;

namespace DK.Generic.Collection
{
    /// <summary>
    /// Item of a Grouped Collection
    /// </summary>
    /// <typeparam name="TI">Type of the Items of the Group</typeparam>
    /// <typeparam name="TG">Type of the property responsable for Grouping</typeparam>
    public class ItemGroup<TI, TG> : IEnumerable
        where TI : IGroupable<TG>
    {
        /// <summary>
        /// Item of a Grouped Collection
        /// </summary>
        public ItemGroup()
        {
            ItemList = new List<TI>();
        }

        /// <summary>
        /// Item of a Grouped Collection
        /// </summary>
        public ItemGroup(TG group)
            : this()
        {
            Group = group;
        }

        ///<summary>
        /// The group chosen of this list
        ///</summary>
        public TG Group { get; set; }


        private IList<TI> ItemList { get; set; }


        ///<summary>
        /// Add an item for this Group
        ///</summary>
        public void Add(TI item)
        {
            ItemList.Add(item);
        }


        ///<summary>
        /// Get an item by its position
        ///</summary>
        public TI this[Int32 item]
        {
            get { return ItemList[item]; }
            set { ItemList[item] = value; }
        }

        ///<summary>
        /// Return the list of objects of this group
        ///</summary>
        public IList<TI> List { get { return ItemList; } }




        #region IEnumerable Members

        ///<summary>
        /// To make ForEach
        ///</summary>
        public IEnumerator<TI> GetEnumerator()
        {
            return ItemList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ItemList.GetEnumerator();
        }

        #endregion
    }
}
