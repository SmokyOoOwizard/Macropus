using System.Reflection;
using Macropus.Schema.Attributes;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public static class DataSchemaUtils
{
	public static DataSchema CreateSchema<T>(List<DataSchema> subSchemas)
	{
		var type = typeof(T);
		var schemaName = type.Name;

		var fields = type.GetFields()
			.Where(t => t.IsPublic && t.FieldType.FilterDataSchemaElement())
			.ToArray();

		var elements = new List<DataSchemaElement>();
		var schemas = new Dictionary<Type, DataSchema>();
		foreach (var field in fields)
			elements.Add(CreateElement(field, schemas));

		subSchemas.AddRange(schemas.Values);

		return new DataSchema(Guid.NewGuid(), schemaName, elements);
	}

	private static DataSchema CreateSchema(Type type, Dictionary<Type, DataSchema> subSchemas)
	{
		var schemaName = type.Name;

		var fields = type.GetFields()
			.Where(t => t.IsPublic && t.FieldType.FilterDataSchemaElement())
			.ToArray();

		var elements = new List<DataSchemaElement>();
		foreach (var field in fields)
			elements.Add(CreateElement(field, subSchemas));

		return new DataSchema(Guid.NewGuid(), schemaName, elements);
	}

	private static DataSchemaElement CreateElement(FieldInfo field, Dictionary<Type, DataSchema> subSchemas)
	{
		var element = new DataSchemaElement();

		var fieldType = field.FieldType;
		if (fieldType.IsArray)
		{
			fieldType = fieldType.GetElementType();
			element.CollectionType = ECollectionType.Array;
		}

		if (fieldType!.IsGenericType && (fieldType.GetGenericTypeDefinition() == typeof(Nullable<>)))
		{
			fieldType = Nullable.GetUnderlyingType(fieldType);
			element.Nullable = true;
		}

		var schemaType = fieldType!.GetSchemaType();
		if (schemaType == ESchemaElementType.INVALID)
			// TODO
			throw new Exception();

		element.Type = schemaType;

		element.FieldName = field.Name;

		var fieldCustomName = field.GetCustomAttribute<NameAttribute>();
		if (fieldCustomName != null)
			element.Name = fieldCustomName.Name;
		else
			element.Name = field.Name;

		if (schemaType == ESchemaElementType.ComplexType)
		{
			if (!subSchemas.TryGetValue(fieldType!, out var schema))
			{
				schema = CreateSchema(fieldType!, subSchemas);
				subSchemas.Add(fieldType!, schema);
			}

			element.SubSchemaId = schema.Id;
		}

		return element;
	}
}