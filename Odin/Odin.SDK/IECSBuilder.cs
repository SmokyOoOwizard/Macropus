using Macropus.ECS.Systems;

namespace Odin.SDK;

public interface IECSBuilder
{
	void RegisterSystem<T>(string contextName) where T : ISystem;
	void RegisterSystem<T>(string contextName, int order) where T : ISystem;
}