using System;

namespace Keon.NHibernate.Operations
{
	/// <summary>
	/// Interface for searches by Query
	/// </summary>
	public interface ISearch
	{
		/// <summary>
		/// Page target, considering ItemsPerPage
		/// </summary>
		Int32 Page { get; }
		
		/// <summary>
		/// Items that should be brought on each page
		/// </summary>
		Int32 ItemsPerPage { get; }
	}
}
