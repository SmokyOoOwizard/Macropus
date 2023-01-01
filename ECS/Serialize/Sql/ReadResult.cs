using Macropus.Schema;

namespace Macropus.ECS.Serialize.Sql;

struct ReadResult
{
	public Dictionary<DataSchemaElement, object?> SimpleValues;
	public Dictionary<DataSchemaElement, object?> ReadRefs;
	
	public Dictionary<DataSchemaElement, long?> ComplexRefs;
	public Dictionary<DataSchemaElement, List<long?>?> ComplexCollectionsRefs;

	public static ReadResult Init()
	{
		return new()
		{
			SimpleValues = new(),
			ReadRefs =  new(),
			ComplexRefs = new(),
			ComplexCollectionsRefs = new()
		};
	}
}