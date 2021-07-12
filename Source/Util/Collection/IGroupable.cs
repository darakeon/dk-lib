namespace Keon.Util.Collection
{
	///<summary>
	/// Interface to use GroupedCollection
	///</summary>
	///<typeparam name="G">Type of the Item that will group the objects</typeparam>
	public interface IGroupable<out G>
	{
		///<summary>
		/// When implemented in a class, Retrieve de Group of the object
		///</summary>
		///<returns></returns>
		G GetGroup();
	}
}
