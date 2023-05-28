using Macropus.Extensions;

namespace ECS.Schema.Extensions;

// TODO very ugly solution
public static class DataSchemaPackExtensions
{
	public static ColdDataSchema Pack(this DataSchema schema)
	{
		var elements = schema.Elements.Select(e => e.Info).ToArray();
		var subSchemas = schema.SubSchemas
			.ToDictionary(kv => kv.Key, kv => kv.Value.Elements
				.Select(e => e.Info)
				.ToArray()
				.AsReadOnlyCollection());

		return new ColdDataSchema(elements, subSchemas);
	}

	public static DataSchema Unpack<T>(this ColdDataSchema coldSchema)
	{
		return Unpack(coldSchema, typeof(T));
	}

	public static DataSchema Unpack(this ColdDataSchema coldSchema, Type type)
	{
		var rawSchemas = new Dictionary<DataSchema, (List<DataSchemaElement>, Dictionary<uint, DataSchema>)>();
		var idToSchemaMap = new Dictionary<uint, DataSchema>();

		UnpackSchemasElements(coldSchema, type, rawSchemas, idToSchemaMap);

		foreach (var rawSchema in rawSchemas)
		{
			var subSchemas = rawSchema.Value.Item2;
			if (subSchemas.Count == 0)
				continue;

			var queue = new Queue<uint>(rawSchema.Value.Item2.Keys);
			do
			{
				var id = queue.Dequeue();

				subSchemas.TryGetValue(id, out var subSchema);
				if (subSchema == null)
				{
					subSchema = idToSchemaMap[id];
					subSchemas[id] = subSchema;

					queue.AddRange(subSchema.SubSchemas.Keys);
				}
			} while (queue.Count > 0);
		}

		return idToSchemaMap[0];
	}

	private static void UnpackSchemasElements(
		ColdDataSchema coldSchema,
		Type type,
		Dictionary<DataSchema, (List<DataSchemaElement>, Dictionary<uint, DataSchema>)> rawSchemas,
		Dictionary<uint, DataSchema> idToSchemaMap
	)
	{
		var schemasQueue = new Queue<KeyValuePair<Type, (uint, ColdDataSchemaElement[])>>();
		schemasQueue.Enqueue(
			new KeyValuePair<Type, (uint, ColdDataSchemaElement[])>(type,
				(0, coldSchema.Elements.ToArray())));

		var unpackedElements = new List<DataSchemaElement>();
		var processedTypes = new Dictionary<Type, DataSchema>();

		do
		{
			var target = schemasQueue.Dequeue();
			var targetType = target.Key;
			var targetElements = target.Value.Item2;

			var targetFields = targetType.GetFields()
				.Where(f => f.IsPublic && f.FieldType.FilterDataSchemaElement())
				.ToHashSet();

			unpackedElements.Clear();

			foreach (var coldElement in targetElements)
			{
				var targetField = targetFields.FirstOrDefault(f => f.Name == coldElement.FieldName);
				if (targetField == null)
					// TODO
					throw new Exception();

				var targetFieldType = targetField.FieldType.GetSchemaType();
				if (targetFieldType != coldElement.Type)
					// TODO
					throw new Exception();

				var targetSchemaElement = DataSchemaElement.Create(targetField, _ => 0);

				targetSchemaElement.Info.SubSchemaId = coldElement.SubSchemaId;
				if (!coldElement.Equals(targetSchemaElement.Info))
					// TODO
					throw new Exception();


				if ((coldElement.Type == ESchemaElementType.ComplexType)
				    && coldElement.SubSchemaId.HasValue)
				{
					var subSchema = coldSchema.SubSchemas[coldElement.SubSchemaId.Value];

					var fieldType = targetField.FieldType.GetSchemaRealType();
					if (fieldType == null)
						// TODO
						throw new Exception();

					if (!processedTypes.ContainsKey(fieldType))
					{
						schemasQueue.Enqueue(
							new KeyValuePair<Type, (uint, ColdDataSchemaElement[])>(fieldType,
								(coldElement.SubSchemaId.Value, subSchema.ToArray())));
					}
				}

				var schemaElement = new DataSchemaElement
				{
					Info = coldElement,
					FieldInfo = targetField
				};
				unpackedElements.Add(schemaElement);

				targetFields.Remove(targetField);
			}

			if (!processedTypes.ContainsKey(targetType))
			{
				var elements = new List<DataSchemaElement>(unpackedElements);
				var subSchemas = new Dictionary<uint, DataSchema>();
				foreach (var element in elements)
				{
					if (element.Info.SubSchemaId.HasValue)
					{
						subSchemas[element.Info.SubSchemaId.Value] = null!;
					}
				}

				var schema = new DataSchema(targetType, elements, subSchemas);

				processedTypes.Add(targetType, schema);
				rawSchemas.Add(schema, (elements, subSchemas));
			}

			idToSchemaMap[target.Value.Item1] = processedTypes[targetType];
		} while (schemasQueue.Count > 0);
	}
}