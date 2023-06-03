using System.Data;
using ECS.Schema;

namespace ECS.Serialize.Deserialize.State;

public interface IDeserializeState : IState
{
	Task Read(IDbConnection dbConnection);

	object? Create();
}