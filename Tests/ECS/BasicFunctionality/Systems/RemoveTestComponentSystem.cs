using Macropus.ECS.Component.Filter;
using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Tests.ECS.BasicFunctionality.Components;

namespace Tests.ECS.BasicFunctionality.Systems;

public class RemoveTestComponentSystem : ASystem, IReactiveSystem
{
	public static ComponentsFilter GetTrigger()
		=> ComponentsFilter.AllOf(typeof(TestComponent)).Build();

	public void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities)
			entity.RemoveComponent<TestComponent>();
	}
}