using Macropus.ECS.Component;
using Macropus.ECS.Component.Storage;

namespace Macropus.ECS.Entity.Impl;

public readonly struct Entity : IEntity
{
	private readonly IReadOnlyComponentsStorage componentsStorage;
	private readonly IComponentsStorage futureChanges;

	public Guid Id { get; }

	public Entity(Guid id, IReadOnlyComponentsStorage componentsStorage, IComponentsStorage futureChanges)
	{
		Id = id;

		this.componentsStorage = componentsStorage;
		this.futureChanges = futureChanges;
	}

	public bool HasComponent<T>() where T : struct, IComponent
	{
		return futureChanges.HasComponent<T>(Id) || componentsStorage.HasComponent<T>(Id);
	}

	public T GetComponent<T>() where T : struct, IComponent
	{
		if (futureChanges.HasComponent<T>(Id))
			return futureChanges.GetComponent<T>(Id);

		return componentsStorage.GetComponent<T>(Id);
	}

	public void ReplaceComponent<T>(T component) where T : struct, IComponent
	{
		futureChanges.ReplaceComponent(Id, component);
	}

	public void RemoveComponent<T>() where T : struct, IComponent
	{
		futureChanges.RemoveComponent<T>(Id);
	}
}