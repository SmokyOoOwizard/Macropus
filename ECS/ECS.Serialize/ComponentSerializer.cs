using ECS.Schema;
using ECS.Serialize.Deserialize;
using ECS.Serialize.Serialize;
using LinqToDB.Data;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.ECS.Component;

namespace ECS.Serialize;

// TODO save components in flat mode. one table per one component type. sub structures ( collections, structures, etc... ) save in json?
public partial class ComponentSerializer : IDisposable
{
	private readonly DataConnection dataConnection;

	private readonly Pool<Serializer> serializers = new();
	private readonly Pool<Deserializer> deserializers = new();

	public ComponentSerializer(DataConnection dataConnection)
	{
		this.dataConnection = dataConnection;
	}

	public async Task SerializeAsync(DataSchema schema, Guid entityId, IComponent component)
	{
		if (schema.Elements.Count == 0 || schema.SubSchemas.Any(s => s.Value.Elements.Count == 0))
			throw new Exception(); // TODO

		var serializer = serializers.Take();

		try
		{
			await serializer.SerializeAsync(dataConnection.Connection, schema, entityId, component);
		}
		finally
		{
			serializers.Release(serializer);
		}
	}

	public async Task<T?> DeserializeAsync<T>(DataSchema schema, Guid entityId) where T : struct, IComponent
	{
		var schemaType = schema.SchemaOf;
		var componentType = typeof(T);
		if (!(schemaType == componentType || componentType.IsSubclassOf(schemaType)))
			return default(T);

		return (T?)await DeserializeAsync(schema, entityId);
	}

	public async Task<IComponent?> DeserializeAsync(DataSchema schema, Guid entityId)
	{
		if (schema.Elements.Count == 0 || schema.SubSchemas.Any(s => s.Value.Elements.Count == 0))
			throw new Exception(); // TODO

		var deserializer = deserializers.Take();

		try
		{
			return await deserializer.DeserializeAsync(dataConnection.Connection, schema, entityId);
		}
		finally
		{
			deserializers.Release(deserializer);
		}
	}

	public void Dispose()
	{
		dataConnection.Dispose();
	}
}