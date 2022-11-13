using Macropus.ECS.Component.Filter;
using Macropus.ECS.Entity;

namespace Macropus.ECS.Systems;

public interface IReactiveSystem
{
	static abstract ComponentsFilter GetTrigger();
	void Execute(IEnumerable<IEntity> entities);
}