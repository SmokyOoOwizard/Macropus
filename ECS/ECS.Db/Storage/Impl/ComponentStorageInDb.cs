using System.Data;
using System.Data.Common;
using ECS.Db.Models;
using ECS.Schema;
using ECS.Serialize;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Macropus.ECS.Component;
using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage;

namespace ECS.Db.Storage.Impl;

public class ComponentsStorageInDb : IComponentsStorage
{
	public uint ComponentsCount { get; }
	public uint EntitiesCount { get; }

	private readonly DataConnection dbConnection;
	private readonly ComponentSerializer componentSerializer;

	private readonly Dictionary<Type, DataSchema> schemas = new();
	private readonly DataSchemaBuilder schemaBuilder = new();

	public ComponentsStorageInDb(IDbConnection dbConnection)
	{
		this.dbConnection = SQLiteTools.CreateDataConnection((DbConnection)dbConnection);
		
		componentSerializer = new ComponentSerializer(dbConnection);
	}

	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var cmpName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(cmpName))
			throw new Exception(); // TODO

		return HasComponent(entityId, cmpName);
	}

	public bool HasComponent(Guid entityId, string name)
	{
		var entityIdStr = ComponentFormatUtils.FormatGuid(entityId);

		return dbConnection
			.GetTable<EntitiesComponentsTable>()
			.TableName("EntitiesComponents")
			.Any(e => e.ComponentName == name && e.EntityId == entityIdStr);
	}

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
		throw new NotImplementedException();
	}

	public IEnumerable<Guid> GetEntities()
	{
		throw new NotImplementedException();
	}

	public IEnumerable<Guid> GetEntities(ComponentsFilter filter)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<IReadOnlyComponentStorage> GetComponents()
	{
		throw new NotImplementedException();
	}

	public void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent
	{
		throw new NotImplementedException();
	}

	public void RemoveComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var entityIdStr = ComponentFormatUtils.FormatGuid(entityId);
		var cmpName = typeof(T).FullName;
		var tableName = ComponentFormatUtils.NormalizeName(cmpName);

		var ids = dbConnection
			.GetTable<EntitiesComponentsTable>()
			.TableName("EntitiesComponents")
			.Where(e => e.ComponentName == cmpName && e.EntityId == entityIdStr)
			.Select(c => new { c.ComponentId, c.Id })
			.FirstOrDefault();

		if (ids == null)
			return;

		dbConnection
			.GetTable<ComponentTableBase>()
			.TableName(tableName)
			.Where(e => e.Id == ids.ComponentId)
			.Delete();

		dbConnection
			.GetTable<EntitiesComponentsTable>()
			.TableName("EntitiesComponents")
			.Where(e => e.Id == ids.Id)
			.Delete();
	}

	public void Apply(IReadOnlyComponentsStorage changes)
	{
		var components = changes.GetComponents();
		foreach (var component in components)
		{
			var cmpName = component.ComponentName;
			var tableName = ComponentFormatUtils.NormalizeName(cmpName);

			var toRemove = new HashSet<string>();

			foreach (var componentChanges in component)
			{
				if (componentChanges.Value == null)
					toRemove.Add(ComponentFormatUtils.FormatGuid(componentChanges.Key) ?? "");

				// TODO changes
			}

			{ // Remove components
				var toRemoveIds = dbConnection
					.GetTable<EntitiesComponentsTable>()
					.TableName("EntitiesComponents")
					.Where(e => e.ComponentName == cmpName && toRemove.Contains(e.EntityId))
					.Join(dbConnection.GetTable<ComponentTableBase>().TableName(tableName), e => e.ComponentId,
						e => e.Id,
						(e, c) => c)
					.Select(e => e.Id)
					.ToHashSet();


				dbConnection
					.GetTable<ComponentTableBase>()
					.TableName(tableName)
					.Where(e => toRemoveIds.Contains(e.Id))
					.Delete();

				dbConnection
					.GetTable<EntitiesComponentsTable>()
					.TableName("EntitiesComponents")
					.Where(e => toRemoveIds.Contains(e.ComponentId))
					.Delete();
			}
		}

		throw new NotImplementedException();
	}

	public void Clear() { }

	public void Dispose()
	{
		dbConnection.Dispose();
	}
}