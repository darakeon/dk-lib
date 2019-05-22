using System;

namespace Keon.NHibernate.Base
{
	/// <summary>
	/// Interface for Entity which has file
	/// </summary>
	public interface IUploadParent
	{
		/// <summary>
		/// Method to set on entity the fields of filename
		/// </summary>
		void SetFileNames(String newFileName, String originalName);
	}
}