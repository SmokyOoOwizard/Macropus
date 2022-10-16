using System.Reflection;
using Macropus.Schema.Attributes;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public struct DataSchemaElement
{
	public ESchemaElementType Type;
	public bool Nullable;
	public string Name;
	public string FieldName;
	public ECollectionType? CollectionType;

	public Guid? SubSchemaId;


	public override string ToString()
	{
		return
			"{\n"
			+ $"\t Type: {Type}\n "
			+ $"\t Name: {Name}\n"
			+ $"\t FieldName: {FieldName}\n"
			+ $"\t Nullable: {Nullable}\n"
			+ $"\t Collection: {CollectionType}\n"
			+ $"\t Sub schema {SubSchemaId}\n"
			+ "}";
	}

	public static DataSchemaElement Create(FieldInfo field, Func<Type, Guid>? schemaIdFactory = null)
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
			fieldType = System.Nullable.GetUnderlyingType(fieldType);
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
			if (schemaIdFactory == null)
				// TODO it's complex type
				throw new Exception();

			element.SubSchemaId = schemaIdFactory(fieldType!);
		}

		return element;
	}
}