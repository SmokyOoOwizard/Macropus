using ECS.Db.Models;
using ECS.Schema;
using ECS.Serialize;
using ECS.Serialize.Models;
using LinqToDB;
using LinqToDB.Data;
using Macropus.ECS.Component;
using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage;

namespace ECS.Db.Storage.Impl;

// TODO db leak. after component delete action in db stay sub structures
public class ComponentsStorageInDb : IComponentsStorage
{
	// TODO optimize
	public uint ComponentsCount => (uint)dataConnection.GetTable<EntitiesComponentsTable>()
		.TableName("EntitiesComponents")
		.Count();

	// TODO optimize
	public uint EntitiesCount => (uint)dataConnection.GetTable<EntitiesComponentsTable>()
		.TableName("EntitiesComponents")
		.GroupBy(c => c.EntityId, c => c.ComponentName)
		.Count();

	private readonly DataConnection dataConnection;
	private readonly ComponentSerializer componentSerializer;

	private readonly Dictionary<Type, DataSchema> schemas = new();
	private readonly DataSchemaBuilder schemaBuilder = new();

	public ComponentsStorageInDb(DataConnection dataConnection)
	{
		// TODO pass data connection directly?
		this.dataConnection = dataConnection;

		componentSerializer = new ComponentSerializer(dataConnection);
	}

	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var cmpName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(cmpName))
			throw new Exception(); // TODO

		return HasComponent(entityId, cmpName);
	}

	// TODO optimize
	public bool HasComponent(Guid entityId, string name)
	{
		var entityIdStr = ComponentFormatUtils.FormatGuid(entityId);

		return dataConnection
			.GetTable<EntitiesComponentsTable>()
			.TableName("EntitiesComponents")
			.Any(e => e.ComponentName == name && e.EntityId == entityIdStr);
	}

	// TODO optimize
	public T GetComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentType = typeof(T);
		if (!schemas.TryGetValue(componentType, out var schema))
		{
			schema = schemaBuilder.CreateSchema<T>();
			schemas[componentType] = schema;
		}

		var result = componentSerializer.DeserializeAsync<T>(schema, entityId)
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();

		if (!result.HasValue)
			throw new Exception(); // TODO

		return result.Value;
	}

	public IComponent GetComponent(Guid entityId, string name)
	{
		var schema = schemas.FirstOrDefault(s => s.Key.FullName == name).Value;
		if (schema == null)
			throw new Exception(); // TODO

		var result = componentSerializer.DeserializeAsync(schema, entityId)
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();

		if (result == null)
			throw new Exception(); // TODO

		return result;
	}

	// TODO optimize
	public IEnumerable<Guid> GetEntities()
	{
		return dataConnection.GetTable<EntitiesComponentsTable>()
			.TableName("EntitiesComponents")
			.Select(c => c.EntityId)
			.Select(c => Guid.Parse(c))
			.ToHashSet();
	}

	// TODO optimize
	public IEnumerable<Guid> GetEntities(ComponentsFilter filter)
	{
		var entities = dataConnection.GetTable<EntitiesComponentsTable>()
			.TableName("EntitiesComponents")
			.GroupBy(c => c.EntityId, c => c.ComponentName)
			.Select(c => new
			{
				Id = Guid.Parse(c.Key),
				Components = c.ToHashSet()
			})
			.ToHashSet();


		return entities
			.Where(c => filter.Filter(c.Components))
			.Select(c => c.Id);
	}

	// TODO optimize
	public void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent
	{
		var componentType = typeof(T);
		if (!schemas.TryGetValue(componentType, out var schema))
		{
			schema = schemaBuilder.CreateSchema<T>();
			schemas[componentType] = schema;
		}

		RemoveComponent<T>(entityId);

		componentSerializer.SerializeAsync(schema, entityId, component).GetAwaiter().GetResult();
	}

	public void ReplaceComponent(Guid entityId, IComponent component)
	{
		var componentType = component.GetType();
		if (!schemas.TryGetValue(componentType, out var schema))
		{
			schema = schemaBuilder.CreateSchema(componentType);
			schemas[componentType] = schema;
		}

		RemoveComponent(entityId, componentType);

		componentSerializer.SerializeAsync(schema, entityId, component).GetAwaiter().GetResult();
	}

	// TODO optimize
	public void RemoveComponent<T>(Guid entityId) where T : struct, IComponent
	{
		RemoveComponent(entityId, typeof(T));
	}

	public void RemoveComponent(Guid entityId, Type cmpType)
	{
		var entityIdStr = ComponentFormatUtils.FormatGuid(entityId);
		var cmpName = cmpType.FullName;
		var tableName = ComponentFormatUtils.NormalizeName(cmpName);

		var ids = dataConnection
			.GetTable<EntitiesComponentsTable>()
			.TableName("EntitiesComponents")
			.Where(e => e.ComponentName == cmpName && e.EntityId == entityIdStr)
			.Select(c => new { c.ComponentId, c.Id })
			.FirstOrDefault();

		if (ids == null)
			return;

		dataConnection
			.GetTable<ComponentTableBase>()
			.TableName(tableName)
			.Where(e => e.Id == ids.ComponentId)
			.Delete();

		dataConnection
			.GetTable<EntitiesComponentsTable>()
			.TableName("EntitiesComponents")
			.Where(e => e.Id == ids.Id)
			.Delete();
	}

	// TODO optimize
	public void Apply(IReadOnlyComponentsChangesStorage changes)
	{
		var storages = changes.GetComponents();
		foreach (var storage in storages)
		{
			var cmpName = storage.ComponentName;
			var tableName = ComponentFormatUtils.NormalizeName(cmpName);

			var toRemove = storage.Select(c => ComponentFormatUtils.FormatGuid(c.Key) ?? "").ToHashSet();

			var toAdd = storage.Where(c => c.Value != null).ToList();


			// TODO holy shit.....
			{
				// Remove components
				var toRemoveIds = dataConnection
					.GetTable<EntitiesComponentsTable>()
					.TableName("EntitiesComponents")
					.Where(e => e.ComponentName == cmpName && toRemove.Contains(e.EntityId))
					.Join(dataConnection.GetTable<ComponentTableBase>().TableName(tableName), e => e.ComponentId,
						e => e.Id,
						(e, c) => c)
					.Select(e => e.Id)
					.ToHashSet();

				dataConnection
					.GetTable<ComponentTableBase>()
					.TableName(tableName)
					.Where(e => toRemoveIds.Contains(e.Id))
					.Delete();

				dataConnection
					.GetTable<EntitiesComponentsTable>()
					.TableName("EntitiesComponents")
					.Where(e => e.ComponentName == cmpName && toRemoveIds.Contains(e.ComponentId))
					.Delete();
			}
			{
				foreach (var (id, component) in toAdd)
				{
					ReplaceComponent(id, component!);
				}
			}
		}
	}

	public void Clear()
	{
		// TODO
	}

	public void Dispose()
	{
		dataConnection.Dispose();
	}
}