using Macropus.Schema;

namespace Macropus.ECS.Serialize.Deserialize.State;

interface ITargetDeserializeState : IDeserializeState
{
	DataSchemaElement Target { get; }
}