using Macropus.Linq;

namespace Macropus.Schema;

public class DataSchema
{
	public readonly Guid Id;
	public readonly string Name;

	public readonly IReadOnlyCollection<DataSchemaElement> Elements;
	public readonly IReadOnlyCollection<Guid> SubSchemas;

	public DataSchema(Guid id, string name, IReadOnlyCollection<DataSchemaElement> elements)
	{
		Id = id;
		Name = name;
		Elements = elements;
		SubSchemas = elements.Select(e => e.SubSchemaId).NotNull().ToArray();
	}
}