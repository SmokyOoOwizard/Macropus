﻿using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Tests.ECS.BasicFunctionality.Components;

namespace Tests.ECS.BasicFunctionality.Systems;

public class RemoveTestComponentSystem : ASystem
{
	public override void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities)
			entity.RemoveComponent<TestComponent>();
	}
}