using Macropus.ECS.Entity.Context;
using Macropus.ECS.Systems;

namespace Macropus.ECS;

public interface IECSContext
{
	IEntityContext EntityContext { get; }

	void AddSystem(ISystem system);
	void RemoveSystem(ISystem system);
}