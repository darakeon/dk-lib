using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Keon.NHibernate.Helpers;

namespace Keon.NHibernate.Conventions
{
    internal class NameConvention
    {
        internal class ManyToMany : ManyToManyTableNameConvention
        {
            protected override String GetBiDirectionalTableName(IManyToManyCollectionInspector collection, IManyToManyCollectionInspector otherSide)
            {
				return $"{collection.EntityType.Name.ToLower()}_{otherSide.EntityType.Name.ToLower()}";
			}

			protected override String GetUniDirectionalTableName(IManyToManyCollectionInspector collection)
            {
				return $"{collection.EntityType.Name.ToLower()}_{collection.ChildType.Name.ToLower()}";
			}
		}

        internal class Reference : IReferenceConvention
        {
            public void Apply(IManyToOneInstance instance)
            {
                var propertyName = putID(instance.Property.Name);

                instance.Column(propertyName);

                instance.ForeignKey($"FK_{instance.EntityType.Name}_{instance.Name}");
            }
        }

		internal class TableNameConvention : IClassConvention
		{
			public void Apply(IClassInstance instance)
			{
				var tableName = GetTableNameWithNamespace(instance.EntityType);

				instance.Table(tableName.ToLower());
			}
		}

		internal class HasMany : IHasManyConvention
        {
            public void Apply(IOneToManyCollectionInstance instance)
            {
				var referenceName = instance.Member.Name;

				referenceName = referenceName.EndsWith("List")
							   ? referenceName.Substring(0, referenceName.Length - 4)
							   : referenceName;

				if (instance.IsSystemEntity())
				{
					var memberTypeName = instance.ChildType.Name;

					var keepEntityNameAsForeignKey =
						referenceName.ToLower().Contains(memberTypeName.ToLower())
						|| memberTypeName.ToLower().Contains(referenceName.ToLower());

					referenceName = keepEntityNameAsForeignKey
							? instance.EntityType.Name
							: referenceName;

					var constraintName = $"FK_{memberTypeName}_{referenceName}";
					instance.Key.ForeignKey(constraintName);

					referenceName = putID(referenceName);
					instance.Key.Column(referenceName);
				}
				else
				{
					var typeName = instance.EntityType.Name;

					var constraintName = $"FK_{typeName}_{referenceName}";
					instance.Key.ForeignKey(constraintName);

					var originTableName = GetTableNameWithNamespace(instance.EntityType);

					instance.Table((originTableName + "_" + referenceName).ToLower());

					referenceName = putID(typeName);
					instance.Key.Column(referenceName);
				}
			}
		}

        private static String putID(String name)
        {
            return $"{name}_ID";
        }

		private const int first_letters = 2;
		private const int last_letters = 2;
		private const int total_letters = first_letters + last_letters;

		/// <summary>
		/// Rules for table naming from entity type
		/// </summary>
		public static string GetTableNameWithNamespace(Type entityType)
		{
			var dllName = entityType.Assembly.ManifestModule.Name;
			var mainNamespace = dllName.Replace(".dll", "");

			if (entityType.Namespace == null || entityType.Namespace == mainNamespace)
				return entityType.Name;

			var @namespace = entityType.Namespace.Substring(mainNamespace.Length + 1);

			var namespacePieces = @namespace.Split('.');

			for (var p = 0; p < namespacePieces.Length; p++)
			{
				var piece = namespacePieces[p];
				var count = piece.Length;

				if (count <= total_letters)
					continue;

				var firstPart = piece.Substring(0, first_letters);
				var shortenLetters = count - total_letters;
				var lastPart = piece.Substring(count - last_letters, last_letters);

				namespacePieces[p] = String.Concat(firstPart, shortenLetters, lastPart);
			}

			var namespaceForDB = String.Join("_", namespacePieces);

			return String.Concat(namespaceForDB, "_", entityType.Name);
		}
	}
}
