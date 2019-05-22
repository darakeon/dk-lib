namespace Keon.NHibernate.Queries
{
	/// <summary>
	/// Type of summarize
	/// </summary>
	public enum SummarizeType
	{
		/// <summary>
		/// Count items
		/// </summary>
		Count = 1,

		/// <summary>
		/// Get biggest item
		/// </summary>
		Max = 2,

		/// <summary>
		/// Sum all items
		/// </summary>
		Sum = 3,
	}
}