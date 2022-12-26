using System.Collections;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class SerializeStatePools
{
	public readonly QueuePool<object?> UnprocessedQueuePool = new();
	public readonly ListPool<long?> ProcessedListPool = new();
}

class ComponentSerializeState : ISerializeState
{
	private static readonly SerializeStatePools Pools = new();

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

			var queue = Pools.UnprocessedQueuePool.Take();

			var elementValue = element.FieldInfo.GetValue(value);

			if (element.Info.CollectionType is ECollectionType.Array)
			{
				if (elementValue is IList array)
				{
					for (var i = 0; i < array.Count; i++)
					{
						queue.Enqueue(array[i]);
					}
				}
			}
			else
			{
				queue.Enqueue(element.FieldInfo.GetValue(value));
			}

			if (queue.Count == 0)
			{
				Pools.UnprocessedQueuePool.Release(queue);
				if (elementValue == null)
					complexRefs.Add(element, null);
				else
					complexRefs.Add(element, Pools.ProcessedListPool.Take());
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

				Pools.UnprocessedQueuePool.Release(target.Value);
				continue;
			}

			return target;
		}

		return default;
	}

	public void AddProcessed(DataSchemaElement target, long? componentId)
	{
		if (!complexRefs.TryGetValue(target, out var list))
		{
			list = Pools.ProcessedListPool.Take();
			complexRefs[target] = list;
		}

		list.Add(componentId);
	}
	public void AddRangeProcessed(DataSchemaElement target, long?[] componentsIds)
	{
		if (!complexRefs.TryGetValue(target, out var list))
		{
			list = Pools.ProcessedListPool.Take();
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
		
		foreach (var queue in unprocessedComplexFields)
		{
			Pools.UnprocessedQueuePool.Release(queue.Value);
		}
		
		unprocessedComplexFields.Clear();

		foreach (var list in complexRefs)
		{
			if (list.Value != null)
				Pools.ProcessedListPool.Release(list.Value);
		}
		
		complexRefs.Clear();
	}
}