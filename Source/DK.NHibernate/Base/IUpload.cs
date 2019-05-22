using System;

namespace Keon.NHibernate.Base
{
	/// <summary>
	/// Interface for object which brings the file
	/// </summary>
	public interface IUpload
	{
		/// <summary>
		/// Original name of the file (to be shown)
		/// </summary>
		String OriginalName { get; }

		/// <summary>
		/// Method of saving on disk the file
		/// </summary>
		/// <param name="path"></param>
		void Save(String path);
	}
}