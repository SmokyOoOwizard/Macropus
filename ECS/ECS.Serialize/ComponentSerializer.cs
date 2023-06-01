﻿using System.Data;
using ECS.Schema;
using ECS.Serialize.Deserialize;
using ECS.Serialize.Serialize;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.ECS.Component;

namespace ECS.Serialize;

// TODO save components in flat mode. one table per one component type. sub structures ( collections, structures, etc... ) save in json?
public partial class ComponentSerializer : IDisposable
{
	private readonly IDbConnection dbConnection;

	private readonly Pool<Serializer> serializers = new();
	private readonly Pool<Deserializer> deserializers = new();

	public ComponentSerializer(IDbConnection dbConnection)
	{
		this.dbConnection = dbConnection;

		dbConnection.Open();
	}

	public async Task SerializeAsync(DataSchema schema, Guid entityId, IComponent component)
	{
		if (schema.Elements.Count == 0 || schema.SubSchemas.Any(s => s.Value.Elements.Count == 0))
			throw new Exception(); // TODO

		var serializer = serializers.Take();

		try
		{
			await serializer.SerializeAsync(dbConnection, schema, entityId, component);
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
			return await deserializer.DeserializeAsync(dbConnection, schema, entityId);
		}
		finally
		{
			deserializers.Release(deserializer);
		}
	}

	public void Dispose()
	{
		dbConnection.Dispose();
	}
}