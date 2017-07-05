using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DK.Generic.Collection
{
    /// <summary>
    /// Collection Grouped by one of the properties of the original IList
    /// </summary>
    /// <typeparam name="TI">Type of the Items of the Group</typeparam>
    /// <typeparam name="TG">Type of the property responsable for Grouping</typeparam>
    public class GroupedCollection<TI, TG> : IEnumerable
        where TI : IGroupable<TG>
    {
        /// <summary>
        /// Collection Grouped by one of the properties of the original IList
        /// </summary>
        public GroupedCollection()
        {
            GroupList = new List<ItemGroup<TI, TG>>();
        }


        private IList<ItemGroup<TI, TG>> GroupList { get; set; }

        ///<summary>
        /// Add an Item to the list, evaluation the Group whether it belongs
        ///</summary>
        public void Add(TI item)
        {
            var group = item.GetGroup();

            if (!GroupList.Any(mg => mg.Group.Equals(group)))
                GroupList.Add(new ItemGroup<TI, TG>(group));


            GroupList
                .SingleOrDefault(gp => gp.Group.Equals(group))
                .Add(item);
        }

        ///<summary>
        /// Add an List of the Item to the list, evaluation the Group whether it belongs
        ///</summary>
        public void AddRange(IList<TI> list)
        {
            foreach (var measure in list)
            {
                Add(measure);
            }
        }


        ///<summary>
        /// Return a Group of items
        ///</summary>
        public ItemGroup<TI, TG> this[Int32 group]
        {
            get { return GroupList[group]; }
            set { GroupList[group] = value; }
        }

        ///<summary>
        /// Return the list of Groups and itens
        /// Recommended when is needed to use Linq
        ///</summary>
        public IList<ItemGroup<TI, TG>> List { get { return GroupList; } }


        /// <summary> Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>. </summary>
        /// <returns> A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>. </returns>
        public override String ToString()
        {
            return String.Format("Count = {0}", List.Count);
        }



        #region IEnumerable Members

        ///<summary>
        /// To make ForEach
        ///</summary>
        public IEnumerator<ItemGroup<TI, TG>> GetEnumerator()
        {
            return GroupList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GroupList.GetEnumerator();
        }

        #endregion

    }

}
