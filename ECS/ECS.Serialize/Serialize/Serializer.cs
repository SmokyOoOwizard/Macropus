using System.Data;
using ECS.Schema;
using ECS.Serialize.Sql;
using Macropus.CoolStuff;
using Macropus.ECS.Component;

namespace ECS.Serialize.Serialize;

class Serializer : IClearable
{
	private readonly SqlSerializer serializer = new();

	public async Task SerializeAsync(IDbConnection dbConnection, DataSchema schema, Guid entityId, IComponent component)
	{
		using var transaction = dbConnection.BeginTransaction();
		try
		{
			var componentId = await serializer.InsertComponent(dbConnection, schema, component);

			var componentName = schema.SchemaOf.FullName;
			await serializer.AddEntityComponent(dbConnection, componentId, componentName!, entityId)
				.ConfigureAwait(false);

			transaction.Commit();
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
		finally
		{
			DbCommandCache.Clear(dbConnection);
		}
	}

	public void Clear()
	{
		serializer.Clear();
	}
}