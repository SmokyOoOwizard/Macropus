using Macropus.ECS.Entity;

namespace Macropus.ECS.Systems;

public abstract class ASystem
{
	public abstract void Execute(IEnumerable<IEntity> entities);
}