using System.Reflection;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public class CachedDataSchema
{
	public readonly DataSchema Schema;
	public readonly Type Type;
	public readonly IReadOnlyDictionary<DataSchemaElement, FieldInfo> CachedFields;

	private CachedDataSchema(
		Type type,
		DataSchema schema,
		IReadOnlyDictionary<DataSchemaElement, FieldInfo> cachedFields
	)
	{
		Type = type;
		Schema = schema;
		CachedFields = cachedFields;
	}

	public static CachedDataSchema Create<T>(IDataSchemasStorage subSchemasStorage)
	{
		return Create(typeof(T), DataSchemaUtils.CreateSchema<T>(subSchemasStorage), subSchemasStorage);
	}

	public static CachedDataSchema Create<T>(DataSchema schema, IDataSchemasStorage subSchemasStorage)
	{
		return Create(typeof(T), schema, subSchemasStorage);
	}

	public static CachedDataSchema Create(Type type, DataSchema schema, IDataSchemasStorage subSchemasStorage)
	{
		if (!schema.IsFullCorrectType(type, subSchemasStorage))
			// TODO
			throw new Exception();

		var cachedFields = new Dictionary<DataSchemaElement, FieldInfo>();

		var typeFields = type.GetFields()
			.Where(f => f.IsPublic && f.FieldType.FilterDataSchemaElement())
			.ToHashSet();

		foreach (var schemaElement in schema.Elements)
		{
			var targetField = typeFields.FirstOrDefault(f => f.Name == schemaElement.FieldName);
			if (targetField == null)
				// TODO
				throw new Exception();

			cachedFields.Add(schemaElement, targetField);
			typeFields.Remove(targetField);
		}

		return new CachedDataSchema(type, schema, cachedFields);
	}
}