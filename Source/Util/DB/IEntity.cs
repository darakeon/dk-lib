using System;

namespace Keon.Util.DB
{
    /// <summary>
    /// Minimum entity for DB
    /// </summary>
    public interface IEntity<I>
		where I: struct
    {
        /// <summary>
        /// DB unique identifier
        /// </summary>
        I ID { get; set; }
    }

    /// <inheritdoc />
    public interface IEntityShort : IEntity<Int16> { }

    /// <inheritdoc />
    public interface IEntity : IEntity<Int32> { }

	/// <inheritdoc />
	public interface IEntityLong : IEntity<Int64> { }
}
