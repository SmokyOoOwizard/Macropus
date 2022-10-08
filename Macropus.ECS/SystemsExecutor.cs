using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.Entity;
using Macropus.ECS.System;
using Macropus.ECS.System.Extensions;

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly Dictionary<ASystem, ISystemFilter?> systems;

	public SystemsExecutor(ASystem[] systems)
	{
		this.systems = systems.ToDictionary(s => s, s => s.GetFilter());
	}

	public void Execute(
		IComponentsStorage componentsStorage,
		IEnumerationComponents changes,
		IComponentsStorage futureChanges
	)
	{
		if (changes.Count == 0)
			return;

		foreach (var system in systems)
		{
			var filter = system.Value;

			IEnumerable<IEntity> filteredEntities;
			if (filter != null)
				filteredEntities = EntityWrapper.Wrap(filter.Filter(changes), componentsStorage, futureChanges);
			else
				filteredEntities = EntityWrapper.Wrap(changes.GetEntities(), componentsStorage, futureChanges);

			system.Key.Execute(filteredEntities);
		}
	}
}