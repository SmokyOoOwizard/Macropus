using Macropus.ECS.ComponentsStorage;

namespace Macropus.ECS.Entity;

public static class EntityWrapper
{
	public static IEnumerable<IEntity> Wrap(
		IEnumerable<Guid> entitiesId,
		IComponentsStorage componentsStorage,
		IComponentsStorage forChanges
	)
	{
		foreach (var id in entitiesId) yield return new Impl.Entity(id, componentsStorage, forChanges);
	}
}