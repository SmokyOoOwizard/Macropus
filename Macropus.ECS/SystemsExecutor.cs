using Macropus.ECS.ComponentsStorage;
using Macropus.ECS.Entity;
using Macropus.ECS.Systems;
using Macropus.ECS.Systems.Extensions;
using Macropus.ECS.Systems.Filter;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Macropus.ECS;

public sealed class SystemsExecutor
{
	private readonly ASystem[] systems;
	private readonly Dictionary<ASystem, ComponentsFilter> reactiveSystems = new();
	private readonly MergedComponentsStorage changesComponents = new();
	private readonly MergedComponentsStorage componentsStorage = new();

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
			(system as IUpdateSystem)?.Update();

			if (reactiveSystems.TryGetValue(system, out var filter))
			{
				IEnumerable<IEntity> filteredEntities = EntityWrapper.Wrap(
					changesComponents.GetEntities().Where(id => filter.Filter(id, changesComponents)),
					componentsStorage, changedComponents);

				(system as IReactiveSystem)?.Execute(filteredEntities);
			}
		}
	}
}