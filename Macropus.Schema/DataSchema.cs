using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public sealed class DataSchema
{
	public readonly Type SchemaOf;

	public readonly IReadOnlyCollection<DataSchemaElement> Elements;
	public readonly IReadOnlyDictionary<uint, DataSchema> SubSchemas;

	public DataSchema(
		Type type,
		IReadOnlyCollection<DataSchemaElement> elements,
		IReadOnlyDictionary<uint, DataSchema> subSchemas
	)
	{
		SchemaOf = type;
		Elements = elements;
		SubSchemas = subSchemas;
	}


	public bool IsCorrectType<T>()
	{
		return IsCorrectType(typeof(T));
	}

	public bool IsCorrectType(Type type)
	{
		try
		{
			var targetFields = type.GetFields()
				.Where(f => f.IsPublic && f.FieldType.FilterDataSchemaElement())
				.ToHashSet();

			foreach (var schemaElement in Elements)
			{
				var targetField = targetFields.FirstOrDefault(f => f.Name == schemaElement.FieldName);
				if (targetField == null)
					return false;

				var targetFieldType = targetField.FieldType.GetSchemaType();
				if (targetFieldType != schemaElement.Type)
					return false;

				var targetSchemaElement = DataSchemaElement.Create(targetField,
					t => SubSchemas.FirstOrDefault(q => q.Value.SchemaOf == t).Key);

				if (!schemaElement.Equals(targetSchemaElement))
					return false;

				if ((schemaElement.Type == ESchemaElementType.ComplexType) && schemaElement.SubSchemaId.HasValue)
				{
					// TODO check sub schema
				}

				targetFields.Remove(targetField);
			}

			return true;
		}
		catch
		{
			return false;
		}
	}
}