using System.Data;
using Macropus.CoolStuff;
using Macropus.Schema;

namespace Macropus.ECS.Serialize.Deserialize;

public interface IDeserializeState : IClearable
{
	Task Read(IDbConnection dbConnection);

	bool HasRefs();
	IDeserializeState PopSomeRefs();
	void AddRef(DataSchemaElement target, object? obj);

	object? Create();
}