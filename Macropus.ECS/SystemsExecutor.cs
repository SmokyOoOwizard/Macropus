using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.Entity;
using Macropus.ECS.System;
using Macropus.ECS.System.Extensions;

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly Dictionary<ASystem, ComponentsFilter?> systems;
	private readonly MergedComponentsStorage changesComponents = new();
	private readonly MergedComponentsStorage componentsStorage = new();

	public SystemsExecutor(params ASystem[] systems)
	{
		this.systems = systems.ToDictionary(s => s, s => s.GetFilter());
	}

	public void Execute(
		IComponentsStorage alreadyExistsComponents,
		IReadOnlyComponentsStorage newComponents,
		IComponentsStorage changedComponents
	)
	{
		if (newComponents.EntitiesCount == 0)
			return;

		changesComponents.SetStorages(changedComponents, newComponents);
		componentsStorage.SetStorages(changesComponents, alreadyExistsComponents);

		foreach (var system in systems)
		{
			var filter = system.Value;

			IEnumerable<IEntity> filteredEntities;
			if (filter != null)
				filteredEntities = EntityWrapper.Wrap(filter.Filter(changesComponents),
					componentsStorage, changedComponents);
			else
				filteredEntities = EntityWrapper.Wrap(changesComponents.GetEntities(),
					componentsStorage, changedComponents);

			system.Key.Execute(filteredEntities);
		}
	}
}