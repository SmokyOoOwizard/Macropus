using System.Reflection;
using Macropus.Schema.Attributes;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public static class DataSchemaUtils
{
	public static DataSchema CreateSchema(Type type, List<DataSchema> subSchemas)
	{
		var schemaName = type.Name;

		var fields = type.GetFields()
			.Where(t => t.IsPublic && !t.FieldType.GetSchemaType().IsSimpleType())
			.ToArray();

		var elements = new List<DataSchemaElement>();
		foreach (var field in fields)
			elements.Add(CreateElement(field, subSchemas));

		return new DataSchema(Guid.NewGuid(), schemaName, elements);
	}

	public static DataSchemaElement CreateElement(FieldInfo field, List<DataSchema> subSchemas)
	{
		var element = new DataSchemaElement();

		var fieldType = field.FieldType;
		ESchemaElementType schemaType;
		if (fieldType.IsArray)
		{
			schemaType = fieldType.GetElementType()!.GetSchemaType();
			element.CollectionType = ECollectionType.Array;
		}
		else
		{
			schemaType = fieldType.GetSchemaType();
		}

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
			var schema = CreateSchema(fieldType, subSchemas);
			subSchemas.Add(schema);
			element.SubSchemaId = schema.Id;
		}

		return element;
	}
}