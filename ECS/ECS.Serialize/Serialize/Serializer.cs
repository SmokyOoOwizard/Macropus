using ECS.Schema;
using ECS.Serialize.Models;
using LinqToDB;
using LinqToDB.Data;

namespace ECS.Serialize.Serialize;

internal class Serializer
{
	private readonly SqlSerializer serializer = new();

	public async Task SerializeAsync(DataConnection dataConnection, DataSchema schema, Guid entityId, object component)
	{
		await using var transaction = await dataConnection.BeginTransactionAsync();
		try
		{
			var componentId = await serializer.InsertComponent(dataConnection.Connection, schema, component);

			var componentName = schema.SchemaOf.FullName;
			await dataConnection.GetTable<EntitiesComponentsTable>()
				.InsertAsync(() => new EntitiesComponentsTable()
				{
					ComponentId = componentId,
					ComponentName = componentName,
					EntityId = entityId.ToString("N")
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