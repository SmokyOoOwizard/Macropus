using Macropus.ECS.Entity;

namespace Macropus.ECS.Systems;

public interface IReactiveSystem
{
	static abstract ComponentsFilter Filter();
	void Execute(IEnumerable<IEntity> entities);
}