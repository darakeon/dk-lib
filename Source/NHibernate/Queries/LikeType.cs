namespace Keon.NHibernate.Queries
{
	/// <summary>
	/// Type of like comparison
	/// </summary>
	public enum LikeType
	{
		/// <summary>
		/// Look start and end of string
		/// </summary>
		Both = 1,

		/// <summary>
		/// Look just at start of string
		/// </summary>
		JustStart = 2,

		/// <summary>
		/// Look just at end of string
		/// </summary>
		JustEnd = 3,
	}
}
