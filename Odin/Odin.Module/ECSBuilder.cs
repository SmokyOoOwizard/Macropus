using Macropus.ECS.Systems;
using Odin.SDK;

namespace Odin.Module;

public class ECSBuilder : IECSBuilder
{
	private readonly Dictionary<string, List<KeyValuePair<Type, int>>> systemsByContexts = new();

	public void RegisterSystem<T>(string contextName) where T : ISystem
	{
		if (!systemsByContexts.TryGetValue(contextName, out var systems))
		{
			systems = new();
			systemsByContexts[contextName] = systems;
		}

		systems.Add(new(typeof(T), 0));
	}

	public void RegisterSystem<T>(string contextName, int order) where T : ISystem
	{
		if (!systemsByContexts.TryGetValue(contextName, out var systems))
		{
			systems = new();
			systemsByContexts[contextName] = systems;
		}

		systems.Add(new(typeof(T), order));
	}
}