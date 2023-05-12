using Macropus.ECS.Component.Filter;
using Macropus.ECS.Component.Trigger;

namespace Macropus.ECS.Systems.Extensions;

public static class SystemExtensions
{
	public static ComponentsTrigger? GetTrigger<T>(this T system) where T : ISystem
	{
		if (system is IReactiveSystem reactiveSystem)
			return reactiveSystem.GetTrigger();

		return null;
	}

	public static ComponentsFilter? GetFilter<T>(this T system) where T : ISystem
	{
		if (system is ISystemWithFilter systemWithFilter)
			return systemWithFilter.GetFilter();

		return null;
	}
}