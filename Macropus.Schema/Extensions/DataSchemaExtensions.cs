namespace Macropus.Schema.Extensions;

public static class DataSchemaExtensions
{
	public static bool IsCorrectType<T>(this DataSchema schema)
	{
		return IsCorrectType(schema, typeof(T));
	}

	public static bool IsCorrectType(this DataSchema schema, Type? type)
	{
		if (type == null)
			return false;

		try
		{
			var targetFields = type.GetFields()
				.Where(f => f.IsPublic && f.FieldType.FilterDataSchemaElement())
				.ToHashSet();

			foreach (var schemaElement in schema.Elements)
			{
				var targetField = targetFields.FirstOrDefault(f => f.Name == schemaElement.FieldName);
				if (targetField == null)
					return false;

				var targetFieldType = targetField.FieldType.GetSchemaType();
				if (targetFieldType != schemaElement.Type)
					return false;

				var targetSchemaElement = DataSchemaElement.Create(targetField,
					t => schema.SubSchemas.FirstOrDefault(q => q.Value.SchemaOf == t).Key);

				if (!schemaElement.Equals(targetSchemaElement))
					return false;

				if ((schemaElement.Type == ESchemaElementType.ComplexType) && schemaElement.SubSchemaId.HasValue)
				{
					var subSchema = schema.SubSchemas[schemaElement.SubSchemaId.Value];

					var fieldType = targetField.FieldType.GetSchemaRealType();
					if (!subSchema.IsCorrectType(fieldType))
						return false;
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