using Macropus.ECS.Component;

namespace Macropus.ECS.ComponentsStorage.Impl;

public class ComponentStorage<T> : IComponentStorage<T> where T : struct, IComponent
{
	private readonly Dictionary<Guid, T?> components = new();

	public string ComponentName { get; }

	public ComponentStorage()
	{
		ComponentName = typeof(T).FullName!;
	}

	public bool HasEntity(Guid entity)
	{
		return components.ContainsKey(entity);
	}

	public T? GetComponent(Guid entity)
	{
		return components.TryGetValue(entity, out var component) ? component : null;
	}

	public void ReplaceComponent(Guid entity, T component)
	{
		components[entity] = component;
	}

	public void RemoveComponent(Guid entity)
	{
		components[entity] = null;
	}

	public void Clear()
	{
		components.Clear();
	}

	public IReadOnlyCollection<Guid> GetEntities()
	{
		return components.Keys;
	}
}