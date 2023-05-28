namespace ECS.Schema;

public sealed class DataSchema
{
	public readonly Type SchemaOf;

	public readonly IReadOnlyList<DataSchemaElement> Elements;
	public readonly IReadOnlyDictionary<uint, DataSchema> SubSchemas;

	public DataSchema(
		Type type,
		IReadOnlyList<DataSchemaElement> elements,
		IReadOnlyDictionary<uint, DataSchema> subSchemas
	)
	{
		SchemaOf = type;
		Elements = elements;
		SubSchemas = subSchemas;
	}

	public bool IsFlat() => SubSchemas.Count == 0;
}