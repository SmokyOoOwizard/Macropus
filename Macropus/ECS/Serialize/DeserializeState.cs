using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Linq;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class DeserializeStatePools
{
	public readonly ListPool<KeyValuePair<DataSchemaElement, object?>> ReadValuesPool = new();
	public readonly ListPool<DataSchemaElement> UnreadValues = new();

	public readonly StackPool<KeyValuePair<DataSchemaElement, long?>> RefsPool = new();
	public readonly DictionaryPool<DataSchemaElement, List<object?>> ReadRefsPool = new();
	public readonly ListPool<object?> ReadRefsListPool = new();
}

struct DeserializeState : IClearable
{
	private static readonly DeserializeStatePools Pools = new();

	public readonly DataSchema Schema;
	public readonly DataSchemaElement? Element;
	public readonly long ComponentId;

	public readonly Dictionary<DataSchemaElement, List<object?>> ReadRefs;
	public readonly List<KeyValuePair<DataSchemaElement, object?>> ReadValues;
	private readonly List<DataSchemaElement> unreadValues;
	private readonly Stack<KeyValuePair<DataSchemaElement, long?>> refs;

	public DeserializeState(DataSchema schema, long componentId)
	{
		Schema = schema;
		ComponentId = componentId;

		ReadValues = Pools.ReadValuesPool.Take();

		unreadValues = Pools.UnreadValues.Take();
		schema.Elements.Fill(unreadValues);

		refs = Pools.RefsPool.Take();
		ReadRefs = Pools.ReadRefsPool.Take();

		Element = null;
	}

	public DeserializeState(DataSchema schema, DataSchemaElement element, long componentId) : this(schema, componentId)
	{
		Element = element;
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