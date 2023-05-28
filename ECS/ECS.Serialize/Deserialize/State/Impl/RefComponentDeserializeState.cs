
using ECS.Schema;

namespace ECS.Serialize.Deserialize.State.Impl;

class RefComponentDeserializeState : ComponentDeserializeState, ITargetDeserializeState
{
	public DataSchemaElement Target { get; private set; }

	public RefComponentDeserializeState Init(DataSchema schema, long componentId, DataSchemaElement target)
	{
		base.Init(schema, componentId);

		Target = target;
		return this;
	}
}