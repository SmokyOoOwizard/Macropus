using Macropus.ECS.Entity;
using Macropus.ECS.System;

namespace Tests.ECS.BasicFunctionality.Systems;

public class BrokenFilterTestComponentSystem : ASystem, IFilteredSystem
{
	public static ComponentsFilter Filter()
	{
		return ComponentsFilter.NoneOf(typeof(string));
	}

	public override void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities) { }
	}
}