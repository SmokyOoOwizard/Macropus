using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Sql;

class ReadResult : IClearable
{
	private static readonly DictionaryPool<DataSchemaElement, object?> ReadObjectsPool = DictionaryPool<DataSchemaElement, object?>.Instance;
	private static readonly DictionaryPool<DataSchemaElement, long?> RefPool = DictionaryPool<DataSchemaElement, long?>.Instance;
	private static readonly DictionaryPool<DataSchemaElement, List<long?>?> RefCollectionPool = DictionaryPool<DataSchemaElement, List<long?>?>.Instance;
	private static readonly ListPool<long?> NullableIdsListPool = ListPool<long?>.Instance;

	public Dictionary<DataSchemaElement, object?> SimpleValues;
	public Dictionary<DataSchemaElement, object?> ReadRefs;

	public Dictionary<DataSchemaElement, long?> ComplexRefs;
	public Dictionary<DataSchemaElement, List<long?>?> ComplexCollectionsRefs;

	public ReadResult Init()
	{
		SimpleValues = ReadObjectsPool.Take();
		ReadRefs = ReadObjectsPool.Take();
		ComplexRefs = RefPool.Take();
		ComplexCollectionsRefs = RefCollectionPool.Take();

		return this;
	}

	public void Clear()
	{
		ReadObjectsPool.Release(SimpleValues);
		ReadObjectsPool.Release(ReadRefs);
		RefPool.Release(ComplexRefs);

		foreach (var (_, refs) in ComplexCollectionsRefs)
		{
			if (refs != null)
				NullableIdsListPool.Release(refs);
		}
		RefCollectionPool.Release(ComplexCollectionsRefs);
	}
}