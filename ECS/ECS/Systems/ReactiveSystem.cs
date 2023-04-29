using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Trigger;
using Macropus.ECS.Entity;

namespace Macropus.ECS.Systems;

public interface IReactiveSystem : ISystem
{
	public ComponentsTrigger GetTrigger();
	public void Execute(IEnumerable<IEntity> entities);
}

public interface ISystemWithFilter : ISystem
{
	public ComponentsFilter GetFilter();
}