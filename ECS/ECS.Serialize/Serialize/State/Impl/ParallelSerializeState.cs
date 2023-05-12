using Macropus.Schema;

namespace ECS.Serialize.Serialize.State.Impl;

class ParallelSerializeState : ISerializeState
{
	public DataSchema? Schema;
	public DataSchemaElement? ParentRef;
	public Queue<object?>? Values;

	public ParallelSerializeState Init(DataSchema schema, Queue<object?> values, DataSchemaElement parentRef)
	{
		Schema = schema;
		ParentRef = parentRef;
		Values = values;

		return this;
	}

	public void Clear()
	{
		Schema = null;
		ParentRef = null;
		Values = null;
	}
}