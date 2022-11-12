using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Macropus.ECS.Systems.Extensions;
using Tests.ECS.BasicFunctionality.Components;

namespace Tests.ECS.BasicFunctionality.Systems;

public class FilterTestComponentUpdateSystem : ASystem, IReactiveSystem
{
	public static ComponentsFilter Filter()
	{
		return ComponentsFilter.AnyOf(
				ComponentsFilter.AllOf(typeof(TestComponent2), typeof(TestComponent4), typeof(TestComponent7)),
				ComponentsFilter.AllOf(typeof(TestComponent10)).NoneOf(typeof(TestComponent6)))
			.Build();
	}

	public void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities)
		{
			if (entity.HasComponent<TestComponent2>()
			    && entity.HasComponent<TestComponent4>()
			    && entity.HasComponent<TestComponent7>())
				return;

			if (entity.HasComponent<TestComponent10>() && !entity.HasComponent<TestComponent6>())
				return;

			throw new Exception();
		}
	}
}