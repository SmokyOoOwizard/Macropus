using Macropus.ECS.Entity;

namespace Macropus.ECS.System;

public abstract class ASystem
{
	public abstract void Execute(IEnumerable<IEntity> entities);
}