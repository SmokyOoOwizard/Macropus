﻿using Macropus.ECS.Entity;
using Macropus.ECS.Systems;

namespace Tests.ECS.BasicFunctionality.Systems;

public class BrokenFilterTestComponentUpdateSystem : ASystem, IReactiveSystem
{
	public static ComponentsFilter Filter()
	{
		return ComponentsFilter.NoneOf(typeof(string));
	}

	public void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities) { }
	}
}