using System.Collections;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Schema;

namespace ECS.Serialize.Serialize.State.Impl;

class ComponentSerializeState : ISerializeState
{
	private static readonly QueuePool<object?> NullableObjectQueuePool = QueuePool<object?>.Instance;
	private static readonly ListPool<long?> NullableIdsListPool = ListPool<long?>.Instance;

	private readonly Dictionary<DataSchemaElement, List<long?>?> complexRefs = new();
	private readonly Stack<KeyValuePair<DataSchemaElement, Queue<object?>>> unprocessedComplexFields = new();

	public DataSchema? Schema;
	public DataSchemaElement? ParentRef;
	public object? Value;


	public ComponentSerializeState Init(DataSchema schema, object value)
	{
		Schema = schema;
		Value = value;

		foreach (var element in schema.Elements)
		{
			if (element.Info.Type != ESchemaElementType.ComplexType)
				continue;

			var queue = NullableObjectQueuePool.Take();

			var elementValue = element.FieldInfo.GetValue(value);

			if (element.Info.CollectionType is ECollectionType.Array)
			{
				if (elementValue is IList array)
					foreach (var t in array)
						queue.Enqueue(t);
			}
			else
				queue.Enqueue(element.FieldInfo.GetValue(value));

			if (queue.Count == 0)
			{
				NullableObjectQueuePool.Release(queue);

				complexRefs.Add(element, elementValue == null ? null : new List<long?>());
			}
			else
				unprocessedComplexFields.Push(new(element, queue));
		}


		ParentRef = null;

		return this;
	}

	public ComponentSerializeState Init(DataSchema schema, object newTarget, DataSchemaElement parentRef)
	{
		Init(schema, newTarget);
		ParentRef = parentRef;

		return this;
	}

	public KeyValuePair<DataSchemaElement, Queue<object?>>? TryGetUnprocessed()
	{
		if (unprocessedComplexFields.Count == 0)
			return default;

		while (unprocessedComplexFields.Count > 0)
		{
			var target = unprocessedComplexFields.Peek();

			if (target.Value.Count == 0)
			{
				unprocessedComplexFields.Pop();

				NullableObjectQueuePool.Release(target.Value);
				continue;
			}

			return target;
		}

		return default;
	}

	public void AddProcessed(DataSchemaElement target, long? componentId)
	{
		if (!complexRefs.TryGetValue(target, out var list) || list == null)
		{
			list = NullableIdsListPool.Take();
			complexRefs[target] = list;
		}

		list.Add(componentId);
	}

	public void AddRangeProcessed(DataSchemaElement target, long?[] componentsIds)
	{
		if (!complexRefs.TryGetValue(target, out var list) || list == null)
		{
			list = NullableIdsListPool.Take();
			complexRefs[target] = list;
		}

		list.AddRange(componentsIds);
	}

	public List<long?>? GetProcessed(DataSchemaElement target)
	{
		return complexRefs[target];
	}

	public void Clear()
	{
		Schema = null;
		ParentRef = null;
		Value = null;

		foreach (var (_, value) in unprocessedComplexFields)
		{
			NullableObjectQueuePool.Release(value);
		}

		unprocessedComplexFields.Clear();

		foreach (var (_, value) in complexRefs)
		{
			if (value != null)
				NullableIdsListPool.Release(value);
		}

		complexRefs.Clear();
	}
}