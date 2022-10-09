using Macropus.ECS.Entity;
using Macropus.ECS.System;
using Tests.ECS.BasicFunctionality.Components;

namespace Tests.ECS.BasicFunctionality.Systems;

public class FilterTestComponentSystem : ASystem, IFilteredSystem
{
	public static ComponentsFilter Filter()
	{
		return ComponentsFilter.NoneOf(typeof(TestComponent));
	}

	public override void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities)
		{
			if (entity.HasComponent<TestComponent>())
				throw new Exception();
		}
	}
}