using Macropus.Schema;

namespace Macropus.ECS.Serialize.Deserialize;

interface ITargetDeserializeState : IDeserializeState
{
	DataSchemaElement Target { get; }
}