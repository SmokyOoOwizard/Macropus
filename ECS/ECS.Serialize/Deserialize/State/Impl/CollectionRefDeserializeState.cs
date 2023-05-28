using System.Data;
using ECS.Schema;
using Macropus.CoolStuff.Collections.Pool;
using Macropus.Linq;

// ReSharper disable ParameterHidesMember

namespace ECS.Serialize.Deserialize.State.Impl;

class CollectionRefDeserializeState : ITargetDeserializeState
{
	private static readonly StatePool StatePool = StatePool.Instance;
	private static readonly StackPool<long?> NullableIdsStackPool = StackPool<long?>.Instance;
	private static readonly ListPool<object?> NullableObjectsListPool = ListPool<object?>.Instance;

	private DataSchema schema = null!;
	private List<object?> readComponents = null!;
	private Stack<long?> components = null!;

	public DataSchemaElement Target { get; private set; }

	public CollectionRefDeserializeState Init(
		DataSchema schema,
		List<long?> components,
		DataSchemaElement target
	)
	{
		this.schema = schema;

		readComponents = NullableObjectsListPool.Take();

		this.components = NullableIdsStackPool.Take();
		components.Fill(this.components);
		
		Target = target;

		return this;
	}

	public Task Read(IDbConnection dbConnection)
	{
		return Task.CompletedTask;
	}

	public bool HasRefs()
	{
		return components.Count > 0;
	}

	public IDeserializeState PopSomeRefs()
	{
		long componentId = 0;
		do
		{
			if (components.Count == 0)
				throw new Exception();

			var id = components.Pop();
			if (id == null)
			{
				readComponents.Add(null);
				continue;
			}

			componentId = id.Value;
			break;
		} while (components.Count > 0);

		return StatePool.RefDeserializeStatePool.Take().Init(schema, componentId, Target);
	}

	public void AddRef(DataSchemaElement target, object? obj)
	{
		readComponents.Add(obj);

		while (components.Count > 0)
		{
			var id = components.Peek();
			if (id != null)
				break;

			components.Pop();
			readComponents.Add(null);
		}
	}

	public object Create()
	{
		var fieldType = Target.FieldInfo.FieldType.GetElementType();
		if (fieldType == null)
			// TODO
			throw new Exception();

		var array = Array.CreateInstance(fieldType, readComponents.Count);

		for (var j = 0; j < array.Length; j++)
			array.SetValue(readComponents[j], j);

		return array;
	}

	public void Clear()
	{
		NullableIdsStackPool.Release(components);
	}
}