using Keon.NHibernate.Helpers;

namespace Keon.NHibernate.UserPassed
{
    /// <summary>
    /// Inherit this class and pass yours to ConnectionInfo class to fill the DB after create.
    /// The property CreateDB need to be TRUE.
    /// </summary>
    public interface IDataInitializer
    {
		/// <summary>
		/// Fill the Database.
		/// </summary>
		void PopulateDB();

		/// <summary>
		/// Which action to take when start NHibernate
		/// </summary>
		DBAction DBAction { get; }
	}
}
