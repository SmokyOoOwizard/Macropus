using ECS.Schema;
using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections.Pool;

namespace ECS.Serialize.Sql;

class ReadResult : IClearable
{
	private static readonly DictionaryPool<DataSchemaElement, object?> ReadObjectsPool = DictionaryPool<DataSchemaElement, object?>.Instance;

	public Dictionary<DataSchemaElement, object?> SimpleValues;

	public ReadResult Init()
	{
		SimpleValues = ReadObjectsPool.Take();

		return this;
	}

	public void Clear()
	{
		ReadObjectsPool.Release(SimpleValues);
	}
}