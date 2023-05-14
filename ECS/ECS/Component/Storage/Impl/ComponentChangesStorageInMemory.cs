using Macropus.ECS.Component.Exceptions;

namespace Macropus.ECS.Component.Storage.Impl;

public class ComponentChangesStorageInMemory<T> : ComponentStorageInMemory where T : struct, IComponent
{
	private readonly Dictionary<Guid, T?> components;
	private readonly bool isReadOnlyComponent;

	public sealed override string ComponentName { get; protected set; }
	public sealed override Type ComponentType => typeof(T);

	public ComponentChangesStorageInMemory()
	{
		components = new();
		var type = typeof(T);
		isReadOnlyComponent = type.IsAssignableTo(typeof(IReadOnlyComponent));
		ComponentName = type.FullName!;
	}

	private ComponentChangesStorageInMemory(Dictionary<Guid, T?> components)
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

	public bool GetGenericComponent(Guid entity, out T? component)
	{
		return components.TryGetValue(entity, out component);
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
		if (isReadOnlyComponent)
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

	public override ComponentStorageInMemory DeepClone()
	{
		// TODO it's not deep clone. copy components collection
		return new ComponentChangesStorageInMemory<T>(new(components));
	}

	public override IEnumerator<KeyValuePair<Guid, IComponent?>> GetEnumerator()
	{
		foreach (var component in components)
		{
			yield return new KeyValuePair<Guid, IComponent?>(component.Key, component.Value);
		}
	}

	public override void Apply(IReadOnlyComponentStorage changes)
	{
		var first = changes.FirstOrDefault(c => c.Value != null);
		// TODO if all entities has null changes was exp. need add chech
		if (first.Value != null && first.Value is not T)
			throw new InvalidCastException();

		foreach (var component in changes)
		{
			components[component.Key] = (T?)component.Value;
		}
	}
}