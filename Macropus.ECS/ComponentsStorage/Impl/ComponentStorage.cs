using Macropus.ECS.Component;
using Macropus.ECS.Component.Exceptions;

namespace Macropus.ECS.ComponentsStorage.Impl;

public class ComponentStorage<T> : IComponentStorage<T> where T : struct, IComponent
{
	private readonly Dictionary<Guid, T?> components = new();
	private readonly bool isReadOnlyComponent;

	public string ComponentName { get; }

	public ComponentStorage()
	{
		var type = typeof(T);
		isReadOnlyComponent = type.IsAssignableTo(typeof(IReadOnlyComponent));
		ComponentName = type.FullName!;
	}

	public bool HasEntity(Guid entity)
	{
		return components.ContainsKey(entity);
	}

	public IComponent? GetComponent(Guid entity)
	{
		return components.TryGetValue(entity, out var component) ? component : null;
	}

	public T? GetGenericComponent(Guid entity)
	{
		return components.TryGetValue(entity, out var component) ? component : null;
	}

	public void ReplaceComponent(Guid entity, T component)
	{
		if (isReadOnlyComponent)
			if (components.TryGetValue(entity, out var existsComponent) && (existsComponent != null))
				throw new IsReadOnlyComponentException();

		components[entity] = component;
	}

	public void RemoveComponent(Guid entity)
	{
		if (isReadOnlyComponent && components.ContainsKey(entity))
			throw new IsReadOnlyComponentException();

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