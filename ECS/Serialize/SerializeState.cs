using System.Collections;
using Macropus.CoolStuff;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Schema;

namespace Macropus.ECS.Serialize;

class SerializeStatePools
{
	public readonly StackPool<KeyValuePair<DataSchemaElement, Queue<object?>>> UnprocessedPool = new();
	public readonly QueuePool<object?> UnprocessedQueuePool = new();
	public readonly DictionaryPool<DataSchemaElement, List<long?>?> ProcessedPool = new();
	public readonly ListPool<long?> ProcessedListPool = new();
}

struct SerializeState : IClearable
{
	private static readonly SerializeStatePools Pools = new();

	public readonly DataSchema Schema;
	public readonly DataSchemaElement? ParentRef;
	public readonly object? Value;

	private readonly Dictionary<DataSchemaElement, List<long?>?> processed;
	private readonly Stack<KeyValuePair<DataSchemaElement, Queue<object?>>> unprocessed;

	public SerializeState(DataSchema schema, object value)
	{
		Schema = schema;
		Value = value;

		unprocessed = Pools.UnprocessedPool.Take();
		processed = Pools.ProcessedPool.Take();

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
				{
					processed.Add(element, null);
				}
				else
				{
					processed.Add(element, Pools.ProcessedListPool.Take());
				}
			}
			else
				unprocessed.Push(new(element, queue));
		}


		ParentRef = null;
	}

	public SerializeState(DataSchema schema, object newTarget, DataSchemaElement parentRef) : this(schema, newTarget)
	{
		ParentRef = parentRef;
	}

	public KeyValuePair<DataSchemaElement, Queue<object?>>? TryGetUnprocessed()
	{
		if (unprocessed.Count == 0)
			return default;

		while (unprocessed.Count > 0)
		{
			var target = unprocessed.Peek();

			if (target.Value.Count == 0)
			{
				unprocessed.Pop();

				Pools.UnprocessedQueuePool.Release(target.Value);
				continue;
			}

			return target;
		}

		return default;
	}

	public void AddProcessed(DataSchemaElement target, long? componentId)
	{
		if (!processed.TryGetValue(target, out var list))
		{
			list = Pools.ProcessedListPool.Take();
			processed[target] = list;
		}

		list.Add(componentId);
	}

	public List<long?>? GetProcessed(DataSchemaElement target)
	{
		return processed[target];
	}

	public void Clear()
	{
		foreach (var queue in unprocessed)
		{
			Pools.UnprocessedQueuePool.Release(queue.Value);
		}

		Pools.UnprocessedPool.Release(unprocessed);

		foreach (var list in processed)
		{
			if (list.Value != null)
				Pools.ProcessedListPool.Release(list.Value);
		}

		Pools.ProcessedPool.Release(processed);
	}
}