using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.Entity;
using Macropus.ECS.System;
using Macropus.ECS.System.Extensions;

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly Dictionary<ASystem, ISystemFilter?> systems;
	private readonly MergedComponentsStorage mergedComponentsStorage = new();

	public SystemsExecutor(ASystem[] systems)
	{
		this.systems = systems.ToDictionary(s => s, s => s.GetFilter());
	}

	public void Execute(
		IComponentsStorage componentsStorage,
		IReadOnlyComponentsStorage changes,
		IComponentsStorage futureChanges
	)
	{
		if (changes.EntitiesCount == 0)
			return;

		mergedComponentsStorage.SetStorages(futureChanges, changes);

		foreach (var system in systems)
		{
			var filter = system.Value;

			IEnumerable<IEntity> filteredEntities;
			if (filter != null)
				filteredEntities = EntityWrapper.Wrap(filter.Filter(mergedComponentsStorage),
					componentsStorage, futureChanges);
			else
				filteredEntities = EntityWrapper.Wrap(mergedComponentsStorage.GetEntities(),
					componentsStorage, futureChanges);

			system.Key.Execute(filteredEntities);
		}
	}
}