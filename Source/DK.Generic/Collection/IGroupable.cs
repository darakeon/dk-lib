namespace DK.Generic.Collection
{
    ///<summary>
    /// Interface to use GroupedCollection
    ///</summary>
    ///<typeparam name="TG">Type of the Item that will group the objects</typeparam>
    public interface IGroupable<out TG>
    {
        ///<summary>
        /// When implemented in a class, Retrieve de Group of the object
        ///</summary>
        ///<returns></returns>
        TG GetGroup();
    }
}
