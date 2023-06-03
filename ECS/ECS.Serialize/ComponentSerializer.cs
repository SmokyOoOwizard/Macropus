﻿using ECS.Schema;
using ECS.Serialize.Deserialize;
using ECS.Serialize.Serialize;
using LinqToDB.Data;
using Macropus.ECS.Component;

namespace ECS.Serialize;

public partial class ComponentSerializer : IDisposable
{
	private readonly DataConnection dataConnection;

	private readonly Serializer serializer = new();
	private readonly Deserializer deserializer = new();

	public ComponentSerializer(DataConnection dataConnection)
	{
		this.dataConnection = dataConnection;
	}

	public async Task SerializeAsync(DataSchema schema, Guid entityId, IComponent component)
	{
		await serializer.SerializeAsync(dataConnection, schema, entityId, component);
	}

	public async Task<T?> DeserializeAsync<T>(DataSchema schema, Guid entityId) where T : struct, IComponent
	{
		var schemaType = schema.SchemaOf;
		var componentType = typeof(T);
		if (!(schemaType == componentType || componentType.IsSubclassOf(schemaType)))
			return default(T);

		return (T?)await DeserializeAsync(schema, entityId);
	}

	public Task<IComponent?> DeserializeAsync(DataSchema schema, Guid entityId)
	{
		return deserializer.DeserializeAsync(dataConnection, schema, entityId);
	}

	public void Dispose()
	{
		dataConnection.Dispose();
	}
}