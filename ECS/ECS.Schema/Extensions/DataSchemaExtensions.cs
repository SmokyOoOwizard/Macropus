namespace ECS.Schema.Extensions;

public static class DataSchemaExtensions
{
	public static bool IsCorrectType<T>(this ColdDataSchema schema)
	{
		return IsCorrectType(schema, typeof(T));
	}

	public static bool IsCorrectType(this ColdDataSchema schema, Type? type)
	{
		if (type == null)
			return false;

		try
		{
			var checkedTypes = new HashSet<Type>();

			var schemasQueue = new Queue<KeyValuePair<Type, IReadOnlyCollection<ColdDataSchemaElement>>>();
			schemasQueue.Enqueue(
				new KeyValuePair<Type, IReadOnlyCollection<ColdDataSchemaElement>>(type, schema.Elements));

			do
			{
				var target = schemasQueue.Dequeue();
				var targetType = target.Key;
				var targetElements = target.Value;

				var targetFields = targetType.GetFields()
					.Where(f => f.IsPublic && f.FieldType.FilterDataSchemaElement())
					.ToHashSet();

				foreach (var schemaElement in targetElements)
				{
					var targetField = targetFields.FirstOrDefault(f => f.Name == schemaElement.FieldName);
					if (targetField == null)
						return false;

					var targetFieldType = targetField.FieldType.GetSchemaType();
					if (targetFieldType != schemaElement.Type)
						return false;

					var targetSchemaElement = DataSchemaElement.Create(targetField, _ => 0);

					targetSchemaElement.Info.SubSchemaId = schemaElement.SubSchemaId;
					if (!schemaElement.Equals(targetSchemaElement.Info))
						return false;

					if ((schemaElement.Type == ESchemaElementType.ComplexType)
					    && schemaElement.SubSchemaId.HasValue)
					{
						var subSchema = schema.SubSchemas[schemaElement.SubSchemaId.Value];

						var fieldType = targetField.FieldType.GetSchemaRealType();
						if (fieldType == null)
							return false;

						if (!checkedTypes.Contains(fieldType))
						{
							schemasQueue.Enqueue(
								new KeyValuePair<Type, IReadOnlyCollection<ColdDataSchemaElement>>(fieldType,
									subSchema));
						}
					}

					checkedTypes.Add(targetType);
					targetFields.Remove(targetField);
				}
			} while (schemasQueue.Count > 0);

			return true;
		}
		catch
		{
			return false;
		}
	}
}