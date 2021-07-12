using System;
using System.Collections;
using System.Collections.Generic;

namespace Keon.Util.Collection
{
	/// <inheritdoc />
	/// <summary>
	/// Item of a Grouped Collection
	/// </summary>
	/// <typeparam name="I">Type of the Items of the Group</typeparam>
	/// <typeparam name="G">Type of the property responsible for Grouping</typeparam>
	public class ItemGroup<I, G> : IEnumerable
		where I : IGroupable<G>
	{
		/// <summary>
		/// Item of a Grouped Collection
		/// </summary>
		public ItemGroup()
		{
			itemList = new List<I>();
		}

		/// <inheritdoc />
		/// <summary>
		/// Item of a Grouped Collection
		/// </summary>
		public ItemGroup(G group)
			: this()
		{
			Group = group;
		}

		///<summary>
		/// The group chosen of this list
		///</summary>
		public G Group { get; set; }


		private IList<I> itemList { get; }


		///<summary>
		/// Add an item for this Group
		///</summary>
		public void Add(I item)
		{
			itemList.Add(item);
		}


		///<summary>
		/// Get an item by its position
		///</summary>
		public I this[Int32 item]
		{
			get => itemList[item];
			set => itemList[item] = value;
		}

		///<summary>
		/// Return the list of objects of this group
		///</summary>
		public IList<I> List => itemList;


		#region IEnumerable Members

		///<summary>
		/// To make ForEach
		///</summary>
		public IEnumerator<I> GetEnumerator()
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
