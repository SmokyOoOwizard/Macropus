namespace Macropus.ECS.Component.Storage;

public interface IComponentsStorage : IReadOnlyComponentsStorage
{
	void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent;
	void RemoveComponent<T>(Guid entityId) where T : struct, IComponent;
	void Apply(IReadOnlyComponentsChangesStorage changes);
	void Clear();
}