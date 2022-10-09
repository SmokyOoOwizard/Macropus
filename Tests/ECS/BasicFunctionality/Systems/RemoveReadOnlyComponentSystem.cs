using Macropus.ECS.Entity;
using Macropus.ECS.System;
using Tests.ECS.BasicFunctionality.Components;

namespace Tests.ECS.BasicFunctionality.Systems;

public class RemoveReadOnlyComponentSystem : ASystem
{
	public override void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities) entity.RemoveComponent<ReadOnlyComponent>();
	}
}