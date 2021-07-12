namespace Keon.NHibernate.Schema
{
	///<summary>
	/// Action for DB when create SessionFactory
	///</summary>
	public enum DBAction
	{
		///<summary>
		/// Don't even validate the schema.
		/// The errors will appear just when the entity is accessed.
		///</summary>
		None,
		///<summary>
		/// Drop and Create the DB.
		/// The saved data will be LOST.
		///</summary>
		Recreate,
		///<summary>
		/// Just adjust the DB to match the entities.
		/// Can recreate foreign keys.
		///</summary>
		Update,
		///<summary>
		/// Verify errors on entities and mapping.
		/// Avoid error to just appear when entities are accessed.
		///</summary>
		Validate,
	}
}
