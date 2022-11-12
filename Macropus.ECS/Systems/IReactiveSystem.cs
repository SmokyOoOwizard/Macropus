﻿using Macropus.ECS.Entity;
using Macropus.ECS.Systems.Filter;

namespace Macropus.ECS.Systems;

public interface IReactiveSystem
{
	static abstract ComponentsFilter Filter();
	void Execute(IEnumerable<IEntity> entities);
}