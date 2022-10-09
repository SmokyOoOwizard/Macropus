using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.Entity;
using Macropus.ECS.System;
using Macropus.ECS.System.Extensions;

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly Dictionary<ASystem, ISystemFilter?> systems;
	private readonly MergedComponentsStorage tryTriggerSystemEvents = new();

	public SystemsExecutor(ASystem[] systems)
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

		tryTriggerSystemEvents.SetStorages(changedComponents, newComponents);

		foreach (var system in systems)
		{
			var filter = system.Value;

			IEnumerable<IEntity> filteredEntities;
			if (filter != null)
				filteredEntities = EntityWrapper.Wrap(filter.Filter(tryTriggerSystemEvents),
					alreadyExistsComponents, changedComponents);
			else
				filteredEntities = EntityWrapper.Wrap(tryTriggerSystemEvents.GetEntities(),
					alreadyExistsComponents, changedComponents);

			system.Key.Execute(filteredEntities);
		}
	}
}