using Macropus.ECS.Exceptions;

namespace Macropus.ECS;

public sealed class SimpleEntitiesComponentStorage : IEntitiesComponentStorage
{
	private readonly Dictionary<string, Dictionary<Guid, IComponent?>> components = new();
	private readonly Dictionary<string, Dictionary<Guid, IComponent?>> changesComponents = new();

	public bool HasComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			return false;

		if (!components.TryGetValue(componentName, out var entities))
			if (!changesComponents.TryGetValue(componentName, out entities))
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

		if (!components.TryGetValue(componentName, out var entities))
			if (!changesComponents.TryGetValue(componentName, out entities))
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

		if (!changesComponents.TryGetValue(componentName, out var entities))
		{
			entities = new();
			changesComponents.Add(componentName, entities);
		}

		entities[entityId] = component;
	}

	public void RemoveComponent<T>(Guid entityId) where T : struct, IComponent
	{
		var componentName = typeof(T).FullName;
		if (string.IsNullOrWhiteSpace(componentName))
			throw new TypeNameNotSupportedException();

		if (!changesComponents.TryGetValue(componentName, out var entities))
		{
			entities = new Dictionary<Guid, IComponent?>();
			changesComponents.Add(componentName, entities);
		}

		entities[entityId] = null;
	}
}