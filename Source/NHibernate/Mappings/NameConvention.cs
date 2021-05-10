using System;
using System.Linq;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Keon.NHibernate.Mappings
{
    internal class NameConvention
    {
        internal class ManyToMany : ManyToManyTableNameConvention
        {
            protected override String GetBiDirectionalTableName(
	            IManyToManyCollectionInspector collection,
	            IManyToManyCollectionInspector otherSide
	        )
            {
	            var firstEntity = collection.EntityType.Name.ToLower();
	            var secondEntity = otherSide.EntityType.Name.ToLower();
	            return $"{firstEntity}_{secondEntity}";
            }

			protected override String GetUniDirectionalTableName(
				IManyToManyCollectionInspector collection
			)
			{
				var entityName = collection.EntityType.Name.ToLower();
				var childName = collection.ChildType.Name.ToLower();
				return $"{entityName}_{childName}";
			}
		}

        internal class Reference : IReferenceConvention
        {
            public void Apply(IManyToOneInstance instance)
            {
                var propertyName = putID(instance.Property.Name);

                instance.Column(propertyName);

                var entityType = instance.EntityType.Name;
                instance.ForeignKey($"FK_{entityType}_{instance.Name}");
            }
        }

		internal class TableNameConvention : IClassConvention
		{
			public void Apply(IClassInstance instance)
			{
				var tableName = GetTableNameWithNamespace(
					instance.EntityType
				);

				instance.Table(tableName.ToLower());
			}
		}

		internal class HasMany : IHasManyConvention
        {
            public void Apply(IOneToManyCollectionInstance instance)
            {
				var referenceName = instance.Member.Name;

				referenceName = referenceName.EndsWith("List")
					? referenceName[..^4]
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

		/// <summary>
		/// Rules for table naming from entity type
		/// </summary>
		public static string GetTableNameWithNamespace(Type entityType)
		{
			var dllName = entityType.Assembly.ManifestModule.Name;
			var mainNamespace = dllName.Replace(".dll", "");

			if (entityType.Namespace == null || entityType.Namespace == mainNamespace)
				return entityType.Name;

			var @namespace = entityType.Namespace[(mainNamespace.Length + 1)..];

			var namespacePieces = @namespace
				.Split('.')
				.Select(shortenName);

			var namespaceForDB = String.Join("_", namespacePieces);

			return String.Concat(namespaceForDB, "_", entityType.Name);
		}

		private const int firstLetters = 2;
		private const int lastLetters = 2;
		private const int totalLetters = firstLetters + lastLetters;

		private static String shortenName(String text)
		{
			var count = text.Length;

			if (count <= totalLetters)
				return text;

			var firstPart = text[..firstLetters];
			var shortenLetters = count - totalLetters;
			var lastPart = text.Substring(count - lastLetters, lastLetters);

			return String.Concat(firstPart, shortenLetters, lastPart);
		}
    }
}
