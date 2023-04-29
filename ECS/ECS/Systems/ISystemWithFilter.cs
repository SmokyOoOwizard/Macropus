using Macropus.ECS.Component.Filter;

namespace Macropus.ECS.Systems;

public interface ISystemWithFilter : ISystem
{
	public ComponentsFilter GetFilter();
}