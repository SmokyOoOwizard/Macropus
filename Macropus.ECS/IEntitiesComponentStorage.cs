namespace Macropus.ECS;

public interface IEntitiesComponentStorage
{
	bool HasComponent<T>(Guid entityId) where T : struct, IComponent;
	T GetComponent<T>(Guid entityId) where T : struct, IComponent;
	void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent;
	void RemoveComponent<T>(Guid entityId) where T : struct, IComponent;
}