using Macropus.ECS.Component;
using Macropus.ECS.Component.Exceptions;

namespace Macropus.ECS.ComponentsStorage.Impl;

public abstract class ComponentStorage
{
	public abstract bool HasEntity(Guid entity);
	public abstract IComponent? GetComponent(Guid entity);
	public abstract void RemoveComponent(Guid entity);
	public abstract void Clear();
	public abstract IReadOnlyCollection<Guid> GetEntities();
}

public class ComponentStorage<T> : ComponentStorage where T : struct, IComponent
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

	public override bool HasEntity(Guid entity)
	{
		return components.ContainsKey(entity);
	}

	public override IComponent? GetComponent(Guid entity)
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

	public override void RemoveComponent(Guid entity)
	{
		if (isReadOnlyComponent && components.ContainsKey(entity))
			throw new IsReadOnlyComponentException();

		components[entity] = null;
	}

	public override void Clear()
	{
		components.Clear();
	}

	public override IReadOnlyCollection<Guid> GetEntities()
	{
		return components.Keys;
	}
}