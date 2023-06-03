using System.Collections;
using ECS.Schema;
using Macropus.CoolStuff.Collections.Pool;

namespace ECS.Serialize.Serialize.State.Impl;

class ComponentSerializeState : ISerializeState
{
	private static readonly QueuePool<object?> NullableObjectQueuePool = QueuePool<object?>.Instance;

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
	}
}