using System;
using System.Collections.Generic;
using System.Linq;
using Keon.Util.DB;
using Keon.Util.Exceptions;

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

		/// <summary>
		/// Use in-memory data with lists, for tests
		/// </summary>
		internal static IDictionary<String, IDictionary<String, Int16>> FakeFieldSizes { get; set; }

		internal static void TestSizes<T>(T entity)
			where T : class, IEntity, new()
		{
			var entityName = typeof(T).Name;

			if (!FakeFieldSizes.ContainsKey(entityName))
				return;

			FakeFieldSizes[entityName]
				.ToList()
				.ForEach(p => testSize(p.Key, p.Value, entity));
		}

		private static void testSize<T>(String name, Int16 length, T entity)
			where T : class, IEntity, new()
		{
			var property = typeof(T).GetProperty(name);
			var value = property?.GetValue(entity)?.ToString();

			if (value?.Length > length)
				throw new DKException("TooLargeData");
		}
	}
}
