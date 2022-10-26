using Macropus.CoolStuff;
using Macropus.Schema.Extensions;

namespace Macropus.Schema;

public sealed class DataSchemaBuilder
{
	private readonly Map<Type, uint> alreadyExistsSchemas = new();
	private readonly Dictionary<Type, uint> toCreateSchemas = new();
	private readonly List<KeyValuePair<Type, DataSchemaElement[]>> subSchemas = new();
	private readonly Dictionary<Type, DataSchema> schemas = new();
	private readonly Dictionary<Type, List<Dictionary<uint, DataSchema>>> missingSchemas = new();

	private uint lastId;

	public DataSchema CreateSchema<T>()
	{
		return CreateSchema(typeof(T));
	}

	public DataSchema CreateSchema(Type type)
	{
		try
		{
			alreadyExistsSchemas.Add(type, lastId++);

			var rootElements = CreateSchemaElements(type);
			CreateSubSchemasElements();

			CreateSubSchemas();

			var rootSubSchemas =
				schemas.ToDictionary(key => alreadyExistsSchemas.Forward[key.Key], value => value.Value);
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

			FillMissingSchemas();

			return rootSchema;
		}
		finally
		{
			Clear();
		}
	}

	private void Clear()
	{
		lastId = 0;

		alreadyExistsSchemas.Clear();
		toCreateSchemas.Clear();
		subSchemas.Clear();
		schemas.Clear();
		missingSchemas.Clear();
	}

	private DataSchemaElement[] CreateSchemaElements(Type type)
	{
		var fields = type.GetFields()
			.Where(t => t.IsPublic && t.FieldType.FilterDataSchemaElement())
			.ToArray();

		var elements = new List<DataSchemaElement>();
		foreach (var field in fields)
			elements.Add(DataSchemaElement.Create(field, SchemaIdFactory));

		return elements.ToArray();
	}

	private void CreateSubSchemasElements()
	{
		do
		{
			var firstPair = toCreateSchemas.FirstOrDefault();

			alreadyExistsSchemas.Add(firstPair.Key, firstPair.Value);

			var subSchema = CreateSchemaElements(firstPair.Key);
			subSchemas.Add(new KeyValuePair<Type, DataSchemaElement[]>(firstPair.Key, subSchema));

			toCreateSchemas.Remove(firstPair.Key);
		} while (toCreateSchemas.Count > 0);
	}

	private void CreateSubSchemas()
	{
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
				var subSchemasType = alreadyExistsSchemas.Reverse[subId];

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
	}

	private void FillMissingSchemas()
	{
		foreach (var missingSchema in missingSchemas)
		{
			var schema = schemas[missingSchema.Key];
			var schemaId = alreadyExistsSchemas.Forward[missingSchema.Key];

			foreach (var seekers in missingSchema.Value)
				seekers[schemaId] = schema;
		}
	}

	private uint SchemaIdFactory(Type type)
	{
		if (alreadyExistsSchemas.Forward.TryGetValue(type, out var id))
			return id;

		if (toCreateSchemas.TryGetValue(type, out id))
			return id;

		lastId++;
		toCreateSchemas.Add(type, lastId);
		return lastId;
	}
}