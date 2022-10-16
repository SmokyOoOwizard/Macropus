using System.Collections;

namespace Macropus.Schema.Impl;

public class DataSchemasMemoryStorage : IDataSchemasStorage, IEnumerable<DataSchema>
{
	private readonly Dictionary<Guid, DataSchema> schemas = new();
	private readonly Dictionary<Type, Guid> types = new();


	public void AddSchema(DataSchema schema)
	{
		schemas[schema.Id] = schema;
	}

	public void AddSchema<T>(DataSchema schema)
	{
		var type = typeof(T);

		types[type] = schema.Id;
		schemas[schema.Id] = schema;
	}

	public void AddSchema(DataSchema schema, Type type)
	{
		types[type] = schema.Id;
		schemas[schema.Id] = schema;
	}

	public bool SchemaExists(Guid id)
	{
		return schemas.ContainsKey(id);
	}

	public bool SchemaExists<T>()
	{
		var type = typeof(T);

		if (!types.TryGetValue(type, out var schemaId))
			return false;

		return schemas.ContainsKey(schemaId);
	}

	public bool SchemaExists(Type type)
	{
		if (!types.TryGetValue(type, out var schemaId))
			return false;

		return schemas.ContainsKey(schemaId);
	}

	public DataSchema GetSchema(Guid id)
	{
		return schemas[id];
	}

	public DataSchema GetSchema<T>()
	{
		var type = typeof(T);

		var schemaId = types[type];

		return schemas[schemaId];
	}

	public DataSchema GetSchema(Type type)
	{
		var schemaId = types[type];

		return schemas[schemaId];
	}

	public Guid GetSchemaId<T>()
	{
		var type = typeof(T);

		var schemaId = types[type];

		return schemas[schemaId].Id;
	}

	public Guid GetSchemaId(Type type)
	{
		var schemaId = types[type];

		return schemas[schemaId].Id;
	}

	public IEnumerator<DataSchema> GetEnumerator()
	{
		return schemas.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return schemas.Values.GetEnumerator();
	}
}