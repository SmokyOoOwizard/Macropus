using System.ComponentModel;
using System.Data;
using ECS.Serialize.Deserialize;
using ECS.Serialize.Serialize;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Schema;

namespace ECS.Serialize;

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
		if (schema.Elements.Count == 0 || schema.SubSchemas.Any(s => s.Value.Elements.Count == 0))
			// TODO
			throw new Exception();

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
		if (schema.Elements.Count == 0 || schema.SubSchemas.Any(s => s.Value.Elements.Count == 0))
			// TODO
			throw new Exception();

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