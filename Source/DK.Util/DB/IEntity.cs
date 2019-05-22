using System;

namespace Keon.Util.DB
{
    /// <summary>
    /// Minimum entity for DB
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// DB unique identifier
        /// </summary>
        Int32 ID { get; set; }
    }
}
