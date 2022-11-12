using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Macropus.ECS.Systems.Filter;
using Tests.ECS.BasicFunctionality.Components;

namespace Tests.ECS.BasicFunctionality.Systems;

public class RemoveTestComponentUpdateSystem : ASystem, IReactiveSystem
{
	public static ComponentsFilter Filter()
		=> ComponentsFilter.AllOf(typeof(TestComponent)).Build();

	public void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities)
			entity.RemoveComponent<TestComponent>();
	}
}