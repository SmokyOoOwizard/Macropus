using Macropus.ECS.Component;
using Macropus.ECS.Component.Exceptions;

namespace Macropus.ECS.ComponentsStorage.Impl;

public class ComponentsStorage : IComponentsStorage
{
	private readonly Dictionary<uint, IComponentStorage> storage = new();
	private readonly HashSet<Guid> existsEntities = new();

	private readonly IComponentTypesStorage typesStorage;

	public uint ComponentsCount => (uint)storage.Count;
	public uint EntitiesCount => (uint)existsEntities.Count;

	public ComponentsStorage()
	{
		typesStorage = new ComponentTypesStorage();
	}

	public ComponentsStorage(IComponentTypesStorage typesStorage)
	{
		this.typesStorage = typesStorage;
	}

	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		if (!storage.TryGetValue(typesStorage.GetComponentTypeId<T>(), out var entities))
			return false;

		var component = (entities as IComponentStorage<T>)!.GetGenericComponent(entityId);

		return component != null;
	}

	public bool HasComponent(Guid entityId, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return false;

		if (!storage.TryGetValue(typesStorage.GetComponentTypeId(name), out var entities))
			return false;

		var component = entities.GetComponent(entityId);

		return component != null;
	}

	public T GetComponent<T>(Guid entityId) where T : struct, IComponent
	{
		if (!storage.TryGetValue(typesStorage.GetComponentTypeId<T>(), out var entities))
			throw new ComponentNotFoundException();

		var component = (entities as IComponentStorage<T>)!.GetGenericComponent(entityId);

		if (component == null)
			throw new ComponentNotFoundException();

		return (T)component;
	}

	public IComponent GetComponent(Guid entityId, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentNullException(nameof(name));

		if (!storage.TryGetValue(typesStorage.GetComponentTypeId(name), out var entities))
			throw new ComponentNotFoundException();

		var component = entities.GetComponent(entityId);

		if (component == null)
			throw new ComponentNotFoundException();

		return component;
	}


	public void ReplaceComponent<T>(Guid entityId, T component) where T : struct, IComponent
	{
		var typeId = typesStorage.GetComponentTypeId<T>();

		IComponentStorage<T> entities;
		if (storage.TryGetValue(typeId, out var en))
		{
			entities = (en as IComponentStorage<T>)!;
		}
		else
		{
			entities = new ComponentStorage<T>();
			storage.Add(typeId, entities);
		}

		entities.ReplaceComponent(entityId, component);

		existsEntities.Add(entityId);
	}

	public void RemoveComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var typeId = typesStorage.GetComponentTypeId<T>();

		if (!storage.TryGetValue(typeId, out var entities))
		{
			entities = new ComponentStorage<T>();
			storage.Add(typeId, entities);
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