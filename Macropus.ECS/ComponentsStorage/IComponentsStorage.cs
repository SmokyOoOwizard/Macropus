using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage;

public interface IComponentsStorage
{
	bool Empty { get; }

	bool HasComponent<T>(Guid entityId) where T : struct, IComponent;
	T GetComponent<T>(Guid entityId) where T : struct, IComponent;
	void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent;
	void RemoveComponent<T>(Guid entityId) where T : struct, IComponent;

	IEnumerationComponents GetEnumerationComponents(bool copy);
}