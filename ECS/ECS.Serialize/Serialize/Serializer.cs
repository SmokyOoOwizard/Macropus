using ECS.Schema;
using ECS.Serialize.Models;
using LinqToDB;
using LinqToDB.Data;

namespace ECS.Serialize.Serialize;

internal static class Serializer
{
	public static async Task SerializeAsync(DataConnection dataConnection, DataSchema schema, Guid entityId, object component)
	{
		await using var transaction = await dataConnection.BeginTransactionAsync();
		try
		{
			var componentId = await SqlSerializer.InsertComponent(dataConnection.Connection, schema, component);

			var componentName = schema.SchemaOf.FullName;
			if (componentName == null)
				throw new Exception(); // TODO
			
			await dataConnection.GetTable<EntitiesComponentsTable>()
				.InsertAsync(() => new()
				{
					ComponentId = componentId,
					ComponentName = componentName,
					EntityId = ComponentFormatUtils.FormatGuid(entityId)
				});

			await transaction.CommitAsync();
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
		finally
		{
			DbCommandCache.Clear(dataConnection.Connection);
		}
	}
}