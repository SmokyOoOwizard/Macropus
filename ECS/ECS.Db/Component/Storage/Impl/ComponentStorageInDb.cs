using System.Data;
using System.Data.Common;
using ECS.Models;
using ECS.Serialize;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Macropus.ECS.Component;
using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Storage;

namespace ECS.Component.Storage.Impl;

public class ComponentsStorageInDb : IComponentsStorage
{
	private readonly DataConnection dbConnection;
	public uint ComponentsCount { get; }
	public uint EntitiesCount { get; }

	public ComponentsStorageInDb(IDbConnection dbConnection)
	{
		this.dbConnection = SQLiteTools.CreateDataConnection((DbConnection)dbConnection);
	}

	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var cmpName = ComponentFormatUtils.NormalizeName(typeof(T).FullName);
		if (string.IsNullOrWhiteSpace(cmpName))
			throw new Exception(); // TODO

		return HasComponent(entityId, cmpName);
	}

	public bool HasComponent(Guid entityId, string name)
	{
		var entityIdStr = ComponentFormatUtils.FormatGuid(entityId);

		return dbConnection
			.GetTable<EntitiesComponentsTable>()
			.Any(e => e.ComponentName == name && e.EntityId == entityIdStr);
	}

	public T GetComponent<T>(Guid entityId) where T : struct, IComponent
	{
		throw new NotImplementedException();
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
		throw new NotImplementedException();
	}

	public void Apply(IReadOnlyComponentsStorage changes)
	{
		throw new NotImplementedException();
	}

	public void Clear() { }

	public void Dispose()
	{
		dbConnection.Dispose();
	}
}