using System;

namespace Keon.NHibernate.Fakes
{
	/// <summary>
	/// Helper for Fake DB
	/// </summary>
	public class FakeHelper
	{
		/// <summary>
		/// Use in-memory data with lists, for tests
		/// </summary>
		public static Boolean IsFake { get; set; }
	}
}
