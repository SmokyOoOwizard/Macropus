using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Tests.ECS.BasicFunctionality.Components;

namespace Tests.ECS.BasicFunctionality.Systems;

public class RemoveReadOnlyComponentUpdateSystem :ASystem, IReactiveSystem
{
	public static ComponentsFilter Filter() 
		=> ComponentsFilter.AllOf(typeof(ReadOnlyComponent));

	public void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities)
			entity.RemoveComponent<ReadOnlyComponent>();
	}
}