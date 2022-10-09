using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage;

public interface IReadOnlyComponentsStorage
{
	uint ComponentsCount { get; }
	uint EntitiesCount { get; }

	bool HasComponent<T>(Guid entityId) where T : struct, IComponent;
	T GetComponent<T>(Guid entityId) where T : struct, IComponent;

	IEnumerable<Guid> GetEntities();
}