using System.Data;
using Macropus.CoolStuff;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Deserialize.State;

public interface IDeserializeState : IState
{
	Task Read(IDbConnection dbConnection);

	bool HasRefs();
	IDeserializeState PopSomeRefs();
	void AddRef(DataSchemaElement target, object? obj);
	
	object? Create();
}