using Macropus.ECS.Component.Filter;
using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.ComponentsStorage.Impl;
using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Macropus.ECS.Systems.Extensions;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly ASystem[] systems;
	private readonly Dictionary<ASystem, ComponentsFilter> reactiveSystems = new();
	private readonly MergedComponentsStorage changes = new();
	private readonly MergedComponentsStorage storage = new();

	public SystemsExecutor(params ASystem[] systems)
	{
		foreach (var system in systems)
		{
			var filter = system.GetFilter();
			if (filter != null)
			{
				reactiveSystems[system] = filter.Value;
			}
		}

		this.systems = systems;
	}

	public ComponentsFilter[] GetFilters()
	{
		return reactiveSystems.Values.ToArray();
	}

	public void Execute(
		IComponentsStorage alreadyExistsComponents,
		IReadOnlyComponentsStorage newComponents,
		IComponentsStorage changedComponents
	)
	{
		if (newComponents.EntitiesCount == 0)
			return;

		changes.SetStorages(changedComponents, newComponents);
		storage.SetStorages(changes, alreadyExistsComponents);

		foreach (var system in systems)
		{
			(system as IUpdateSystem)?.Update();

			if (reactiveSystems.TryGetValue(system, out var filter))
			{
				IEnumerable<IEntity> filteredEntities =
					EntityWrapper.Wrap(changes.GetEntities(filter), storage, changedComponents);

				(system as IReactiveSystem)?.Execute(filteredEntities);
			}
		}
	}
}