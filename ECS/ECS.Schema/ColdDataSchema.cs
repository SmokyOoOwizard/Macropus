namespace ECS.Schema;

public sealed class ColdDataSchema
{
	public readonly IReadOnlyCollection<ColdDataSchemaElement> Elements;
	public readonly IReadOnlyDictionary<uint, IReadOnlyCollection<ColdDataSchemaElement>> SubSchemas;

	public ColdDataSchema(
		IReadOnlyCollection<ColdDataSchemaElement> elements,
		IReadOnlyDictionary<uint, IReadOnlyCollection<ColdDataSchemaElement>> subSchemas
	)
	{
		Elements = elements;
		SubSchemas = subSchemas;
	}
}