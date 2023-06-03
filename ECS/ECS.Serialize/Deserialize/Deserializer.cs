using ECS.Schema;
using ECS.Serialize.Models;
using LinqToDB;
using LinqToDB.Data;
using Macropus.Database.Adapter;
using Macropus.ECS.Component;
using Microsoft.Data.Sqlite;

namespace ECS.Serialize.Deserialize;

internal static class Deserializer
{
	public static async Task<IComponent?> DeserializeAsync(DataConnection dataConnection, DataSchema schema, Guid entityId)
	{
		var tableName = schema.SchemaOf.FullName;
		if (tableName == null)
			// TODO
			throw new Exception();

		try
		{
			var componentId = await GetValue(dataConnection, schema, entityId);

			var cmd = DbCommandCache.GetReadCmd(dataConnection.Connection, tableName, schema.Elements);
			cmd.Parameters.Add(new SqliteParameter("@id", componentId));

			using var reader = await cmd.ExecuteReaderAsync();

			await reader.ReadAsync();

			var readResult = SqlComponentReader.ReadComponent(reader, schema);

			var instance = Activator.CreateInstance(schema.SchemaOf);

			foreach (var (element, value) in readResult)
				element.FieldInfo.SetValue(instance, value);

			return instance as IComponent;
		}
		finally
		{
			DbCommandCache.Clear(dataConnection.Connection);
		}
	}

	private static async Task<int> GetValue(DataConnection dataConnection, DataSchema schema, Guid entityId)
	{
		var componentName = schema.SchemaOf.FullName;
		if (componentName == null)
			throw new Exception();

		var entityIdString = ComponentFormatUtils.FormatGuid(entityId);

		var entity = await dataConnection
			.GetTable<EntitiesComponentsTable>()
			.Select(c => new { c.EntityId, c.ComponentId, c.ComponentName })
			.FirstOrDefaultAsync(c => c.EntityId == entityIdString && c.ComponentName == componentName);

		if (entity == null)
			throw new Exception(); // TODO

		var componentId = entity.ComponentId;
		return componentId;
	}
}