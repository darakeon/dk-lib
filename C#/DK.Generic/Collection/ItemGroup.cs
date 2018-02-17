using System;
using System.Collections;
using System.Collections.Generic;

namespace DK.Generic.Collection
{
    /// <inheritdoc />
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
            itemList = new List<TI>();
        }

        /// <inheritdoc />
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


        private IList<TI> itemList { get; }


        ///<summary>
        /// Add an item for this Group
        ///</summary>
        public void Add(TI item)
        {
            itemList.Add(item);
        }


        ///<summary>
        /// Get an item by its position
        ///</summary>
        public TI this[Int32 item]
        {
            get => itemList[item];
	        set => itemList[item] = value;
        }

        ///<summary>
        /// Return the list of objects of this group
        ///</summary>
        public IList<TI> List => itemList;


	    #region IEnumerable Members

        ///<summary>
        /// To make ForEach
        ///</summary>
        public IEnumerator<TI> GetEnumerator()
        {
            return itemList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return itemList.GetEnumerator();
        }

        #endregion
    }
}
