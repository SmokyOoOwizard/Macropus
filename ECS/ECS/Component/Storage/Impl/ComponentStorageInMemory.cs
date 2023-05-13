using System.Collections;
using Macropus.ECS.Component.Exceptions;

namespace Macropus.ECS.Component.Storage.Impl;

public abstract class ComponentStorageInMemory : IReadOnlyComponentStorage
{
	public abstract string ComponentName { get; protected set; }
	public abstract Type ComponentType { get; }

	public abstract bool HasEntity(Guid entity);
	public abstract IComponent? GetComponent(Guid entity);
	public abstract void RemoveComponent(Guid entity);
	public abstract void Clear();
	public abstract IReadOnlyCollection<Guid> GetEntities();

	public abstract void Apply(IReadOnlyComponentStorage changes);
	public abstract IReadOnlyComponentStorage DeepClone();

	public abstract IEnumerator<KeyValuePair<Guid, IComponent?>> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

public class ComponentStorageInMemory<T> : ComponentStorageInMemory where T : struct, IComponent
{
	private readonly Dictionary<Guid, T> components;
	private readonly bool isReadOnlyComponent;

	public sealed override string ComponentName { get; protected set; }
	public sealed override Type ComponentType => typeof(T);

	public ComponentStorageInMemory()
	{
		components = new();
		var type = typeof(T);
		isReadOnlyComponent = type.IsAssignableTo(typeof(IReadOnlyComponent));
		ComponentName = type.FullName!;
	}

	private ComponentStorageInMemory(Dictionary<Guid, T> components)
	{
		this.components = components;
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

	public bool GetGenericComponent(Guid entity, out T component)
	{
		return components.TryGetValue(entity, out component);
	}

	public void ReplaceComponent(Guid entity, T component)
	{
		if (isReadOnlyComponent)
			if (components.TryGetValue(entity, out _))
				throw new IsReadOnlyComponentException();

		components[entity] = component;
	}

	public override void RemoveComponent(Guid entity)
	{
		if (isReadOnlyComponent)
			throw new IsReadOnlyComponentException();

		components.Remove(entity);
	}

	public override void Clear()
	{
		components.Clear();
	}

	public override IReadOnlyCollection<Guid> GetEntities()
	{
		return components.Keys;
	}

	public override ComponentStorageInMemory DeepClone()
	{
		// TODO it's not deep clone. copy components collection
		return new ComponentStorageInMemory<T>(new(components));
	}

	public override IEnumerator<KeyValuePair<Guid, IComponent?>> GetEnumerator()
	{
		foreach (var component in components)
		{
			yield return new(component.Key, component.Value);
		}
	}

	public override void Apply(IReadOnlyComponentStorage changes)
	{
		var first = changes.FirstOrDefault(c => c.Value != null);
		if (first.Value != null && first.Value is not T)
			throw new InvalidCastException();

		foreach (var component in changes)
		{
			if (component.Value == null)
				components.Remove(component.Key);
			else
				components[component.Key] = (T) component.Value;
		}
	}
}