using Macropus.ECS.Component.Filter;
using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Tests.ECS.BasicFunctionality.Components;

namespace Tests.ECS.BasicFunctionality.Systems;

public class RemoveReadOnlyComponentSystem :ASystem, IReactiveSystem
{
	public static ComponentsFilter GetTrigger() 
		=> ComponentsFilter.AllOf(typeof(ReadOnlyComponent)).Build();

	public void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities)
			entity.RemoveComponent<ReadOnlyComponent>();
	}
}