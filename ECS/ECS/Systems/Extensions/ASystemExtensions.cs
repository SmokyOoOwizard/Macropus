using Macropus.ECS.Component.Trigger;

namespace Macropus.ECS.Systems.Extensions;

public static class ASystemExtensions
{
	public static ComponentsTrigger? GetTrigger<T>(this T system) where T : ASystem
	{
		if (system is AReactiveSystem reactiveSystem)
			return reactiveSystem.GetTrigger();

		return null;
	}
}