using System.Data;
using Macropus.CoolStuff.Collections;
using Macropus.ECS.Component;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

public partial class ComponentSerializer : IDisposable
{
	private readonly IDbConnection dbConnection;

	private readonly Pool<Serializer> serilizers = new();
	private readonly Pool<Deserializer> deserilizers = new();

	public ComponentSerializer(IDbConnection dbConnection)
	{
		this.dbConnection = dbConnection;

		dbConnection.Open();
	}

	public async Task SerializeAsync<T>(DataSchema schema, Guid entityId, T component) where T : struct, IComponent
	{
		var serializer = serilizers.Take();

		try
		{
			await serializer.SerializeAsync(dbConnection, schema, entityId, component);
		}
		finally
		{
			serilizers.Release(serializer);
		}
	}

	public async Task<T?> DeserializeAsync<T>(DataSchema schema, Guid entityId) where T : struct, IComponent
	{
		var deserializer = deserilizers.Take();

		try
		{
			return await deserializer.DeserializeAsync<T>(dbConnection, schema, entityId);
		}
		finally
		{
			deserilizers.Release(deserializer);
		}
	}

	public void Dispose()
	{
		dbConnection.Dispose();
	}
}