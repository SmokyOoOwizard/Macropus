using Macropus.CoolStuff.Collections.Pool;
using Macropus.Linq;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Deserialize;

class DeserializeStatePools
{
	public readonly ListPool<KeyValuePair<DataSchemaElement, object?>> ReadValuesPool = new();
	public readonly ListPool<DataSchemaElement> UnreadValues = new();

	public readonly StackPool<KeyValuePair<DataSchemaElement, long?>> RefsPool = new();
	public readonly DictionaryPool<DataSchemaElement, List<object?>> ReadRefsPool = new();
	public readonly ListPool<object?> ReadRefsListPool = new();
}

class ComponentDeserializeState : IDeserializeState
{
	private static readonly DeserializeStatePools Pools = new();

	public DataSchema Schema;
	public DataSchemaElement? Element;
	public long ComponentId;

	public Dictionary<DataSchemaElement, List<object?>> ReadRefs;
	public List<KeyValuePair<DataSchemaElement, object?>> ReadValues;
	private List<DataSchemaElement> unreadValues;
	private Stack<KeyValuePair<DataSchemaElement, long?>> refs;
	
	public ComponentDeserializeState Init(DataSchema schema, long componentId)
	{
		Schema = schema;
		ComponentId = componentId;

		ReadValues = Pools.ReadValuesPool.Take();

		unreadValues = Pools.UnreadValues.Take();
		schema.Elements.Fill(unreadValues);

		refs = Pools.RefsPool.Take();
		ReadRefs = Pools.ReadRefsPool.Take();

		Element = null;

		return this;
	}

	public ComponentDeserializeState Init(DataSchema schema, DataSchemaElement element, long componentId)
	{
		Init(schema, componentId);
		Element = element;

		return this;
	}


	public void AddRef(DataSchemaElement target, long? id)
	{
		refs.Push(new(target, id));
	}

	public KeyValuePair<DataSchemaElement, long?>? TryGetRef()
	{
		if (refs.TryPop(out var result))
			return result;

		return default;
	}

	public void AddReadRef(DataSchemaElement target, object? value)
	{
		if (!ReadRefs.TryGetValue(target, out var list))
		{
			list = Pools.ReadRefsListPool.Take();
			ReadRefs[target] = list;
		}

		list.Add(value);
	}

	public IReadOnlyList<DataSchemaElement> GetUnreadFields()
	{
		return unreadValues;
	}

	public void MarkUnreadFieldsAsRead()
	{
		unreadValues.Clear();
	}

	public void Clear()
	{
		Schema = null;
		Element = null;
		ComponentId = 0;
		
		Pools.ReadValuesPool.Release(ReadValues);
		Pools.UnreadValues.Release(unreadValues);
		Pools.RefsPool.Release(refs);

		foreach (var readRef in ReadRefs)
		{
			Pools.ReadRefsListPool.Release(readRef.Value);
		}

		Pools.ReadRefsPool.Release(ReadRefs);
	}
}