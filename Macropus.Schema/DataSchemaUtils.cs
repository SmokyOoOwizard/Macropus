using Macropus.CoolStuff;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public static class DataSchemaUtils
{
	public static DataSchema CreateSchema<T>()
	{
		return CreateSchema(typeof(T));
	}

	public static DataSchema CreateSchema(Type type)
	{
		uint lastId = 0;
		var alreadyExistsSchemas = new Map<Type, uint>();
		alreadyExistsSchemas.Add(type, lastId++);

		var toCreateSchemas = new Dictionary<Type, uint>();

		uint CreateIdFunc(Type t)
		{
			if (toCreateSchemas.ContainsKey(t))
				return toCreateSchemas[t];

			lastId++;
			toCreateSchemas.Add(t, lastId);
			return lastId;
		}

		var rootElements = CreateSchema(type, alreadyExistsSchemas, CreateIdFunc);
		var subSchemas = new List<KeyValuePair<Type, DataSchemaElement[]>>();
		do
		{
			var firstPair = toCreateSchemas.FirstOrDefault();

			alreadyExistsSchemas.Add(firstPair.Key, firstPair.Value);

			var subSchema = CreateSchema(firstPair.Key, alreadyExistsSchemas, CreateIdFunc);
			subSchemas.Add(new KeyValuePair<Type, DataSchemaElement[]>(firstPair.Key, subSchema));

			toCreateSchemas.Remove(firstPair.Key);
		} while (toCreateSchemas.Count > 0);

		var schemas = new Dictionary<Type, DataSchema>();
		var missingSchemas = new Dictionary<Type, List<Dictionary<uint, DataSchema>>>();
		foreach (var schemaRaw in subSchemas)
		{
			var schemaSubSchemas = new Dictionary<uint, DataSchema>();
			var schema = new DataSchema(schemaRaw.Key, schemaRaw.Value, schemaSubSchemas);

			schemas[schemaRaw.Key] = schema;
			foreach (var element in schemaRaw.Value)
			{
				if (!element.SubSchemaId.HasValue)
					continue;

				var subId = element.SubSchemaId.Value;
				if (!alreadyExistsSchemas.Reverse.TryGetValue(subId, out var subSchemasType))
					// TODO fail. some types without schemas
					throw new Exception();

				if (!schemas.TryGetValue(subSchemasType, out var subSchema))
				{
					if (!missingSchemas.TryGetValue(subSchemasType, out var missingSchema))
					{
						missingSchema = new List<Dictionary<uint, DataSchema>>();
						missingSchemas.Add(subSchemasType, missingSchema);
					}

					missingSchema.Add(schemaSubSchemas);
					continue;
				}

				schemaSubSchemas[subId] = subSchema;
			}
		}

		var rootSubSchemas = schemas.ToDictionary(key => alreadyExistsSchemas.Forward[key.Key], value => value.Value);
		var rootSchema = new DataSchema(type, rootElements, rootSubSchemas);
		if (missingSchemas.ContainsKey(type))
		{
			schemas.Add(type, rootSchema);

			if (!missingSchemas.TryGetValue(type, out var missingSchema))
			{
				missingSchema = new List<Dictionary<uint, DataSchema>>();
				missingSchemas.Add(type, missingSchema);
			}

			missingSchema.Add(rootSubSchemas);
		}

		foreach (var missingSchema in missingSchemas)
		{
			if (!schemas.TryGetValue(missingSchema.Key, out var schema))
				// TODO fail. some types without schemas
				throw new Exception();

			var schemaId = alreadyExistsSchemas.Forward[missingSchema.Key];

			foreach (var seekers in missingSchema.Value)
				seekers[schemaId] = schema;
		}

		return rootSchema;
	}

	private static DataSchemaElement[] CreateSchema(
		Type type,
		Map<Type, uint> alreadyExists,
		Func<Type, uint> cr
	)
	{
		var fields = type.GetFields()
			.Where(t => t.IsPublic && t.FieldType.FilterDataSchemaElement())
			.ToArray();

		var elements = new List<DataSchemaElement>();
		foreach (var field in fields)
		{
			elements.Add(DataSchemaElement.Create(field,
				subSchemaType =>
				{
					if (alreadyExists.Forward.TryGetValue(subSchemaType, out var id))
						return id;

					return cr(subSchemaType);
				}));
		}

		return elements.ToArray();
	}
}