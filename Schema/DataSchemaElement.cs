using System.Reflection;
using Macropus.Schema.Attributes;
using Macropus.Schema.Exceptions;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public struct DataSchemaElement
{
	public ColdDataSchemaElement Info;

	public FieldInfo FieldInfo;

	public override string ToString()
	{
		return
			"{\n"
			+ $"\t Type: {Info.Type}\n "
			+ $"\t Name: {Info.Name}\n"
			+ $"\t FieldName: {Info.FieldName}\n"
			+ $"\t Nullable: {Info.Nullable}\n"
			+ $"\t Collection: {Info.CollectionType}\n"
			+ $"\t Sub schema {Info.SubSchemaId}\n"
			+ "}";
	}

	public static DataSchemaElement Create(FieldInfo field, Func<Type, uint>? schemaIdFactory = null)
	{
		var element = new DataSchemaElement
		{
			FieldInfo = field
		};

		var fieldType = field.FieldType;
		if (fieldType.IsArray)
		{
			fieldType = fieldType.GetElementType();
			element.Info.CollectionType = ECollectionType.Array;
		}

		if (fieldType == typeof(string))
		{
			element.Info.Nullable = true;
		}

		if (fieldType!.IsGenericType && (fieldType.GetGenericTypeDefinition() == typeof(Nullable<>)))
		{
			fieldType = Nullable.GetUnderlyingType(fieldType);
			element.Info.Nullable = true;
		}

		var schemaType = fieldType!.GetSchemaType();
		if (schemaType == ESchemaElementType.INVALID)
			throw new UnableCreateSchemaForTypeException(fieldType!);

		element.Info.Type = schemaType;

		element.Info.FieldName = field.Name;

		var fieldCustomName = field.GetCustomAttribute<NameAttribute>();
		if (fieldCustomName != null)
			element.Info.Name = fieldCustomName.Name;
		else
			element.Info.Name = field.Name;

		if (schemaType == ESchemaElementType.ComplexType)
		{
			if (schemaIdFactory == null)
				throw new NullIdFactoryException();

			element.Info.SubSchemaId = schemaIdFactory(fieldType!);
		}

		return element;
	}
}