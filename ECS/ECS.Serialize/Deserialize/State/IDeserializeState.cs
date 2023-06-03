using System.Data;

namespace ECS.Serialize.Deserialize.State;

public interface IDeserializeState : IState
{
	Task Read(IDbConnection dbConnection);

	object? Create();
}