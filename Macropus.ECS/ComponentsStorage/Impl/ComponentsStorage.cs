using Macropus.ECS.Component;
using Macropus.ECS.Component.Exceptions;

namespace Macropus.ECS.ComponentsStorage.Impl;

public class ComponentsStorage : IClearableComponentsStorage
{
	private readonly Dictionary<string, Dictionary<Guid, IComponent?>> storage = new();
	private readonly HashSet<Guid> existsEntities = new();

	public bool Empty => existsEntities.Count == 0;


	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			return false;

		if (!storage.TryGetValue(componentName, out var entities))
			return false;

		if (!entities.TryGetValue(entityId, out var component))
			return false;

		return component != null;
	}

	public T GetComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (!storage.TryGetValue(componentName, out var entities))
			throw new ComponentNotFoundException();

		if (!entities.TryGetValue(entityId, out var component))
			throw new ComponentNotFoundException();

		if (component == null)
			throw new ComponentNotFoundException();

		return (T)component;
	}

	public void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (!storage.TryGetValue(componentName, out var entities))
		{
			entities = new Dictionary<Guid, IComponent?>();
			storage.Add(componentName, entities);
		}

		entities[entityId] = component;

		existsEntities.Add(entityId);
	}

	public void RemoveComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (!storage.TryGetValue(componentName, out var entities))
		{
			entities = new Dictionary<Guid, IComponent?>();
			storage.Add(componentName, entities);
		}

		entities[entityId] = null;
		existsEntities.Add(entityId);
	}

	public IEnumerationComponents GetEnumerationComponents(bool copy)
	{
		if (copy)
			return new EnumerationComponentsCopyComponentsStorage(storage);
		return new EnumerationComponentsRefComponentsStorage(storage);
	}

	public void Clear()
	{
		foreach (var componentType in storage)
			componentType.Value.Clear();

		existsEntities.Clear();
	}
}