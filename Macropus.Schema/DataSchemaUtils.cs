using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public static class DataSchemaUtils
{
	public static DataSchema CreateSchema<T>(IDataSchemasStorage storage)
	{
		return CreateSchema(typeof(T), storage);
	}

	public static DataSchema CreateSchema(Type type, IDataSchemasStorage storage)
	{
		if (storage.SchemaExists(type))
			return storage.GetSchema(type);

		var schemaName = type.Name;

		var fields = type.GetFields()
			.Where(t => t.IsPublic && t.FieldType.FilterDataSchemaElement())
			.ToArray();

		var elements = new List<DataSchemaElement>();
		foreach (var field in fields)
		{
			elements.Add(DataSchemaElement.Create(field,
				subSchemaType =>
				{
					if (storage.SchemaExists(subSchemaType))
						return storage.GetSchemaId(subSchemaType);

					return CreateSchema(subSchemaType, storage).Id;
				}));
		}

		var schema = new DataSchema(Guid.NewGuid(), schemaName, elements);
		storage.AddSchema(schema, type);
		return schema;
	}
}