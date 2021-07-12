using System;
using System.Linq;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Conventions;
using Keon.NHibernate.Mappings;
using Keon.NHibernate.Schema;
using Keon.Util.DB;

namespace Keon.NHibernate.UserPassed
{
	/// <summary>
	/// Information to use auto-mapping of Fluent
	/// </summary>
	/// <typeparam name="Map">Some of the AutoMaps, just for reference</typeparam>
	/// <typeparam name="Entity">Some of the Entities, just for reference</typeparam>
	public class AutoMappingInfo<Map, Entity> where Map : IAutoMappingOverride<Entity>
	{
		/// <summary>
		/// BaseEntities, if it exists, to be ignored on mapping
		/// </summary>
		public Type[] BaseEntities { get; set; }

		/// <summary>
		/// Classes which subclasses use its table
		/// </summary>
		public Type[] SuperEntities { get; set; }

		/// <summary>
		/// Conventions to configure Fluent
		/// </summary>
		public IConvention[] Conventions { get; set; }

		internal AutoPersistenceModel CreateAutoMapping()
		{
			var storeConfiguration = new StoreConfiguration(SuperEntities);
			var assembly = typeof(Entity).Assembly;

			var autoMap = AutoMap
				.Assemblies(storeConfiguration, assembly)
				.UseOverridesFromAssemblyOf<Map>()
				.Conventions.AddFromAssemblyOf<EnumConvention>()
				.Conventions.Add(
					new NullableConvention.Property(),
					new NullableConvention.Reference(),
					new CascadeConvention.OneToMany(),
					new CascadeConvention.ManyToOne(),
					new NameConvention.ManyToMany(),
					new NameConvention.HasMany(),
					new NameConvention.Reference(),
					new NameConvention.TableNameConvention()
				);

			if (BaseEntities != null)
			{
				autoMap = BaseEntities
					.Aggregate(
						autoMap,
						(current, baseEntity) => current.IgnoreBase(baseEntity)
					);
			}

			if (Conventions != null)
				autoMap = autoMap.Conventions.Add(Conventions);

			return autoMap;
		}

		private readonly Type[] entityInterfaces = {
			typeof(IEntity),
			typeof(IEntityShort),
			typeof(IEntityLong),
		};

		private Boolean isEntity(Type type)
		{
			return type.GetInterfaces()
				.Intersect(entityInterfaces)
				.Any();
		}
	}
}
