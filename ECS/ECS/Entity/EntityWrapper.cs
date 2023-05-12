using System;
using System.Collections.Generic;
using Macropus.ECS.Component.Storage;

namespace Macropus.ECS.Entity;

public static class EntityWrapper
{
	public static IEnumerable<IEntity> Wrap(
		IEnumerable<Guid> entitiesId,
		IReadOnlyComponentsStorage componentsStorage,
		IComponentsStorage forChanges
	)
	{
		foreach (var id in entitiesId)
			yield return new Impl.Entity(id, componentsStorage, forChanges);
	}
}