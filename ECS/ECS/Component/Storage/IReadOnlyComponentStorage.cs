namespace Macropus.ECS.Component.Storage;

public interface IReadOnlyComponentStorage : IEnumerable<KeyValuePair<Guid, IComponent?>>
{
	string ComponentName { get; }
	Type ComponentType { get; }
	bool HasEntity(Guid entity);
	IComponent? GetComponent(Guid entity);
	IReadOnlyCollection<Guid> GetEntities();
	IReadOnlyComponentStorage DeepClone();
}