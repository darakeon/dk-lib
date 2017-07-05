using System;

namespace Ak.Generic.DB
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
