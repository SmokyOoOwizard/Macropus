using Macropus.ECS.Component;
using Macropus.ECS.Component.Exceptions;

namespace Macropus.ECS.ComponentsStorage.Impl;

public class ComponentsStorage : IComponentsStorage
{
	private readonly Dictionary<string, IComponentStorage> storage = new();
	private readonly HashSet<Guid> existsEntities = new();

	public uint ComponentsCount => (uint)storage.Count;
	public uint EntitiesCount => (uint)existsEntities.Count;

	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			return false;

		if (!storage.TryGetValue(componentName, out var entities))
			return false;

		var component = (entities as IComponentStorage<T>)!.GetComponent(entityId);

		return component != null;
	}

	public T GetComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (!storage.TryGetValue(componentName, out var entities))
			throw new ComponentNotFoundException();

		var component = (entities as IComponentStorage<T>)!.GetComponent(entityId);

		if (component == null)
			throw new ComponentNotFoundException();

		return (T)component;
	}

	public void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		IComponentStorage<T> entities;
		if (storage.TryGetValue(componentName, out var en))
		{
			entities = (en as IComponentStorage<T>)!;
		}
		else
		{
			entities = new ComponentStorage<T>();
			storage.Add(componentName, entities);
		}

		entities.ReplaceComponent(entityId, component);

		existsEntities.Add(entityId);
	}

	public void RemoveComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (!storage.TryGetValue(componentName, out var entities))
		{
			entities = new ComponentStorage<T>();
			storage.Add(componentName, entities);
		}

		entities.RemoveComponent(entityId);

		existsEntities.Add(entityId);
	}

	public IEnumerable<Guid> GetEntities()
	{
		return existsEntities;
	}

	public void Clear()
	{
		foreach (var componentType in storage)
			componentType.Value.Clear();

		existsEntities.Clear();
	}
}