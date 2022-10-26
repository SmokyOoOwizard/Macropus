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
}