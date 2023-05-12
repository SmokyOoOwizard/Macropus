using Macropus.Schema;

namespace ECS.Serialize.Deserialize.State;

interface ITargetDeserializeState : IDeserializeState
{
	DataSchemaElement Target { get; }
}