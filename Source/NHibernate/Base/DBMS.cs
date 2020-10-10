namespace Keon.NHibernate.Base
{
	/// <summary>
	/// List of accepted DBMSs
	/// </summary>
	public enum DBMS
	{
		///<summary></summary>
		MySQL,
		///<summary></summary>
		MsSql2000,
		///<summary></summary>
		MsSql2005,
		///<summary></summary>
		MsSql2008,
		///<summary></summary>
		MsSql7,
		/// <summary>Doesn't need SERVER property</summary>
		Postgres,
		/// <summary>Doesn't need DATABASE property</summary>
		Oracle9,
		/// <summary>Doesn't need DATABASE property</summary>
		Oracle10,
		/// <summary>Doesn't need DATABASE property</summary>
		SQLite,
	}
}
