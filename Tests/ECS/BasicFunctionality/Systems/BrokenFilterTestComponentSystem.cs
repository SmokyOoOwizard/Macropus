using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Macropus.ECS.Systems.Filter;

namespace Tests.ECS.BasicFunctionality.Systems;

public class BrokenFilterTestComponentUpdateSystem : ASystem, IReactiveSystem
{
	public static ComponentsFilter Filter()
	{
		return ComponentsFilter.NoneOf(typeof(string)).Build();
	}

	public void Execute(IEnumerable<IEntity> entities)
	{
		foreach (var entity in entities) { }
	}
}