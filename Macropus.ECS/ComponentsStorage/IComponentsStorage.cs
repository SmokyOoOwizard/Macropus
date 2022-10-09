using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage;

public interface IComponentsStorage : IReadOnlyComponentsStorage
{
	void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent;
	void RemoveComponent<T>(Guid entityId) where T : struct, IComponent;

	void Clear();
}